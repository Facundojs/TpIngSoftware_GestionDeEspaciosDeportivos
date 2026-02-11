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

**BLL Layer (Service/Logic)**
- Implementa toda la lógica de negocio y reglas del dominio
- Orquesta operaciones entre múltiples repositorios
- Valida reglas de negocio antes de persistir
- Genera movimientos contables, calcula balances, aplica state machines

**Domain Layer (Domain)**
- Define entidades de negocio (POCOs)
- Define abstracciones y contratos
- Composite Pattern para permisos (Familia/Patente)
- NO contiene lógica de persistencia ni UI

**DAL Layer (Service/Impl & Service/Contracts)**
- Implementa acceso a datos via ADO.NET raw SQL
- Define interfaces (contratos) para repositorios
- Implementaciones específicas por motor (SqlServer)

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

**Objetivo**: Garantizar transaccionalidad en operaciones que involucran múltiples repositorios.

**Implementation Pattern**:
```csharp
using (var conn = new SqlConnection(_connectionString))
{
    conn.Open();
    using (var tran = conn.BeginTransaction())
    {
        try
        {
            // Multiple operations
            cmd1.Transaction = tran;
            cmd1.ExecuteNonQuery();
            
            cmd2.Transaction = tran;
            cmd2.ExecuteNonQuery();
            
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

**Cuándo aplicar**:
- Operaciones que modifican múltiples tablas relacionadas
- Cuando un rollback es crítico para mantener consistencia
- Ejemplos: Crear reserva (Reserva + Pago + Movimiento), Asignar membresía (Cliente + Pago + Balance)

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
```csharp
// Use parameterized queries ALWAYS
string query = "SELECT * FROM Cliente WHERE DNI = @DNI";
SqlParameter[] parameters = { new SqlParameter("@DNI", dni) };

// Table names match entity names
// Column names match property names (PascalCase)
```

### Transaction Management
```csharp
// For single operations: use ExecuteNonQuery from BaseRepository
ExecuteNonQuery("INSERT INTO...", parameters);

// For multi-table operations: explicit transaction
using (var conn = new SqlConnection(_connectionString))
{
    conn.Open();
    using (var tran = conn.BeginTransaction())
    {
        try
        {
            // operations...
            tran.Commit();
        }
        catch { tran.Rollback(); throw; }
    }
}
```

---

## Service Layer Standards

### Structure
```csharp
public class ClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IMembresiaRepository _membresiaRepository;
    private readonly BalanceService _balanceService;
    
    public ClienteService()
    {
        _clienteRepository = FactoryDao.ClienteRepository;
        _membresiaRepository = FactoryDao.MembresiaRepository;
        _balanceService = new BalanceService();
    }
    
    public void RegistrarCliente(ClienteDTO dto)
    {
        // 1. Validations
        ValidarDatos(dto);
        
        // 2. Business rules
        if (_clienteRepository.ExistsByDNI(dto.DNI))
            throw new Exception("Cliente ya registrado");
        
        // 3. Orchestrate repositories
        var cliente = MapToEntity(dto);
        _clienteRepository.Add(cliente);
        
        // 4. Side effects (balance, logs, etc.)
        _balanceService.InicializarBalance(cliente.Id);
    }
}
```

### Error Handling
```csharp
// Services throw exceptions with business-friendly messages
if (!ValidarMembresia(membresiaId))
    throw new Exception("La membresía no existe o está deshabilitada");

// UI catches and shows translated errors
try 
{
    _service.Operation();
}
catch (Exception ex)
{
    MessageBox.Show("MSG_ERROR_KEY".Translate() + ex.Message);
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
        UpdateLanguage(); // Always after InitializeComponent
    }
    
    private void FrmClientes_Load(object sender, EventArgs e)
    {
        ApplyPermissions(); // Check user permissions first
        LoadData();
    }
    
    private void ApplyPermissions()
    {
        btnCreate.Enabled = _currentUser.TienePermiso(PermisoKeys.ClienteCrear);
        // ...
    }
    
    private void UpdateLanguage()
    {
        this.Text = "CLIENTE_TITLE".Translate();
        btnCreate.Text = "BTN_CREATE".Translate();
    }
}
```

### Permission Checks
```csharp
// Always check permissions before allowing operations
if (!_currentUser.TienePermiso(PermisoKeys.ClienteListar))
{
    MessageBox.Show("MSG_NO_PERMISSION".Translate());
    this.Close();
    return;
}
```

---

## Translation System

### Structure
```
Languages/
  Language.es-MX
  Language.en-US
```

### Usage
```csharp
// Via extension method (preferred)
string text = "KEY_NAME".Translate();

// Via service
var langService = new LanguageService();
string text = langService.Translate("KEY_NAME");
```

### Key Naming Convention
```
// Format: CONTEXT_TYPE_DESCRIPTION
CLIENTE_TITLE=Gestión de Clientes
BTN_SAVE=Guardar
MSG_SUCCESS_CREATE=Cliente creado exitosamente
MSG_ERROR_NOT_FOUND=Cliente no encontrado
LBL_DNI=DNI
```

---

## Code Style

### Comments
```csharp
// English for technical comments
// Calculate monthly balance based on membership periodicity

// Spanish acceptable for complex business rules
// Según requerimiento 3.1.4: No se puede actualizar membresía si el cliente tiene deuda
```

### Formatting
- Follow existing code style (Allman braces, 4-space indent)
- Keep methods focused and short (< 30 lines ideal)
- Extract complex conditions to named methods

```csharp
// Good
if (ClienteTieneDeuda(clienteId))
    throw new Exception("Cliente tiene deuda pendiente");

// Avoid
if (_balanceService.GetBalance(clienteId).Saldo < 0)
    throw new Exception("Cliente tiene deuda pendiente");
```

### GUID Usage
```csharp
// Always use GUID for entity IDs (already implemented)
public Guid Id { get; set; } = Guid.NewGuid();

// Constructor initialization
public Cliente()
{
    Id = Guid.NewGuid();
}
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

### Indexes
```sql
-- For frequent queries
CREATE INDEX IX_Movimiento_ClienteID ON Movimiento(ClienteID);
CREATE INDEX IX_Reserva_Fecha ON Reserva(Fecha);
```

---

## Business Logic Patterns

### Immutable Audit Trail
```csharp
// Movimientos son INMUTABLES (según spec 2.1.4)
// Never UPDATE Movimiento, only INSERT
public void RegistrarMovimiento(Movimiento mov)
{
    // No validation to change existing
    _repository.Add(mov);
    ActualizarBalanceView(mov.ClienteID);
}
```

### Balance Calculation
```csharp
// Balance se calcula desde vista materializada o query agregada
// NO se almacena directamente, se deriva de Movimientos
public decimal GetBalance(Guid clienteId)
{
    return _repository.GetBalanceFromView(clienteId);
}
```

### State Machines
```csharp
// Pago: Abonado → Reembolsado
// Reserva: Pendiente → Pagada / Cancelada
public void CancelarReserva(Guid reservaId)
{
    var reserva = _repository.GetById(reservaId);
    
    if (reserva.Estado != "Pendiente" && reserva.Estado != "Pagada")
        throw new InvalidOperationException("Solo se pueden cancelar reservas Pendientes o Pagadas");
    
    reserva.Estado = "Cancelada";
    _repository.Update(reserva);
}
```

---

## Exception Handling Strategy

### Service Layer
```csharp
// Throw exceptions with clear business messages (Spanish OK)
throw new InvalidOperationException("El cliente tiene deuda pendiente");
throw new ArgumentException("La membresía no existe");
```

### Repository Layer
```csharp
// Let SQL exceptions bubble up, or wrap with context
catch (SqlException ex)
{
    throw new InvalidOperationException($"Error al guardar cliente: {ex.Message}", ex);
}
```

### UI Layer
```csharp
// Catch, translate, show to user
try
{
    _service.Operation();
    MessageBox.Show("MSG_SUCCESS".Translate(), "TITLE_SUCCESS".Translate());
}
catch (Exception ex)
{
    MessageBox.Show("MSG_ERROR".Translate() + ex.Message, "TITLE_ERROR".Translate());
}
```

---

## Critical Business Rules (from PDF)

### 1. Cliente debe estar sin deuda para:
- Hacer check-in (Req 3.1.5)
- Actualizar membresía (Req 3.1.4)
- Registrarse (Req 3.1.1)

### 2. Movimientos son inmutables
- Una vez creados, no se modifican
- Reembolsos generan NUEVO movimiento inverso
- Balance se recalcula desde vista/agregación

### 3. Auditoría obligatoria
- Toda operación registra en Bitácora vía CU.ARQ.001
- Usuario que ejecuta la operación
- Timestamp y detalles de la transacción

### 4. Reservas generan deuda automática
- Al crear reserva: Movimiento negativo por monto total
- Al pagar seña: Movimiento positivo parcial
- Al completar pago: Movimiento positivo restante

---

## File Organization

```
Solution/
├── Domain/                     # Entities + Abstractions
│   ├── Entities/              # POCOs
│   ├── Composite/             # Permissions (Familia/Patente)
│   └── Usuario.cs             # Base abstract class
│
├── Service/                    # BLL + DAL
│   ├── Contracts/             # Interfaces (IRepository, IService)
│   ├── DTO/                   # Data Transfer Objects
│   ├── Factory/               # FactoryDao (Singleton)
│   ├── Helpers/               # ConnectionManager, CryptographyHelper, etc.
│   ├── Impl/                  # Repository implementations
│   │   ├── SqlServer/        # SQL Server specific
│   │   ├── Text/             # Language files
│   │   └── Console/          # Console logger
│   └── Logic/                 # Business Logic Services
│
└── UI/                        # Windows Forms
    ├── Frm*.cs               # Form implementations
    └── Languages/            # Translation files (linked)
```
