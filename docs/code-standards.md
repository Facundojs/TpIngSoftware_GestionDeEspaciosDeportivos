# Code Standards - SIGCAF Project

## Architecture Overview

El proyecto SIGCAF implementa una arquitectura en capas (Layered Architecture) con clara separación de responsabilidades:

```
UI (Presentation) → BLL (Business Logic) → DAL (Data Access) → Database
```

### Layer Responsibilities

**UI Layer (TpIngSoftware_GestionDeEspaciosDeportivos)**
- Windows Forms para interacción con usuarios
- NO contiene lógica de negocio, solo orquestación de formularios
- Consume únicamente los servicios de la capa BLL
- Maneja traducciones via extension methods
- Validaciones opcionales client-side para UX (formato, required fields)

**BLL Layer (Service/Logic)**
- Implementa toda la lógica de negocio y reglas del dominio
- Orquesta operaciones entre múltiples repositorios
- **Validaciones OBLIGATORIAS** de reglas de negocio antes de persistir
- Genera movimientos contables, calcula balances, aplica state machines
- Maneja transacciones vía Unit of Work

**Domain Layer (Domain)**
- Define entidades de negocio (POCOs)
- Define abstracciones y contratos
- Composite Pattern para permisos (Familia/Patente)
- NO contiene lógica de persistencia ni UI

**DAL Layer (Service/Impl & Service/Contracts)**
- Implementa acceso a datos via ADO.NET raw SQL
- Define interfaces (contratos) para repositorios
- Implementaciones específicas por motor (SqlServer)
- NO contiene lógica de negocio, solo queries y comandos

---

## Design Patterns

### 1. Factory + Singleton (FactoryDao)

**Objetivo**: Centralizar creación de dependencias y garantizar única instancia por tipo de repositorio.

**Implementation**:
```csharp
public static class FactoryDao
{
    private static IUsuarioRepository _usuarioRepository;
    
    public static IUsuarioRepository UsuarioRepository
    {
        get
        {
            if (_usuarioRepository == null)
            {
                _usuarioRepository = new UsuarioRepository();
            }
            return _usuarioRepository;
        }
    }
}
```

**Usage**:
```csharp
// En BLL Services
private readonly IUsuarioRepository _repository;
public UsuarioService()
{
    _repository = FactoryDao.UsuarioRepository;
}
```

**Reglas**:
- NUNCA instanciar repositorios directamente con `new`
- Agregar nuevos repositorios al FactoryDao antes de usarlos
- Usar interfaces (IXxxRepository) para desacoplamiento

---

### 2. Unit of Work Pattern

**Objetivo**: Garantizar transaccionalidad ACID en operaciones que involucran múltiples repositorios, permitiendo commit/rollback atómico.

**Referencia**: Basado en [Microsoft - Implementing Repository and Unit of Work Patterns](https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application)

#### Cuándo Aplicar

**Casos que REQUIEREN Unit of Work**:
- Operación modifica 2+ entidades relacionadas donde consistencia es crítica
- Necesidad de rollback completo si cualquier paso falla
- Ejemplos concretos del proyecto:
  - GenerarReserva: INSERT Reserva + INSERT Pago + INSERT Movimiento (deuda) + INSERT Movimiento (seña)
  - RegistrarPago: INSERT Pago + INSERT Movimiento
  - CrearRutina: UPDATE Rutina anterior + INSERT Rutina nueva + INSERT N Ejercicios
  - ReembolsarPago: UPDATE Pago + INSERT Movimiento inverso

**Casos que NO requieren UoW**:
- Operaciones CRUD simples de una sola tabla
- Consultas read-only
- Validaciones que no persisten

#### Implementation Pattern - Transacción Explícita

Para operaciones multi-repositorio, usar transacción explícita compartida:

```csharp
// En BLL Service
public void GenerarReserva(GenerarReservaDTO dto)
{
    // 1. Validaciones previas SIN transacción
    ValidarDisponibilidad(dto.EspacioId, dto.FechaHora, dto.Duracion);
    
    // 2. Unit of Work con conexión compartida
    using (var conn = new SqlConnection(ConnectionManager.GetBusinessConnectionString()))
    {
        conn.Open();
        using (var transaction = conn.BeginTransaction())
        {
            try
            {
                // Inyectar conexión y transacción a repositorios
                var reserva = new Reserva { /* ... */ };
                _reservaRepo.Add(reserva, conn, transaction);
                
                if (dto.Adelanto > 0)
                {
                    var pago = new Pago { /* ... */ };
                    _pagoRepo.Add(pago, conn, transaction);
                }
                
                var movimiento = new Movimiento { /* ... */ };
                _movimientoRepo.Insertar(movimiento, conn, transaction);
                
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw; // Re-throw para que BLL maneje
            }
        }
    }
}
```

#### Modificación Requerida en Repositorios

Para soportar Unit of Work, los repositorios deben aceptar conexión y transacción opcionales:

```csharp
// En BaseRepository
protected void ExecuteNonQuery(
    string query, 
    SqlParameter[] parameters,
    SqlConnection conn = null,
    SqlTransaction transaction = null)
{
    bool externalConnection = conn != null;
    
    if (!externalConnection)
    {
        conn = new SqlConnection(_connectionString);
    }
    
    try
    {
        if (!externalConnection) conn.Open();
        
        using (var cmd = new SqlCommand(query, conn, transaction))
        {
            cmd.Parameters.AddRange(parameters);
            cmd.ExecuteNonQuery();
        }
    }
    finally
    {
        if (!externalConnection) conn?.Dispose();
    }
}

// En repositories específicos
public class ReservaRepository : BaseRepository
{
    public void Add(Reserva obj, SqlConnection conn = null, SqlTransaction tran = null)
    {
        string query = "INSERT INTO Reserva (...) VALUES (...)";
        SqlParameter[] p = { /* ... */ };
        ExecuteNonQuery(query, p, conn, tran);
    }
}
```

**Reglas de Unit of Work**:
- La transacción SIEMPRE vive en BLL, NUNCA en DAL
- Repositorios son stateless y aceptan conexión/transacción como parámetros
- Si no se pasa conexión: el repositorio maneja su propia (comportamiento standard)
- Si se pasa conexión: el repositorio la usa sin dispose (la maneja el caller)

---

### 3. Repository Pattern

**Estructura base**:
```csharp
public interface IGenericRepository<T>
{
    void Add(T obj);
    void Update(T obj);
    void Remove(Guid id);
    T GetById(Guid id);
    List<T> GetAll();
}

// Especializaciones para operaciones específicas
public interface IClienteRepository : IGenericRepository<Cliente>
{
    Cliente GetByDNI(int dni);
    bool TieneDeuda(Guid clienteId);
}
```

**BaseRepository**:
```csharp
public abstract class BaseRepository
{
    protected readonly string _connectionString;
    
    protected BaseRepository(string connectionStringName) 
    {
        _connectionString = ConnectionManager.GetConnectionString(connectionStringName);
    }
    
    protected void ExecuteNonQuery(string query, SqlParameter[] parameters) { }
    protected T ExecuteReader<T>(string query, SqlParameter[] parameters, Func<SqlDataReader, T> map) { }
}
```

**Reglas**:
- Usar `ConnectionManager.BaseConnectionName` para arquitectura base (usuarios/permisos)
- Usar `ConnectionManager.BusinessConnectionName` para dominio de negocio
- Heredar de BaseRepository para reutilizar helpers
- Implementar interfaces específicas cuando se necesitan operaciones custom

---

## Naming Conventions

### Classes
```csharp
// Entities
public class Cliente { }
public class Membresia { }

// Repositories
public class ClienteRepository : BaseRepository, IClienteRepository { }

// Services (BLL)
public class ClienteService { }
public class MembresiaService { }

// DTOs
public class ClienteDTO { }

// Mappers
public static class ClienteMapper { }
```

### Methods
```csharp
// CRUD operations
void Add(T obj)
void Update(T obj)
void Remove(Guid id)
T GetById(Guid id)
List<T> GetAll()

// Business operations
void AsignarMembresia(Guid clienteId, Guid membresiaId)
decimal CalcularSaldoMensual(Guid clienteId)
bool ValidarIngreso(int dni)
```

### Variables
```csharp
// English for code, Spanish for business logic comments when needed
private readonly IClienteRepository _repository;
private readonly ClienteService _clienteService;

// SQL parameters
SqlParameter[] parameters = new SqlParameter[]
{
    new SqlParameter("@Id", clienteId),
    new SqlParameter("@DNI", dni)
};
```

---

## Database Access Standards

### Connection Strings
```csharp
// Defined in App.config
<connectionStrings>
    <add name="IngSoftwareBase" connectionString="..." />
    <add name="IngSoftwareNegocio" connectionString="..." />
</connectionStrings>

// Access via ConnectionManager
ConnectionManager.GetBaseConnectionString();
ConnectionManager.GetBusinessConnectionString();
```

### SQL Query Conventions

**MANDATORIO - SQL Injection Prevention**:
```csharp
// ✅ CORRECTO - Parameterized queries SIEMPRE
string query = "SELECT * FROM Cliente WHERE DNI = @DNI";
SqlParameter[] parameters = { new SqlParameter("@DNI", dni) };

// ❌ PROHIBIDO - String concatenation/interpolation
string query = $"SELECT * FROM Cliente WHERE DNI = {dni}"; // NUNCA HACER ESTO
string query = "SELECT * FROM Cliente WHERE DNI = " + dni; // NUNCA HACER ESTO
```

**Razón**: Prevenir SQL Injection. Usar parámetros es OBLIGATORIO sin excepciones, incluso para valores que parecen "seguros".

### Transaction Management
```csharp
// Para operaciones simples: usar ExecuteNonQuery de BaseRepository
ExecuteNonQuery("INSERT INTO...", parameters);

// Para operaciones multi-tabla: Unit of Work explícito
using (var conn = new SqlConnection(ConnectionManager.GetBusinessConnectionString()))
{
    conn.Open();
    using (var tran = conn.BeginTransaction())
    {
        try
        {
            _reservaRepo.Add(reserva, conn, tran);
            _pagoRepo.Add(pago, conn, tran);
            _movimientoRepo.Insertar(movimiento, conn, tran);
            
            tran.Commit();
        }
        catch 
        { 
            tran.Rollback(); 
            throw; 
        }
    }
}
```

---

## DTO Mapping Pattern

### Strategy: Static Mapper Classes

**Ubicación**: `Service/Mappers/` (crear carpeta)

**Estructura**:
```csharp
public static class ClienteMapper
{
    public static ClienteDTO ToDTO(Cliente entity, Membresia membresia = null, decimal balance = 0)
    {
        if (entity == null) return null;
        
        return new ClienteDTO
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Apellido = entity.Apellido,
            DNI = entity.DNI,
            FechaNacimiento = entity.FechaNacimiento,
            MembresiaID = entity.MembresiaID,
            MembresiaDetalle = membresia?.Nombre ?? "Sin membresía",
            Balance = balance
        };
    }
    
    public static Cliente ToEntity(ClienteDTO dto)
    {
        if (dto == null) return null;
        
        return new Cliente
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            DNI = dto.DNI,
            FechaNacimiento = dto.FechaNacimiento,
            MembresiaID = dto.MembresiaID
        };
    }
    
    // Para listas
    public static List<ClienteDTO> ToDTOList(List<Cliente> entities)
    {
        return entities?.Select(e => ToDTO(e)).ToList() ?? new List<ClienteDTO>();
    }
}
```

**Usage en BLL**:
```csharp
public class ClienteService
{
    public ClienteDTO GetCliente(Guid id)
    {
        var entity = _clienteRepo.GetById(id);
        if (entity == null) throw new Exception("Cliente no encontrado");
        
        var membresia = entity.MembresiaID.HasValue 
            ? _membresiaRepo.GetById(entity.MembresiaID.Value) 
            : null;
            
        var balance = _balanceRepo.GetBalance(id).Saldo;
        
        return ClienteMapper.ToDTO(entity, membresia, balance);
    }
    
    public void RegistrarCliente(ClienteDTO dto)
    {
        var entity = ClienteMapper.ToEntity(dto);
        _clienteRepo.Add(entity);
    }
}
```

**Reglas**:
- Un Mapper por entidad de dominio
- Métodos estáticos: `ToDTO`, `ToEntity`, `ToDTOList`
- Mappers NO contienen lógica de negocio, solo transformación
- Si mapping requiere lógica compleja: hacerlo en BLL, no en Mapper
- DTOs pueden tener más propiedades que entities (para display): OK
- Entities NUNCA deben tener propiedades de UI

---

## Validation Strategy

### Dos Niveles de Validación

#### 1. UI Layer - Validaciones Opcionales (UX)

**Objetivo**: Feedback inmediato al usuario sin roundtrip al servidor.

**Casos de uso**:
- Campos requeridos (TextBox vacío)
- Formato de entrada (DNI numérico, email válido)
- Rangos numéricos (precio > 0, repeticiones > 0)

**Implementation**:
```csharp
private void btnGuardar_Click(object sender, EventArgs e)
{
    // Validaciones UI opcionales pero recomendadas
    if (string.IsNullOrWhiteSpace(txtNombre.Text))
    {
        MessageBox.Show("El nombre es requerido");
        txtNombre.Focus();
        return;
    }
    
    if (!int.TryParse(txtDNI.Text, out int dni) || dni <= 0)
    {
        MessageBox.Show("DNI inválido");
        txtDNI.Focus();
        return;
    }
    
    // Llamar a BLL que RE-VALIDA todo
    try
    {
        _service.RegistrarCliente(dto);
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message);
    }
}
```

#### 2. BLL Layer - Validaciones OBLIGATORIAS

**Regla de Oro**: NUNCA confiar en validaciones de UI. BLL SIEMPRE re-valida todo.

**Tipos de validaciones BLL**:
1. **Validaciones Técnicas**: Formato, rangos, nulls
2. **Validaciones de Negocio**: Reglas del dominio según PDF
3. **Validaciones de Integridad**: Existencia de FK, unicidad

**Implementation Pattern**:
```csharp
public class ClienteService
{
    public void RegistrarCliente(ClienteDTO dto)
    {
        // 1. Validaciones técnicas
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            throw new ArgumentException("El nombre es requerido");
            
        if (dto.DNI <= 0)
            throw new ArgumentException("DNI inválido");
        
        // 2. Validaciones de negocio
        if (_clienteRepo.ExistsByDNI(dto.DNI))
            throw new InvalidOperationException("Ya existe un cliente con ese DNI");
        
        if (dto.MembresiaID.HasValue)
        {
            var membresia = _membresiaRepo.GetById(dto.MembresiaID.Value);
            if (membresia == null || !membresia.Activa)
                throw new InvalidOperationException("La membresía no existe o está inactiva");
        }
        
        // 3. Si validaciones OK: proceder con lógica
        var entity = ClienteMapper.ToEntity(dto);
        _clienteRepo.Add(entity);
        
        // 4. Side effects
        _balanceService.InicializarBalance(entity.Id);
    }
}
```

**Estructura Recomendada**:
```csharp
public void OperacionCompleja(DTO dto)
{
    // Bloque 1: Validaciones (throw si error)
    ValidarDatos(dto);
    ValidarReglasNegocio(dto);
    
    // Bloque 2: Orquestación transaccional
    using (var uow = CreateUnitOfWork())
    {
        // Operaciones...
        uow.Commit();
    }
    
    // Bloque 3: Side effects no transaccionales (logs, emails, etc)
    LogOperacion();
}
```

**Mensajes de Error**:
- Deben ser claros y en español
- Deben indicar QUÉ falló y POR QUÉ (no solo "Error")
- Ejemplo: "No se puede actualizar la membresía porque el cliente tiene un saldo adeudado de $500"

---

## Logging & Auditing Standards

### Política de Logging Obligatoria

**Qué Loguear - SIEMPRE**:

1. **Errores (Level: ERROR)**:
   - Toda excepción capturada en BLL o UI
   - Incluir stack trace completo en `ExceptionDetails`
   - Mensaje user-friendly en `Message`

2. **Operaciones Críticas de Auditoría (Level: INFO)**:
   - Crear/Modificar/Eliminar entidades core: Cliente, Membresia, Usuario
   - Transacciones financieras: Pago, Reembolso
   - Cambios de estado: Reserva (Pendiente→Pagada), Cliente (Activo→Deshabilitado)
   - Login exitoso/fallido

3. **Mínimo 1 Log por Caso de Uso**:
   - Al completar exitosamente un CU: log INFO
   - Formato: `"CU-XXX-YYY ejecutado por {usuario} - {detalles}"`

**Qué NO Loguear**:
- Queries de consulta read-only (excepto si fallan)
- Validaciones fallidas (solo si son inesperadas)
- Navegación de UI

### Implementation Pattern

**En BLL Services**:
```csharp
public class ClienteService
{
    private readonly BitacoraService _logger;
    
    public ClienteService()
    {
        _logger = new BitacoraService();
    }
    
    public void RegistrarCliente(ClienteDTO dto)
    {
        try
        {
            // Validaciones...
            
            // Operación...
            var entity = ClienteMapper.ToEntity(dto);
            _clienteRepo.Add(entity);
            _balanceService.InicializarBalance(entity.Id);
            
            // ✅ LOG OBLIGATORIO - Operación crítica exitosa
            _logger.Log($"CU-CLIE-01 ejecutado: Cliente {dto.DNI} registrado con ID {entity.Id}", "INFO");
        }
        catch (Exception ex)
        {
            // ✅ LOG OBLIGATORIO - Error
            _logger.Log($"Error en CU-CLIE-01 - Cliente {dto.DNI}: {ex.Message}", "ERROR", ex);
            throw; // Re-throw para que UI maneje
        }
    }
}
```

**En UI (Opcional - Solo para Eventos Críticos)**:
```csharp
private void btnLogin_Click(object sender, EventArgs e)
{
    try
    {
        var usuario = _usuarioService.Login(username, password);
        // Login exitoso ya se loguea en UsuarioService
    }
    catch (Exception ex)
    {
        // Login fallido ya se loguea en UsuarioService
        MessageBox.Show("Error: " + ex.Message);
    }
}
```

**Formato de Mensajes**:
```csharp
// Para operaciones exitosas
$"CU-{CODIGO} ejecutado: {EntidadTipo} {Identificador} - {Accion}"
// Ejemplo: "CU-MEMB-01 ejecutado: Membresia MENS01 creada con precio $5000"

// Para errores
$"Error en CU-{CODIGO} - {Contexto}: {Exception.Message}"
// Ejemplo: "Error en CU-PAG-01 - Pago Cliente 12345678: Monto excede saldo adeudado"
```

---

## Composite Pattern - Permisos

### Estructura Implementada

```csharp
// Componente abstracto
public abstract class Acceso
{
    public Guid Id { get; set; }
    public string Nombre { get; set; }
    public abstract List<Acceso> Accesos { get; }
    public abstract void Agregar(Acceso componente);
    public abstract void Eliminar(Acceso componente);
}

// Hoja (Leaf)
public class Patente : Acceso
{
    public string TipoAcceso { get; set; } // Constante de PermisoKeys
    public string DataKey { get; set; }
    
    public override List<Acceso> Accesos => new List<Acceso>();
    public override void Agregar(Acceso c) => throw new InvalidOperationException();
    public override void Eliminar(Acceso c) => throw new InvalidOperationException();
}

// Compuesto (Composite)
public class Familia : Acceso
{
    private readonly List<Acceso> _hijos = new List<Acceso>();
    
    public override List<Acceso> Accesos => _hijos;
    
    public override void Agregar(Acceso componente)
    {
        if (!_hijos.Any(x => x.Id == componente.Id))
            _hijos.Add(componente);
    }
    
    public override void Eliminar(Acceso componente)
    {
        var item = _hijos.FirstOrDefault(x => x.Id == componente.Id);
        if (item != null) _hijos.Remove(item);
    }
}
```

### Verificación de Permisos Recursiva

**Helper Method** (`Service/Helpers/PermisoHelper.cs`):
```csharp
public static class PermisoHelper
{
    public static bool TienePermiso(this UsuarioDTO user, string permiso)
    {
        if (user?.Permisos == null) return false;
        
        foreach (var acceso in user.Permisos)
        {
            if (Check(acceso, permiso)) return true;
        }
        return false;
    }
    
    private static bool Check(Acceso acceso, string permiso)
    {
        // Caso base: es una Patente
        if (acceso is Patente patente)
        {
            return patente.TipoAcceso == permiso;
        }
        
        // Caso recursivo: es una Familia
        if (acceso is Familia familia)
        {
            foreach (var hijo in familia.Accesos)
            {
                if (Check(hijo, permiso)) return true;
            }
        }
        
        return false;
    }
}
```

**Usage**:
```csharp
// En UI
if (!_currentUser.TienePermiso(PermisoKeys.ClienteCrear))
{
    btnCrear.Enabled = false;
}

// En BLL (para validaciones críticas)
if (!_currentUser.TienePermiso(PermisoKeys.BackupRealizar))
{
    throw new UnauthorizedAccessException("Usuario no tiene permiso para realizar backups");
}
```

### Definición de Permisos

**Todas las constantes en un solo lugar** (`Domain/Composite/PermisoKeys.cs`):
```csharp
public static class PermisoKeys
{
    // Backup Permissions
    public const string BackupRealizar = "PERMISSION_BACKUP_EXECUTE";
    public const string BackupRestore = "PERMISSION_BACKUP_RESTORE";
    
    // User Management
    public const string UsuarioListar = "PERMISSION_USER_LIST";
    public const string UsuarioCrear = "PERMISSION_USER_CREATE";
    
    // Business Permissions
    public const string ClienteListar = "PERMISSION_CLIENT_LIST";
    public const string ClienteCrear = "PERMISSION_CLIENT_CREATE";
    // ... etc
}
```

**Reglas**:
- Toda nueva funcionalidad debe tener su permiso definido aquí
- Nombrar como: `{Entidad}{Accion}` en PascalCase
- Valor constante: `"PERMISSION_{ENTITY}_{ACTION}"` en UPPER_SNAKE_CASE
- Al agregar nuevo permiso: actualizar `PermisosService.EnsurePermissions()` para auto-seed

### Gestión de Familias

**Familia "Administrador"** (auto-creada en startup):
- Contiene TODAS las patentes del sistema
- Auto-actualizada cuando se agregan nuevos permisos
- No debe ser modificada manualmente

**Familias Custom** (creadas por admin):
- Subconjunto de permisos para roles específicos
- Ejemplo: Familia "Recepcionista" = {ClienteListar, ClienteCrear, ReservaCrear}

---

## Service Layer Standards

### Structure
```csharp
public class ClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IMembresiaRepository _membresiaRepository;
    private readonly BalanceService _balanceService;
    private readonly BitacoraService _logger;
    
    public ClienteService()
    {
        _clienteRepository = FactoryDao.ClienteRepository;
        _membresiaRepository = FactoryDao.MembresiaRepository;
        _balanceService = new BalanceService();
        _logger = new BitacoraService();
    }
    
    public void RegistrarCliente(ClienteDTO dto)
    {
        try
        {
            // 1. Validaciones (ver sección Validation Strategy)
            ValidarDatos(dto);
            ValidarReglasNegocio(dto);
            
            // 2. Mapping
            var entity = ClienteMapper.ToEntity(dto);
            
            // 3. Persistencia (Unit of Work si multi-tabla)
            _clienteRepository.Add(entity);
            
            // 4. Side effects
            _balanceService.InicializarBalance(entity.Id);
            
            // 5. Log obligatorio
            _logger.Log($"CU-CLIE-01: Cliente {dto.DNI} registrado", "INFO");
        }
        catch (Exception ex)
        {
            _logger.Log($"Error CU-CLIE-01: {ex.Message}", "ERROR", ex);
            throw;
        }
    }
    
    private void ValidarDatos(ClienteDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            throw new ArgumentException("El nombre es requerido");
            
        if (dto.DNI <= 0)
            throw new ArgumentException("DNI inválido");
    }
    
    private void ValidarReglasNegocio(ClienteDTO dto)
    {
        if (_clienteRepository.ExistsByDNI(dto.DNI))
            throw new InvalidOperationException($"Ya existe un cliente con DNI {dto.DNI}");
    }
}
```

### Separation of Concerns en Services

**Un Service NO debe**:
- Acceder directamente a base de datos (usar repositorios)
- Manejar UI (MessageBox, Forms, etc)
- Contener SQL inline
- Retornar DataTables o SqlDataReader (solo Entities o DTOs)

**Un Service DEBE**:
- Validar exhaustivamente antes de persistir
- Loguear operaciones críticas y errores
- Usar Unit of Work para operaciones multi-tabla
- Lanzar excepciones con mensajes claros

---

## Error Handling Strategy

### Exception Types

**Usar tipos específicos para diferentes errores**:
```csharp
// Validaciones de entrada
throw new ArgumentException("El precio debe ser mayor a cero");
throw new ArgumentNullException(nameof(dto), "El DTO no puede ser nulo");

// Reglas de negocio violadas
throw new InvalidOperationException("No se puede actualizar: el cliente tiene deuda");
throw new InvalidOperationException("La membresía está deshabilitada");

// Entidad no encontrada
throw new KeyNotFoundException($"No se encontró cliente con ID {id}");

// Permisos
throw new UnauthorizedAccessException("No tiene permisos para esta operación");
```

### Service Layer
```csharp
// Throw exceptions con mensajes de negocio claros (Spanish OK)
throw new InvalidOperationException("El cliente tiene deuda pendiente de $500. Debe saldar antes de cambiar membresía.");
throw new ArgumentException("La membresía no existe o está deshabilitada");

// Log ANTES de re-throw
catch (Exception ex)
{
    _logger.Log($"Error en operación: {ex.Message}", "ERROR", ex);
    throw; // Re-lanzar para que UI maneje
}
```

### Repository Layer
```csharp
// Let SQL exceptions bubble up, o wrap con contexto
catch (SqlException ex)
{
    throw new InvalidOperationException($"Error al guardar cliente en BD: {ex.Message}", ex);
}
```

### UI Layer
```csharp
// Catch genérico, traducir, mostrar al usuario
try
{
    _service.Operation();
    MessageBox.Show("MSG_SUCCESS".Translate(), "TITLE_SUCCESS".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);
}
catch (ArgumentException ex)
{
    // Errores de validación
    MessageBox.Show(ex.Message, "Datos Inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
catch (InvalidOperationException ex)
{
    // Errores de reglas de negocio
    MessageBox.Show(ex.Message, "Operación No Permitida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
catch (UnauthorizedAccessException ex)
{
    // Errores de permisos
    MessageBox.Show(ex.Message, "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
}
catch (Exception ex)
{
    // Errores inesperados
    MessageBox.Show($"Error inesperado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
```

---

## Critical Business Rules (from PDF)

### 1. Cliente debe estar sin deuda para:
- Hacer check-in (Req 3.1.5)
- Actualizar membresía (Req 3.1.4)
- Registrarse (Req 3.1.1)

**Implementation**:
```csharp
private void ValidarSinDeuda(Guid clienteId)
{
    var balance = _balanceService.ConsultarBalance(clienteId);
    if (balance.Saldo < 0)
    {
        throw new InvalidOperationException(
            $"El cliente tiene una deuda de ${Math.Abs(balance.Saldo):N2}. Debe saldar antes de continuar."
        );
    }
}
```

### 2. Movimientos son inmutables

**Regla ABSOLUTA**: Una vez creado un Movimiento, NUNCA se modifica ni elimina.

**Implementation**:
```csharp
// ✅ CORRECTO
public interface IMovimientoRepository
{
    void Insertar(Movimiento mov); // Solo INSERT
    List<Movimiento> GetByCliente(Guid clienteId); // Solo SELECT
}

// ❌ PROHIBIDO
public interface IMovimientoRepository
{
    void Update(Movimiento mov); // NO IMPLEMENTAR
    void Delete(Guid id); // NO IMPLEMENTAR
}
```

**Reembolsos**: Se registran como nuevo movimiento con monto inverso:
```csharp
// Original: Pago $500 → Movimiento +$500
// Reembolso: NO se borra el movimiento, se crea uno nuevo con -$500
```

### 3. Auditoría obligatoria

**Toda operación crítica debe**:
1. Registrar en Bitácora el usuario que la ejecutó
2. Incluir timestamp exacto
3. Detalles de la transacción (IDs involucrados, montos, etc)

**Para operaciones de UI**:
```csharp
// Pasar UsuarioDTO actual desde UI a BLL
public void RegistrarPago(PagoDTO dto, UsuarioDTO usuarioActual)
{
    // ...operación...
    
    _logger.Log($"CU-PAG-01: Usuario {usuarioActual.Username} registró pago de ${dto.Monto} para cliente {dto.ClienteId}", "INFO");
}
```

### 4. Reservas generan deuda automática

**Flujo completo** (del PDF Req 3.1.6):
```csharp
public void GenerarReserva(GenerarReservaDTO dto)
{
    decimal montoTotal = CalcularMontoTotal(dto.EspacioId, dto.Duracion);
    
    using (var conn = new SqlConnection(_connString))
    {
        conn.Open();
        using (var tran = conn.BeginTransaction())
        {
            try
            {
                // 1. Crear reserva
                var reserva = new Reserva { /* ... */, Estado = "Pendiente" };
                _reservaRepo.Add(reserva, conn, tran);
                
                // 2. Generar DEUDA total de la reserva (movimiento negativo)
                var movDeuda = new Movimiento
                {
                    ClienteID = dto.ClienteId,
                    Monto = -montoTotal, // NEGATIVO
                    Tipo = "DeudaReserva",
                    Descripcion = $"Reserva {reserva.CodigoReserva}"
                };
                _movimientoRepo.Insertar(movDeuda, conn, tran);
                
                // 3. Si pagó seña: registrar pago que reduce la deuda
                if (dto.Adelanto > 0)
                {
                    var pago = new Pago { /* ... */, Monto = dto.Adelanto };
                    _pagoRepo.Add(pago, conn, tran);
                    
                    var movPago = new Movimiento
                    {
                        ClienteID = dto.ClienteId,
                        Monto = dto.Adelanto, // POSITIVO
                        Tipo = "PagoSeña",
                        PagoID = pago.Id
                    };
                    _movimientoRepo.Insertar(movPago, conn, tran);
                }
                
                tran.Commit();
            }
            catch { tran.Rollback(); throw; }
        }
    }
    
    // Balance resultante: -montoTotal + adelanto = saldo pendiente
}
```

---

## UI Standards

### Form Structure
```csharp
public partial class FrmClientes : Form
{
    private readonly UsuarioDTO _currentUser;
    private readonly ClienteService _clienteService;
    
    public FrmClientes(UsuarioDTO currentUser)
    {
        InitializeComponent();
        _currentUser = currentUser;
        _clienteService = new ClienteService();
        UpdateLanguage(); // SIEMPRE después de InitializeComponent
    }
    
    private void FrmClientes_Load(object sender, EventArgs e)
    {
        ApplyPermissions(); // Verificar permisos PRIMERO
        LoadData(); // Luego cargar datos
    }
    
    private void ApplyPermissions()
    {
        if (_currentUser == null) return;
        
        btnCreate.Enabled = _currentUser.TienePermiso(PermisoKeys.ClienteCrear);
        btnUpdate.Enabled = _currentUser.TienePermiso(PermisoKeys.ClienteModificar);
        btnDelete.Enabled = _currentUser.TienePermiso(PermisoKeys.ClienteEliminar);
        
        // Si no tiene permiso para listar: cerrar form
        if (!_currentUser.TienePermiso(PermisoKeys.ClienteListar))
        {
            MessageBox.Show("MSG_NO_PERMISSION".Translate());
            this.Close();
        }
    }
    
    private void UpdateLanguage()
    {
        this.Text = "CLIENTE_TITLE".Translate();
        btnCreate.Text = "BTN_CREATE".Translate();
        // ... resto de controles
    }
}
```

### Permission Checks Pattern

```csharp
// Patrón estándar para todas las pantallas de gestión
private void ApplyPermissions()
{
    bool canList = _currentUser.TienePermiso(PermisoKeys.XxxListar);
    bool canCreate = _currentUser.TienePermiso(PermisoKeys.XxxCrear);
    bool canUpdate = _currentUser.TienePermiso(PermisoKeys.XxxModificar);
    bool canDelete = _currentUser.TienePermiso(PermisoKeys.XxxEliminar);
    
    // Si no puede listar: cerrar pantalla
    if (!canList)
    {
        MessageBox.Show("MSG_NO_PERMISSION".Translate());
        this.Close();
        return;
    }
    
    // Habilitar/deshabilitar botones según permisos
    btnCreate.Enabled = canCreate;
    btnUpdate.Enabled = canUpdate;
    btnDelete.Enabled = canDelete;
}
```

---

## Translation System

### Structure
```
Languages/
  Language.es-MX
  Language.en-US (futuro)
```

### Usage
```csharp
// Via extension method (preferido)
string text = "KEY_NAME".Translate();

// Aplicado a controles
this.Text = "FORM_TITLE".Translate();
btnSave.Text = "BTN_SAVE".Translate();
MessageBox.Show("MSG_SUCCESS".Translate());
```

### Key Naming Convention
```
// Format: CONTEXT_TYPE_DESCRIPTION
CLIENTE_TITLE=Gestión de Clientes
BTN_SAVE=Guardar
BTN_CREATE=Crear
BTN_UPDATE=Modificar
BTN_DELETE=Eliminar
MSG_SUCCESS_CREATE=Registro creado exitosamente
MSG_ERROR_NOT_FOUND=No se encontró el registro
MSG_CONFIRM_DELETE=¿Está seguro de eliminar este registro?
LBL_DNI=DNI
LBL_NOMBRE=Nombre
ERR_VALIDATION_REQUIRED=Este campo es requerido
ERR_VALIDATION_INVALID=El valor ingresado no es válido
```

**Reglas de Traducción**:
- Prefijos: `FORM_` (títulos), `BTN_` (botones), `LBL_` (labels), `MSG_` (mensajes), `ERR_` (errores)
- Keys en UPPER_SNAKE_CASE
- Valores en español claro y conciso
- NO incluir signos de puntuación en values (agregarlos en código si es necesario)

---

## Code Style

### Comments
```csharp
// English para comentarios técnicos de implementación
// Calculate monthly balance based on membership periodicity

// Spanish aceptable para reglas de negocio complejas del PDF
// Según requerimiento 3.1.4: No se puede actualizar membresía si el cliente tiene deuda
```

### Formatting
- Seguir estilo existente del proyecto (Allman braces, 4-space indent)
- Mantener métodos enfocados y cortos (< 30 líneas ideal)
- Extraer condiciones complejas a métodos con nombres descriptivos

```csharp
// ✅ BUENO
if (ClienteTieneDeuda(clienteId))
    throw new InvalidOperationException("Cliente tiene deuda pendiente");

// ❌ EVITAR
if (_balanceService.ConsultarBalance(clienteId).Saldo < 0)
    throw new InvalidOperationException("Cliente tiene deuda pendiente");
```

### GUID Usage
```csharp
// SIEMPRE usar GUID para entity IDs (ya implementado en proyecto)
public Guid Id { get; set; }

// Inicialización en constructor
public Cliente()
{
    Id = Guid.NewGuid();
}

// Comparación
if (cliente.Id == Guid.Empty) // Cliente no inicializado
```

### Null Handling
```csharp
// Usar null-conditional operator
var nombre = cliente?.Nombre ?? "Sin nombre";

// Validar nulls en BLL
if (dto == null)
    throw new ArgumentNullException(nameof(dto));
```

---

## Database Schema Conventions

### Table Names
- Singular, PascalCase (matches entity name)
- Examples: `Cliente`, `Membresia`, `Pago`

### Column Names
- PascalCase (matches property name)
- Foreign keys: `EntityID` (e.g., `ClienteID`, `MembresiaID`)
- Timestamps: `Fecha`, `FechaCreacion`, `UltimaActualizacion`

### Primary Keys
```sql
Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID()
```

### Foreign Keys
```sql
ClienteID UNIQUEIDENTIFIER NOT NULL,
FOREIGN KEY (ClienteID) REFERENCES Cliente(Id)
```

### Constraints Importantes
```sql
-- Check constraints para reglas de negocio
CHECK (Precio >= 0)
CHECK (Regularidad > 0)
CHECK (Estado IN ('Pendiente', 'Pagada', 'Cancelada'))

-- Unique constraints
CONSTRAINT UQ_Cliente_DNI UNIQUE (DNI)
CONSTRAINT UQ_Membresia_Codigo UNIQUE (Codigo)
```

### Indexes
```sql
-- Para queries frecuentes
CREATE INDEX IX_Movimiento_ClienteID ON Movimiento(ClienteID);
CREATE INDEX IX_Movimiento_Fecha ON Movimiento(Fecha);
CREATE INDEX IX_Reserva_Fecha ON Reserva(Fecha);
CREATE INDEX IX_Reserva_Estado ON Reserva(Estado);
```

### Vistas Materializadas
```sql
-- Balance derivado de movimientos (patrón del proyecto)
CREATE VIEW vw_Balance AS
SELECT
    ClienteID,
    SUM(Monto) AS Saldo,
    MAX(Fecha) AS UltimaActualizacion
FROM Movimiento
GROUP BY ClienteID;
```

**Ventaja**: Balance SIEMPRE consistente con movimientos (source of truth).
**Desventaja**: Performance en tablas grandes (considerar indexed view si es necesario).

---

## Business Logic Patterns

### Immutable Audit Trail
```csharp
// Movimientos son INMUTABLES (según spec 2.1.4 del PDF)
// NUNCA UPDATE ni DELETE, solo INSERT
public void RegistrarMovimiento(Movimiento mov)
{
    // ✅ CORRECTO
    _movimientoRepo.Insertar(mov);
    
    // ❌ PROHIBIDO
    // _movimientoRepo.Update(mov); // Método NO debe existir
    // _movimientoRepo.Delete(mov.Id); // Método NO debe existir
}

// Para "corregir" un movimiento: crear uno inverso
public void CorregirMovimiento(Guid movimientoId, decimal montoCorreccion)
{
    var original = _movimientoRepo.GetById(movimientoId);
    
    var correccion = new Movimiento
    {
        ClienteID = original.ClienteID,
        Monto = -montoCorreccion, // Inverso
        Tipo = "Corrección",
        Descripcion = $"Corrección de movimiento {movimientoId}",
        Fecha = DateTime.Now
    };
    
    _movimientoRepo.Insertar(correccion);
}
```

### Balance Calculation
```csharp
// Balance NO se almacena directamente, se deriva de movimientos
// Siempre consultar desde vista vw_Balance
public decimal GetBalance(Guid clienteId)
{
    var balance = _balanceRepo.GetBalance(clienteId);
    return balance?.Saldo ?? 0; // Si no hay movimientos: balance cero
}

// Actualizar balance = crear movimiento
public void ActualizarBalance(Guid clienteId, decimal monto, string descripcion, Guid? pagoId = null)
{
    var movimiento = new Movimiento
    {
        ClienteID = clienteId,
        Monto = monto, // Puede ser positivo (pago) o negativo (deuda)
        Tipo = DeterminarTipo(monto),
        Descripcion = descripcion,
        Fecha = DateTime.Now,
        PagoID = pagoId
    };
    
    _movimientoRepo.Insertar(movimiento);
    // Balance en vw_Balance se actualiza automáticamente
}
```

### State Machines

**Pago: Abonado ↔ Reembolsado** (según PDF 5.5.7):
```csharp
public void ReembolsarPago(Guid pagoId)
{
    var pago = _pagoRepo.GetById(pagoId);
    
    // Validar transición de estado permitida
    if (pago.Estado != "Abonado")
        throw new InvalidOperationException($"Solo se pueden reembolsar pagos en estado Abonado. Estado actual: {pago.Estado}");
    
    // Cambiar estado
    pago.Estado = "Reembolsado";
    _pagoRepo.Update(pago);
    
    // Side effect: crear movimiento inverso
    _balanceService.ActualizarBalance(pago.ClienteID, -pago.Monto, $"Reembolso de pago {pago.Codigo}", pago.Id);
}
```

**Reserva: Pendiente → Pagada / Cancelada** (según PDF 5.7):
```csharp
public void CancelarReserva(Guid reservaId)
{
    var reserva = _reservaRepo.GetById(reservaId);
    
    // Validar transiciones permitidas
    if (reserva.Estado != "Pendiente" && reserva.Estado != "Pagada")
        throw new InvalidOperationException("Solo se pueden cancelar reservas Pendientes o Pagadas");
    
    using (var conn = new SqlConnection(_connString))
    {
        conn.Open();
        using (var tran = conn.BeginTransaction())
        {
            try
            {
                // Cambiar estado
                reserva.Estado = "Cancelada";
                _reservaRepo.Update(reserva, conn, tran);
                
                // Reembolsar seña si existe
                if (reserva.Adelanto > 0)
                {
                    var pago = _pagoRepo.GetByReservaId(reserva.Id);
                    pago.Estado = "Reembolsado";
                    _pagoRepo.Update(pago, conn, tran);
                    
                    // Movimiento de reembolso
                    _movimientoRepo.Insertar(new Movimiento { /* ... */ Monto = reserva.Adelanto }, conn, tran);
                }
                
                // Reversa de deuda de reserva
                _movimientoRepo.Insertar(new Movimiento { /* ... */ Monto = montoTotalReserva }, conn, tran);
                
                tran.Commit();
            }
            catch { tran.Rollback(); throw; }
        }
    }
}
```

**Definir Estados en Código**:
```csharp
// En Domain/Entities/EstadosReserva.cs
public static class EstadosReserva
{
    public const string Pendiente = "Pendiente";
    public const string Pagada = "Pagada";
    public const string Cancelada = "Cancelada";
}

// Uso
reserva.Estado = EstadosReserva.Pendiente;

// Validar transiciones
if (reserva.Estado != EstadosReserva.Pendiente && reserva.Estado != EstadosReserva.Pagada)
    throw new InvalidOperationException("Transición de estado no permitida");
```

---

## File Organization

```
Solution/
├── Domain/                     # Entities + Abstractions
│   ├── Entities/              # POCOs (Cliente, Membresia, Pago, etc)
│   ├── Composite/             # Permissions (Familia/Patente/Acceso)
│   └── Usuario.cs             # Base abstract class
│
├── Service/                    # BLL + DAL
│   ├── Contracts/             # Interfaces (IRepository, IService)
│   ├── DTO/                   # Data Transfer Objects
│   ├── Factory/               # FactoryDao (Singleton)
│   ├── Helpers/               # ConnectionManager, CryptographyHelper, PermisoHelper, etc.
│   ├── Impl/                  # Repository implementations
│   │   ├── SqlServer/        # SQL Server specific (BaseRepository + concrete repos)
│   │   ├── Text/             # LanguageRepository
│   │   └── Console/          # ConsoleRepository (fallback logger)
│   ├── Logic/                 # Business Logic Services (BLL)
│   └── Mappers/               # DTO ↔ Entity mappers (crear esta carpeta)
│
└── UI/                        # Windows Forms
    ├── Frm*.cs               # Form implementations
    ├── Languages/            # Translation files (linked from Service)
    └── App.config            # Connection strings + settings
```

---

## Security Standards

### SQL Injection Prevention

**REGLA ABSOLUTA**: NUNCA concatenar valores directamente en SQL. SIEMPRE usar parámetros.

```csharp
// ✅ CORRECTO
string query = "SELECT * FROM Cliente WHERE DNI = @DNI AND Nombre LIKE @Nombre";
SqlParameter[] parameters = 
{
    new SqlParameter("@DNI", dni),
    new SqlParameter("@Nombre", "%" + nombre + "%")
};

// ❌ PROHIBIDO - Vulnerable a SQL Injection
string query = $"SELECT * FROM Cliente WHERE DNI = {dni}"; // NUNCA
string query = "SELECT * FROM Cliente WHERE Nombre = '" + nombre + "'"; // NUNCA
```

**Incluso para valores "seguros"**: GUIDs, enums, fechas → SIEMPRE parámetros.

### Password Hashing

**Patrón implementado** (`Service/Helpers/CryptographyHelper.cs`):
```csharp
// Al registrar/cambiar password
string hashedPassword = CryptographyHelper.HashPassword(plainTextPassword);
usuario.Password = hashedPassword;

// Al validar login
string hashedInput = CryptographyHelper.HashPassword(inputPassword);
if (usuario.Password != hashedInput)
    throw new Exception("Contraseña incorrecta");
```

**NUNCA almacenar passwords en plain text**.

### Digito Verificador (DV)

**Patrón implementado** en `UsuarioService`:
```csharp
private void UpdateDV(Usuario user)
{
    string raw = $"{user.Id}{user.NombreUsuario}{user.Password}{user.Estado}";
    user.DigitoVerificador = CryptographyHelper.HashPassword(raw);
}

// Llamar SIEMPRE antes de Add/Update
var usuario = new Usuario { /* ... */ };
UpdateDV(usuario);
_usuarioRepo.Add(usuario);
```

**Propósito**: Detectar modificaciones directas en DB (integrity check).

---

## Performance Considerations

### Query Optimization

**Usar índices apropiados**:
```sql
-- Para búsquedas frecuentes
CREATE INDEX IX_Cliente_DNI ON Cliente(DNI);

-- Para filtros en reportes
CREATE INDEX IX_Pago_Fecha ON Pago(Fecha);
CREATE INDEX IX_Movimiento_ClienteID_Fecha ON Movimiento(ClienteID, Fecha);
```

**Evitar SELECT * en producción**:
```csharp
// ❌ Evitar
string query = "SELECT * FROM Cliente";

// ✅ Preferir (solo columnas necesarias)
string query = "SELECT Id, Nombre, Apellido, DNI FROM Cliente";
```

**Paginación para listas grandes**:
```csharp
// Pattern usado en BitacoraService
public List<Log> GetLogs(int pageNumber, int pageSize, /* filtros */)
{
    int offset = (pageNumber - 1) * pageSize;
    
    string query = @"SELECT * FROM Bitacora 
                     WHERE /* condiciones */
                     ORDER BY Timestamp DESC 
                     OFFSET @Offset ROWS 
                     FETCH NEXT @PageSize ROWS ONLY";
    // ...
}
```

### DataGridView Performance

**Para grids con muchos registros**:
```csharp
// Deshabilitar auto-generate columns
dgv.AutoGenerateColumns = false;

// Definir columnas manualmente
dgv.Columns.Add(new DataGridViewTextBoxColumn 
{ 
    DataPropertyName = "Nombre", 
    HeaderText = "Nombre" 
});

// Virtual mode para datasets muy grandes (> 10k rows)
dgv.VirtualMode = true;
```

---

## Exception Handling Patterns

### Try-Catch Scope

**En BLL**:
```csharp
public void OperacionCritica(DTO dto)
{
    try
    {
        // Validaciones (pueden lanzar ArgumentException)
        Validar(dto);
        
        // Lógica transaccional
        using (var uow = CreateUnitOfWork())
        {
            // ...
            uow.Commit();
        }
        
        // Log exitoso
        _logger.Log("Operación completada", "INFO");
    }
    catch (Exception ex)
    {
        // Log error
        _logger.Log($"Error: {ex.Message}", "ERROR", ex);
        
        // Re-throw para que UI maneje
        throw;
    }
}
```

**En UI**:
```csharp
private void btnOperation_Click(object sender, EventArgs e)
{
    try
    {
        _service.OperacionCritica(dto);
        MessageBox.Show("MSG_SUCCESS".Translate(), "TITLE_SUCCESS".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    catch (ArgumentException ex)
    {
        MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
    catch (InvalidOperationException ex)
    {
        MessageBox.Show(ex.Message, "Operación No Permitida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error inesperado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
```

**NUNCA catchear y silenciar excepciones sin loguear**.

---

## Data Transfer Objects (DTOs)

### Purpose

DTOs sirven para:
1. **Desacoplar** UI de entities (agregar campos calculados sin modificar domain)
2. **Seguridad**: No exponer campos internos (Password, DigitoVerificador)
3. **Performance**: Evitar lazy loading issues con navegación de propiedades

### Naming Convention
```csharp
// {Entity}DTO para DTOs básicos
public class ClienteDTO { }

// {Accion}{Entity}DTO para DTOs específicos de operaciones
public class GenerarReservaDTO { }
public class ResultadoIngresoDTO { }
```

### DTO Structure Pattern
```csharp
public class ClienteDTO
{
    // Propiedades de la entidad
    public Guid Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public int DNI { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public Guid? MembresiaID { get; set; }
    
    // Propiedades calculadas/agregadas para display
    public string NombreCompleto => $"{Nombre} {Apellido}";
    public string MembresiaDetalle { get; set; } // Join con tabla Membresia
    public decimal Balance { get; set; } // Join con vista vw_Balance
    public string Estado { get; set; } // "Activo" / "Deuda" / "Deshabilitado"
}
```

### Mappers Location

**Crear carpeta**: `Service/Mappers/`

**Un Mapper por entidad de dominio**:
- `ClienteMapper.cs`
- `MembresiaMapper.cs`
- `PagoMapper.cs`
- etc.

**Responsabilidades de Mappers**:
- Transformación pura Entity ↔ DTO
- NO lógica de negocio
- NO queries a DB (recibir datos ya cargados)

```csharp
public static class ClienteMapper
{
    // Entity → DTO con datos adicionales opcionales
    public static ClienteDTO ToDTO(Cliente entity, Membresia membresia = null, decimal? balance = null)
    {
        if (entity == null) return null;
        
        return new ClienteDTO
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Apellido = entity.Apellido,
            DNI = entity.DNI,
            FechaNacimiento = entity.FechaNacimiento,
            MembresiaID = entity.MembresiaID,
            MembresiaDetalle = membresia?.Nombre ?? "Sin membresía",
            Balance = balance ?? 0,
            Estado = DeterminarEstado(balance ?? 0)
        };
    }
    
    // DTO → Entity (para persistir)
    public static Cliente ToEntity(ClienteDTO dto)
    {
        if (dto == null) return null;
        
        return new Cliente
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            DNI = dto.DNI,
            FechaNacimiento = dto.FechaNacimiento,
            MembresiaID = dto.MembresiaID
        };
    }
    
    // Helpers privados
    private static string DeterminarEstado(decimal balance)
    {
        if (balance < 0) return "Deuda";
        if (balance > 0) return "A Favor";
        return "Al Día";
    }
    
    // Para listas
    public static List<ClienteDTO> ToDTOList(List<Cliente> entities)
    {
        return entities?.Select(ToDTO).ToList() ?? new List<ClienteDTO>();
    }
}
```

**Usage en Service**:
```csharp
public ClienteDTO GetCliente(Guid id)
{
    var entity = _clienteRepo.GetById(id);
    if (entity == null) return null;
    
    // Cargar datos relacionados si es necesario
    Membresia membresia = null;
    if (entity.MembresiaID.HasValue)
    {
        membresia = _membresiaRepo.GetById(entity.MembresiaID.Value);
    }
    
    var balance = _balanceRepo.GetBalance(id)?.Saldo ?? 0;
    
    // Mapear con datos completos
    return ClienteMapper.ToDTO(entity, membresia, balance);
}
```

---

## Validation Patterns

### UI Layer Validation (Opcional)

**Propósito**: Mejorar UX con feedback inmediato, NO reemplazar validaciones BLL.

**ErrorProvider Pattern**:
```csharp
private ErrorProvider errorProvider = new ErrorProvider();

private bool ValidarFormulario()
{
    errorProvider.Clear();
    bool valid = true;
    
    if (string.IsNullOrWhiteSpace(txtNombre.Text))
    {
        errorProvider.SetError(txtNombre, "ERR_VALIDATION_REQUIRED".Translate());
        valid = false;
    }
    
    if (!int.TryParse(txtDNI.Text, out int dni) || dni <= 0)
    {
        errorProvider.SetError(txtDNI, "ERR_VALIDATION_INVALID".Translate());
        valid = false;
    }
    
    return valid;
}

private void btnGuardar_Click(object sender, EventArgs e)
{
    if (!ValidarFormulario()) return; // Validación UI
    
    try
    {
        _service.Guardar(dto); // BLL re-valida todo
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message);
    }
}
```

### BLL Layer Validation (OBLIGATORIA)

**Estructura Recomendada**:
```csharp
public class ClienteService
{
    public void RegistrarCliente(ClienteDTO dto)
    {
        // BLOQUE 1: Validaciones técnicas
        ValidarDatosTecnicos(dto);
        
        // BLOQUE 2: Validaciones de negocio
        ValidarReglasNegocio(dto);
        
        // BLOQUE 3: Operación (si todo OK)
        var entity = ClienteMapper.ToEntity(dto);
        _clienteRepo.Add(entity);
    }
    
    private void ValidarDatosTecnicos(ClienteDTO dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));
            
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            throw new ArgumentException("El nombre es requerido");
            
        if (string.IsNullOrWhiteSpace(dto.Apellido))
            throw new ArgumentException("El apellido es requerido");
            
        if (dto.DNI <= 0)
            throw new ArgumentException("El DNI debe ser un número positivo");
            
        if (dto.FechaNacimiento >= DateTime.Today)
            throw new ArgumentException("La fecha de nacimiento no puede ser futura");
    }
    
    private void ValidarReglasNegocio(ClienteDTO dto)
    {
        // Regla: DNI único (del PDF CU-CLIE-01 flujo alt 3.1)
        if (_clienteRepo.ExistsByDNI(dto.DNI))
            throw new InvalidOperationException($"Ya existe un cliente registrado con DNI {dto.DNI}");
        
        // Regla: Membresía debe existir y estar activa
        if (dto.MembresiaID.HasValue)
        {
            var membresia = _membresiaRepo.GetById(dto.MembresiaID.Value);
            if (membresia == null)
                throw new InvalidOperationException("La membresía especificada no existe");
                
            if (!membresia.Activa)
                throw new InvalidOperationException("La membresía está deshabilitada y no puede asignarse");
        }
    }
}
```

**Principio**: Separar validaciones técnicas (formato, required) de reglas de negocio (existencia, estados, relaciones).

---

## Common Patterns & Best Practices

### GUID Handling
```csharp
// Nuevo registro: generar en constructor
public Cliente()
{
    Id = Guid.NewGuid();
}

// Verificar si es nuevo
if (dto.Id == Guid.Empty)
{
    // Crear nuevo
}
else
{
    // Actualizar existente
}

// Comparación
if (cliente.Id == otraEntidad.Id)
```

### Nullable Handling
```csharp
// Usar null-conditional y null-coalescing
var nombre = cliente?.Membresia?.Nombre ?? "Sin membresía";

// Validar nulls explícitamente en parámetros críticos
public void Metodo(ClienteDTO dto)
{
    if (dto == null)
        throw new ArgumentNullException(nameof(dto));
}

// Nullable GUID para FK opcionales
public Guid? MembresiaID { get; set; }

// SQL Parameters con nulls
new SqlParameter("@MembresiaID", (object)entity.MembresiaID ?? DBNull.Value)
```

### DateTime Handling
```csharp
// Usar DateTime.Now para timestamps
Fecha = DateTime.Now

// Usar DateTime.Today para comparaciones de fechas sin hora
if (reserva.Fecha.Date == DateTime.Today)

// Formateo para display
lblFecha.Text = fecha.ToString("dd/MM/yyyy HH:mm");

// Cálculos de fechas
DateTime proximoPago = DateTime.Now.AddDays(membresia.Regularidad);
```

### Decimal for Money
```csharp
// SIEMPRE usar decimal para montos monetarios
public decimal Precio { get; set; }
public decimal Monto { get; set; }

// Formateo para display
lblPrecio.Text = precio.ToString("C"); // Formato moneda según cultura
lblMonto.Text = $"${monto:N2}"; // $1,234.56

// Comparaciones con tolerancia
const decimal tolerance = 0.01m;
if (Math.Abs(montoEsperado - montoReal) < tolerance)
```

---

## Configuración y Settings

### App.config Structure
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <connectionStrings>
        <add name="IngSoftwareBase" connectionString="..." />
        <add name="IngSoftwareNegocio" connectionString="..." />
    </connectionStrings>
    
    <appSettings>
        <add key="LanguagePath" value="Languages\"/>
        <add key="UserLanguageConfigPath" value="LanguageConfig\UserLanguageConfig.txt"/>
        <add key="BackupDirectory" value="Backups\"/>
        <add key="ComprobantesDirectory" value="Comprobantes\"/>
    </appSettings>
</configuration>
```

### Acceso a Settings
```csharp
// Via ConfigurationManager
string langPath = ConfigurationManager.AppSettings["LanguagePath"];
string backupDir = ConfigurationManager.AppSettings["BackupDirectory"];

// Validar existencia
if (string.IsNullOrEmpty(langPath))
    throw new ConfigurationErrorsException("LanguagePath no configurado en App.config");
```

---

## Logging Standards - Detallado

### Log Levels

**INFO**:
- Operaciones exitosas de casos de uso
- Login exitoso
- Cálculo mensual ejecutado
- Formato: `"CU-XXX-YYY: {descripción breve}"`

**ERROR**:
- Toda excepción capturada
- Login fallido
- Operaciones críticas que fallan
- Incluir `ExceptionDetails` con stack trace completo

### Logging Pattern por Capa

**BLL Layer** (Logging Primario):
```csharp
public class MembresiaService
{
    private readonly BitacoraService _logger;
    
    public void CrearMembresia(MembresiaDTO dto)
    {
        try
        {
            Validar(dto);
            
            var entity = MembresiaMapper.ToEntity(dto);
            _membresiaRepo.Add(entity);
            
            // ✅ Log obligatorio por CU
            _logger.Log($"CU-MEMB-003: Membresía '{dto.Nombre}' creada con código {dto.Codigo} y precio ${dto.Precio}", "INFO");
        }
        catch (Exception ex)
        {
            // ✅ Log obligatorio de error
            _logger.Log($"Error en CU-MEMB-003 al crear membresía '{dto.Nombre}': {ex.Message}", "ERROR", ex);
            throw;
        }
    }
}
```

**DAL Layer** (Sin Logging Directo):
- Repositorios NO loguean (responsabilidad de BLL)
- Excepción: Logs de debugging en desarrollo (removibles en producción)

**UI Layer** (Logging Excepcional):
- Solo para eventos que BLL no captura (ej: abrir/cerrar forms, cambios de idioma)
- Generalmente NO es necesario

### Contexto en Logs

**Incluir información relevante**:
```csharp
// ✅ BUENO - Contexto claro
_logger.Log($"CU-PAG-01: Pago de ${monto} registrado para cliente DNI {dni} por concepto {concepto}", "INFO");

// ❌ MALO - Poco contexto
_logger.Log("Pago registrado", "INFO");

// Para errores: incluir valores que causaron el error
_logger.Log($"Error en CU-RES-01: Espacio {espacioId} no disponible en fecha {fecha:dd/MM/yyyy} hora {hora}", "ERROR", ex);
```

### BitacoraService Usage

```csharp
// Instanciar en constructor de services
private readonly BitacoraService _logger = new BitacoraService();

// Log INFO
_logger.Log(message, "INFO");

// Log ERROR con excepción
_logger.Log(message, "ERROR", ex);

// El service maneja automáticamente el timestamp y persiste en DB
```

---

## Composite Pattern - Working with Permissions

### Loading Permisos Recursivos

**En UsuarioRepository** (ya implementado):
```csharp
public List<Familia> GetFamiliasByUsuarioId(Guid usuarioId)
{
    // 1. Obtener IDs de familias asociadas
    var familiaIds = /* query a UsuarioFamilia */;
    
    // 2. Para cada familia, cargar árbol completo
    var familiaRepo = new FamiliaRepository();
    var familias = new List<Familia>();
    
    foreach (var id in familiaIds)
    {
        var familia = familiaRepo.GetById(id); // Carga Familia + Patentes
        familias.Add(familia);
    }
    
    return familias;
}
```

### Verificación de Permisos Efectivos

**Helper ya implementado** (`Service/Helpers/PermisoHelper.cs`):
```csharp
// Extension method en UsuarioDTO
public static bool TienePermiso(this UsuarioDTO user, string permiso)
{
    // Recorre todo el árbol Composite recursivamente
    // Retorna true si encuentra la patente en cualquier nivel
}
```

**Uso típico**:
```csharp
// En UI - habilitar/deshabilitar controles
btnCrear.Enabled = _currentUser.TienePermiso(PermisoKeys.ClienteCrear);

// En BLL - validación crítica
if (!_currentUser.TienePermiso(PermisoKeys.BackupRealizar))
    throw new UnauthorizedAccessException("No autorizado para realizar backups");
```

### Gestión de Familias

**Auto-Seed de Permisos** (`Service/Logic/PermisosService.cs`):
```csharp
public void EnsurePermissions()
{
    // 1. Leer todas las constantes de PermisoKeys via reflection
    var permissionFields = typeof(PermisoKeys)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.IsLiteral && f.FieldType == typeof(string));
    
    // 2. Por cada constante, crear Patente si no existe
    foreach (var field in permissionFields)
    {
        string permisoValue = (string)field.GetValue(null);
        string permisoName = field.Name;
        
        if (!_patenteRepo.ExistsByTipoAcceso(permisoValue))
        {
            _patenteRepo.Add(new Patente 
            { 
                Nombre = permisoName, 
                TipoAcceso = permisoValue 
            });
        }
    }
    
    // 3. Asegurar que familia "Administrador" tiene TODAS las patentes
    EnsureAdminFamily();
}
```

**Llamar en Application Startup**:
```csharp
// En Form1_Load
try
{
    new PermisosService().EnsurePermissions();
}
catch (Exception ex)
{
    MessageBox.Show($"Error inicializando permisos: {ex.Message}");
}
```

**Beneficio**: Al agregar una nueva constante en `PermisoKeys`, automáticamente se crea la patente en DB al iniciar la app.

---

## Transaction Scope Best Practices

### Cuándo NO usar Transacciones

**Casos donde transacción NO es necesaria**:
- Operaciones read-only (SELECT)
- INSERT/UPDATE/DELETE de una sola tabla sin side effects
- Validaciones que no persisten

```csharp
// ✅ Sin transacción (operación simple)
public void CrearMembresia(MembresiaDTO dto)
{
    Validar(dto);
    var entity = MembresiaMapper.ToEntity(dto);
    _membresiaRepo.Add(entity); // Solo 1 INSERT
    _logger.Log("Membresía creada", "INFO");
}
```

### Cuándo SÍ usar Transacciones

**Regla**: Si la operación modifica 2+ tablas y necesitas atomicidad (todo o nada).

**Checklist para Unit of Work**:
- ✅ ¿La operación modifica múltiples entidades relacionadas?
- ✅ ¿Si falla un paso intermedio, necesito revertir pasos anteriores?
- ✅ ¿La inconsistencia parcial sería inaceptable para el negocio?

Si las 3 respuestas son SÍ → usar Unit of Work.

**Ejemplo - Registrar Pago**:
```csharp
// ❌ SIN transacción (riesgo de inconsistencia)
public void RegistrarPago_Incorrecto(PagoDTO dto)
{
    var pago = PagoMapper.ToEntity(dto);
    _pagoRepo.Add(pago); // ✅ Éxito
    
    // ❌ Si esto falla, el pago quedó registrado pero sin movimiento = balance inconsistente
    var movimiento = new Movimiento { /* ... */ };
    _movimientoRepo.Insertar(movimiento);
}

// ✅ CON transacción (atomicidad garantizada)
public void RegistrarPago_Correcto(PagoDTO dto)
{
    using (var conn = new SqlConnection(_connString))
    {
        conn.Open();
        using (var tran = conn.BeginTransaction())
        {
            try
            {
                var pago = PagoMapper.ToEntity(dto);
                _pagoRepo.Add(pago, conn, tran);
                
                var movimiento = new Movimiento { PagoID = pago.Id, /* ... */ };
                _movimientoRepo.Insertar(movimiento, conn, tran);
                
                tran.Commit(); // Ambas operaciones OK → commit
            }
            catch
            {
                tran.Rollback(); // Si falla cualquiera → rollback completo
                throw;
            }
        }
    }
}
```

### Orden de Operaciones en Transacciones

**Principio**: Operaciones con FK primero, dependientes después.

```csharp
// ✅ CORRECTO
using (var tran = ...)
{
    _reservaRepo.Add(reserva, conn, tran); // PK: Reserva.Id
    _pagoRepo.Add(pago, conn, tran); // FK: Pago.ReservaID → Reserva.Id
    tran.Commit();
}

// ❌ INCORRECTO (Pago insertado antes que Reserva = FK constraint violation)
using (var tran = ...)
{
    _pagoRepo.Add(pago, conn, tran); // FK inválido todavía
    _reservaRepo.Add(reserva, conn, tran);
    tran.Commit();
}
```

---

## Migration from Current Code

### Pasos para Adoptar Unit of Work

**1. Modificar BaseRepository** para aceptar conexión/transacción opcionales:

```csharp
// En Service/Impl/SqlServer/BaseRepository.cs
protected void ExecuteNonQuery(string query, SqlParameter[] parameters, 
    SqlConnection conn = null, SqlTransaction tran = null)
{
    bool isExternalConn = conn != null;
    
    if (!isExternalConn)
    {
        conn = new SqlConnection(_connectionString);
    }
    
    try
    {
        if (!isExternalConn) conn.Open();
        
        using (var cmd = new SqlCommand(query, conn, tran))
        {
            cmd.Parameters.AddRange(parameters);
            cmd.ExecuteNonQuery();
        }
    }
    finally
    {
        if (!isExternalConn) conn?.Dispose();
    }
}
```

**2. Actualizar firmas de métodos en repositorios**:

```csharp
// Antes
public void Add(Cliente obj)

// Después
public void Add(Cliente obj, SqlConnection conn = null, SqlTransaction tran = null)

// Llamada interna usa helpers con conexión pasada
ExecuteNonQuery(query, parameters, conn, tran);
```

**3. Uso desde BLL**:

```csharp
// Operación simple (sin UoW)
_clienteRepo.Add(cliente); // conn y tran = null → repo maneja su conexión

// Operación transaccional (con UoW)
using (var conn = new SqlConnection(connString))
{
    conn.Open();
    using (var tran = conn.BeginTransaction())
    {
        try
        {
            _clienteRepo.Add(cliente, conn, tran);
            _movimientoRepo.Add(mov, conn, tran);
            tran.Commit();
        }
        catch { tran.Rollback(); throw; }
    }
}
```

**Ventaja**: Backward compatible. Código existente sigue funcionando sin cambios.

---

## Dependencies & External Libraries

**Proyecto usa .NET Framework 4.7.2** con libraries estándar:

- `System.Data.SqlClient` - ADO.NET para SQL Server
- `System.Configuration` - App.config access
- `System.Windows.Forms` - UI
- `System.Reflection` - Para auto-seed de permisos

**NO usar**:
- Entity Framework (por decisión de arquitectura Raw SQL)
- Dependency Injection containers (usar Factory manual)
- AutoMapper (mapeo manual en Mappers estáticos)

**Justificación**: Proyecto educativo enfocado en fundamentos antes que frameworks.

---

## Common Pitfalls & How to Avoid

### 1. Olvidar Validar en BLL

❌ **Error**:
```csharp
public void Guardar(DTO dto)
{
    // Confía ciegamente en UI
    var entity = Mapper.ToEntity(dto);
    _repo.Add(entity);
}
```

✅ **Correcto**:
```csharp
public void Guardar(DTO dto)
{
    ValidarDatos(dto); // Re-validar SIEMPRE
    ValidarReglas(dto);
    
    var entity = Mapper.ToEntity(dto);
    _repo.Add(entity);
}
```

### 2. Olvidar Unit of Work en Operaciones Multi-Tabla

❌ **Error**:
```csharp
public void GenerarReserva(DTO dto)
{
    _reservaRepo.Add(reserva); // ✅ OK
    _pagoRepo.Add(pago); // ⚠️ Si falla aquí, reserva quedó huérfana
}
```

✅ **Correcto**:
```csharp
public void GenerarReserva(DTO dto)
{
    using (var conn = new SqlConnection(_connString))
    {
        conn.Open();
        using (var tran = conn.BeginTransaction())
        {
            try
            {
                _reservaRepo.Add(reserva, conn, tran);
                _pagoRepo.Add(pago, conn, tran);
                tran.Commit();
            }
            catch { tran.Rollback(); throw; }
        }
    }
}
```

### 3. Modificar Movimientos en lugar de Crear Inversos

❌ **Error**:
```csharp
public void ReembolsarPago(Guid pagoId)
{
    var mov = _movimientoRepo.GetByPagoId(pagoId);
    mov.Monto = -mov.Monto; // ❌ Modificando registro inmutable
    _movimientoRepo.Update(mov);
}
```

✅ **Correcto**:
```csharp
public void ReembolsarPago(Guid pagoId)
{
    var pago = _pagoRepo.GetById(pagoId);
    
    // Crear NUEVO movimiento inverso
    var movReembolso = new Movimiento
    {
        ClienteID = pago.ClienteID,
        Monto = -pago.Monto, // Inverso del original
        Tipo = "Reembolso",
        Descripcion = $"Reembolso de pago {pago.Codigo}",
        PagoID = pagoId
    };
    
    _movimientoRepo.Insertar(movReembolso);
}
```

### 4. Olvidar Loguear Operaciones

❌ **Error**:
```csharp
public void OperacionCritica(DTO dto)
{
    Validar(dto);
    _repo.Add(entity);
    // ❌ Sin log
}
```

✅ **Correcto**:
```csharp
public void OperacionCritica(DTO dto)
{
    try
    {
        Validar(dto);
        _repo.Add(entity);
        _logger.Log("CU-XXX: Operación exitosa", "INFO"); // ✅ Log obligatorio
    }
    catch (Exception ex)
    {
        _logger.Log("Error en CU-XXX", "ERROR", ex); // ✅ Log error
        throw;
    }
}
```

### 5. No Verificar Permisos en Forms

❌ **Error**:
```csharp
private void FrmClientes_Load(object sender, EventArgs e)
{
    LoadData(); // ❌ Sin verificar permisos
}
```

✅ **Correcto**:
```csharp
private void FrmClientes_Load(object sender, EventArgs e)
{
    ApplyPermissions(); // ✅ Verificar PRIMERO
    LoadData();
}

private void ApplyPermissions()
{
    if (!_currentUser.TienePermiso(PermisoKeys.ClienteListar))
    {
        MessageBox.Show("Sin permisos");
        this.Close(); // Cerrar form si no puede listar
        return;
    }
    
    btnCreate.Enabled = _currentUser.TienePermiso(PermisoKeys.ClienteCrear);
    // ... resto de botones
}
```

---

## Code Review Checklist

Antes de considerar una feature completa, verificar:

**General**:
- [ ] Código sigue naming conventions del proyecto
- [ ] Sin SQL inline en BLL/UI (solo en repositorios)
- [ ] Sin concatenación de strings en queries (solo parámetros)
- [ ] GUIDs usados para todos los IDs
- [ ] Exceptions con mensajes claros en español

**Repository (DAL)**:
- [ ] Hereda de BaseRepository
- [ ] Usa ConnectionManager para connection string
- [ ] Todos los queries parametrizados
- [ ] Métodos aceptan conn/tran opcionales (para UoW)
- [ ] Registrado en FactoryDao

**Service (BLL)**:
- [ ] Validaciones técnicas implementadas
- [ ] Validaciones de reglas de negocio implementadas
- [ ] Unit of Work usado si operación multi-tabla
- [ ] Log obligatorio al completar CU exitosamente
- [ ] Log de errores con contexto
- [ ] Excepciones con mensajes de negocio claros

**Mapper**:
- [ ] Métodos estáticos ToDTO y ToEntity
- [ ] Sin lógica de negocio (transformación pura)
- [ ] Manejo correcto de nulls

**UI**:
- [ ] UsuarioDTO actual pasado al servicio
- [ ] Permisos verificados en Load
- [ ] Traducciones aplicadas en UpdateLanguage()
- [ ] Validaciones UI opcionales con ErrorProvider
- [ ] Try-catch con manejo por tipo de excepción
- [ ] MessageBox con texto traducido

**Business Rules**:
- [ ] Reglas del PDF implementadas fielmente
- [ ] Flujos alternativos del PDF considerados
- [ ] Movimientos inmutables respetados
- [ ] Balance derivado de movimientos (no almacenado)
- [ ] State machines implementadas correctamente
