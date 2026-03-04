using BLL.DTOs;
using BLL.Mappers;
using DAL.Contracts;
using DAL.Factory;
using Domain.Entities;
using Domain.Enums;
using Service.Facade.Extension;
using Service.Logic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Services
{
    /// <summary>
    /// Business logic service for client lifecycle management.
    /// </summary>
    /// <remarks>
    /// Coordinates client registration, membership changes, status transitions, and facility check-in.
    /// Multi-step write operations are executed inside <see cref="DAL.Contracts.IUnitOfWork"/> transactions.
    /// All operations are audited via <see cref="BitacoraService"/>.
    /// </remarks>
    public class ClienteService
    {
        private readonly IClienteRepository _repository;
        private readonly BalanceService _balanceService;
        private readonly MembresiaService _membresiaService;
        private readonly BitacoraService _bitacora;

        /// <summary>Initializes all dependencies from <see cref="DAL.Factory.DalFactory"/> singletons.</summary>
        public ClienteService()
        {
            _repository = DalFactory.ClienteRepository;
            _balanceService = new BalanceService();
            _membresiaService = new MembresiaService();
            _bitacora = new BitacoraService();
        }

        /// <summary>
        /// Registers a new client. If a membership is specified, creates the client-membership
        /// record and inserts an initial <c>DeudaMembresia</c> movement in the same transaction.
        /// </summary>
        /// <param name="dto">Client data including optional <see cref="ClienteDTO.MembresiaID"/>.</param>
        /// <exception cref="ArgumentException">Thrown for invalid field values (DNI, name, email format, future birth date).</exception>
        /// <exception cref="InvalidOperationException">Thrown when the DNI is already registered or the membership is inactive.</exception>
        /// <summary>
        /// Registers a new client. If a membership is provided, it creates the <c>ClienteMembresia</c>
        /// link and an initial <c>DeudaMembresia</c> movement, all within a single transaction.
        /// </summary>
        /// <param name="dto">Client data. DNI must be unique, date of birth must be in the past.</param>
        /// <exception cref="ArgumentException">Thrown for invalid field values.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the DNI already exists or the membership is inactive.</exception>
        public void RegistrarCliente(ClienteDTO dto)
        {
            try
            {
                if (dto.DNI <= 0) throw new ArgumentException(Translations.ERR_INVALID_NUMBER.Translate());
                if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new ArgumentException(Translations.ERR_NOMBRE_REQUERIDO.Translate());
                if (string.IsNullOrWhiteSpace(dto.Apellido)) throw new ArgumentException(Translations.ERR_APELLIDO_REQUERIDO.Translate());
                if (dto.FechaNacimiento > DateTime.Now) throw new ArgumentException(Translations.ERR_FECHA_NAC_FUTURA.Translate());
                if (!string.IsNullOrWhiteSpace(dto.Email) && !dto.Email.Contains("@")) throw new ArgumentException(Translations.ERR_EMAIL_INVALIDO.Translate());

                if (_repository.ExistsByDNI(dto.DNI))
                {
                    throw new InvalidOperationException(Translations.ERR_DNI_DUPLICADO_MSG.Translate());
                }

                using (var uow = DalFactory.CreateUnitOfWork())
                {
                    try
                    {
                        uow.BeginTransaction();

                        if (dto.MembresiaID.HasValue)
                        {
                            var membresia = _membresiaService.ObtenerMembresia(dto.MembresiaID.Value);
                            if (membresia == null) throw new InvalidOperationException(Translations.ERR_MEMBRESIA_NO_EXISTE.Translate());
                            if (!membresia.Activa) throw new InvalidOperationException(Translations.ERR_MEMBRESIA_NO_ACTIVA.Translate());

                            _bitacora.Log($"CU-CLIE-01: Cliente DNI {dto.DNI} registrado con membresía {membresia.Nombre}, sin deuda. Próximo cobro: {DateTime.Now.AddDays(membresia.Regularidad):dd/MM/yyyy}", "INFO");
                        }
                        else
                        {
                             _bitacora.Log($"CU-CLIE-01: Cliente DNI {dto.DNI} registrado sin membresía", "INFO");
                        }

                        var entity = ClienteMapper.ToEntity(dto);
                        if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();
                        entity.Estado = ClienteStatus.Activo.ToString();
                        entity.CreatedAt = DateTime.Now;

                        uow.ClienteRepository.Add(entity);

                        if (dto.MembresiaID.HasValue)
                        {
                            var membresia = _membresiaService.ObtenerMembresia(dto.MembresiaID.Value);
                            var clienteMembresia = new ClienteMembresia
                            {
                                ClienteID = entity.Id,
                                MembresiaID = dto.MembresiaID.Value,
                                FechaAsignacion = DateTime.Now,
                                ProximaFechaPago = DateTime.Now.AddDays(membresia.Regularidad)
                            };
                            uow.ClienteMembresiaRepository.Add(clienteMembresia);

                            var movimiento = new Movimiento
                            {
                                Id = Guid.NewGuid(),
                                ClienteID = entity.Id,
                                Monto = -membresia.Precio,
                                Tipo = Domain.Enums.TipoMovimiento.DeudaMembresia,
                                Descripcion = $"Cargo inicial membresía {membresia.Nombre}",
                                Fecha = DateTime.Now
                            };
                            uow.MovimientoRepository.Insertar(movimiento);
                        }

                        uow.Commit();
                    }
                    catch
                    {
                        uow.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error en CU-CLIE-01: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        /// <summary>
        /// Switches a client's active membership to a new plan within a transaction.
        /// Closes the current <c>ClienteMembresia</c> record, creates a new one, and inserts
        /// a <c>DeudaMembresia</c> movement for the new plan's price.
        /// </summary>
        /// <param name="clienteId">The client to update.</param>
        /// <param name="nuevaMembresiaId">The new membership plan to assign.</param>
        /// <exception cref="InvalidOperationException">Thrown when the client has an outstanding negative balance.</exception>
        /// <summary>
        /// Switches a client to a different membership plan.
        /// Closes the current <c>ClienteMembresia</c> record, opens a new one, and inserts a
        /// <c>DeudaMembresia</c> movement for the new plan — all within a single transaction.
        /// Blocked if the client has an outstanding debt.
        /// </summary>
        /// <param name="clienteId">The client to update.</param>
        /// <param name="nuevaMembresiaId">The new membership plan identifier.</param>
        /// <exception cref="InvalidOperationException">Thrown if the client has a negative balance, or the membership is inactive.</exception>
        public void ActualizarMembresia(Guid clienteId, Guid nuevaMembresiaId)
        {
            try
            {
                var cliente = _repository.GetById(clienteId);
                if (cliente == null) throw new InvalidOperationException("Cliente no encontrado");

                var balance = _balanceService.ConsultarBalance(clienteId);
                if (balance != null && balance.Saldo < 0)
                {
                     throw new InvalidOperationException($"Cliente tiene deuda de ${Math.Abs(balance.Saldo):N2}");
                }

                string membresiaNombre = null;
                using (var uow = DalFactory.CreateUnitOfWork())
                {
                    try
                    {
                        uow.BeginTransaction();

                        var membresia = _membresiaService.ObtenerMembresia(nuevaMembresiaId);
                        if (membresia == null) throw new InvalidOperationException("La membresía no existe");
                        if (!membresia.Activa) throw new InvalidOperationException("La membresía no está activa");

                        membresiaNombre = membresia.Nombre;

                        var activeMembresia = uow.ClienteMembresiaRepository.GetActiveByClienteId(clienteId);
                        if (activeMembresia != null)
                        {
                            activeMembresia.FechaBaja = DateTime.Now;
                            uow.ClienteMembresiaRepository.Update(activeMembresia);
                        }

                        var newMembresia = new ClienteMembresia
                        {
                            ClienteID = clienteId,
                            MembresiaID = nuevaMembresiaId,
                            FechaAsignacion = DateTime.Now,
                            ProximaFechaPago = DateTime.Now.AddDays(membresia.Regularidad)
                        };
                        uow.ClienteMembresiaRepository.Add(newMembresia);

                        var movimiento = new Movimiento
                        {
                            Id = Guid.NewGuid(),
                            ClienteID = clienteId,
                            Monto = -membresia.Precio,
                            Tipo = Domain.Enums.TipoMovimiento.DeudaMembresia,
                            Descripcion = $"Actualización de Membresía - {membresia.Nombre}",
                            Fecha = DateTime.Now
                        };
                        uow.MovimientoRepository.Insertar(movimiento);

                        uow.ClienteRepository.AsignarMembresia(clienteId, nuevaMembresiaId);

                        uow.Commit();
                    }
                    catch
                    {
                        uow.Rollback();
                        throw;
                    }
                }

                 _bitacora.Log($"Cliente {cliente.DNI} actualizó membresía a {membresiaNombre}", "INFO");
            }
            catch (Exception ex)
            {
                 _bitacora.Log($"Error al actualizar membresía: {ex.Message}", "ERROR", ex);
                 throw;
            }
        }

        /// <summary>
        /// Sets the client status to <c>Inactivo</c> and records the reason.
        /// </summary>
        /// <param name="clienteId">The client to disable.</param>
        /// <param name="razon">Mandatory reason for disabling; must not be null or whitespace.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="razon"/> is empty.</exception>
        /// <summary>
        /// Sets a client's status to <c>Inactivo</c> and records the reason.
        /// </summary>
        /// <param name="clienteId">The client to disable.</param>
        /// <param name="razon">Mandatory non-empty reason for the disabling.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="razon"/> is empty.</exception>
        public void DeshabilitarCliente(Guid clienteId, string razon)
        {
             try
             {
                 if (string.IsNullOrWhiteSpace(razon)) throw new ArgumentException("La razón de deshabilitación es obligatoria");

                 var cliente = _repository.GetById(clienteId);
                 if (cliente == null) throw new InvalidOperationException("Cliente no encontrado");

                 cliente.Estado = ClienteStatus.Inactivo.ToString();
                 cliente.Razon = razon;
                 _repository.Update(cliente);

                 _bitacora.Log($"CU-CLIE-02: Cliente DNI {cliente.DNI} deshabilitado - Razón: {razon}", "INFO");
             }
             catch (Exception ex)
             {
                 _bitacora.Log($"Error en CU-CLIE-02: {ex.Message}", "ERROR", ex);
                 throw;
             }
        }

        /// <summary>
        /// Sets the client status to <c>Activo</c> and clears the previously recorded reason.
        /// </summary>
        /// <param name="clienteId">The client to re-enable.</param>
        /// <summary>Sets a client's status back to <c>Activo</c> and clears the disabling reason.</summary>
        /// <param name="clienteId">The client to re-enable.</param>
        public void HabilitarCliente(Guid clienteId)
        {
             try
             {
                 var cliente = _repository.GetById(clienteId);
                 if (cliente == null) throw new InvalidOperationException("Cliente no encontrado");

                 cliente.Estado = ClienteStatus.Activo.ToString();
                 cliente.Razon = null;
                 _repository.Update(cliente);

                 _bitacora.Log($"CU-CLIE-03: Cliente DNI {cliente.DNI} habilitado", "INFO");
             }
             catch (Exception ex)
             {
                 _bitacora.Log($"Error en CU-CLIE-03: {ex.Message}", "ERROR", ex);
                 throw;
             }
        }

        /// <summary>
        /// Validates whether a client may enter the facility and, if so, records the check-in.
        /// Access is denied when the client is inactive or has a negative balance.
        /// </summary>
        /// <param name="dni">The client's DNI to look up.</param>
        /// <returns>
        /// A <see cref="ResultadoIngresoDTO"/> with <c>Permitido = true</c> on success, or
        /// <c>Permitido = false</c> with a reason string when access is denied.
        /// </returns>
        /// <summary>
        /// Validates whether a client identified by <paramref name="dni"/> may enter the facility.
        /// If access is granted, persists an <see cref="Domain.Entities.Ingreso"/> record.
        /// Denies access if the client is not found, is inactive, or has a negative balance.
        /// </summary>
        /// <param name="dni">The client's national identity number.</param>
        /// <returns>
        /// A <see cref="ResultadoIngresoDTO"/> with <c>Permitido = true</c> on success,
        /// or <c>Permitido = false</c> with a human-readable <c>Razon</c> on denial.
        /// </returns>
        public ResultadoIngresoDTO ValidarIngreso(int dni)
        {
             try
             {
                 var cliente = _repository.GetByDNI(dni);
                 if (cliente == null)
                 {
                     _bitacora.Log($"CU-CLIE-04: Ingreso denegado para DNI {dni} - Razón: Cliente no encontrado", "WARNING");
                     return new ResultadoIngresoDTO { Permitido = false, Razon = "Cliente no encontrado" };
                 }

                 if (cliente.Estado != ClienteStatus.Activo.ToString())
                 {
                     string razonMsg = !string.IsNullOrEmpty(cliente.Razon) ? $"Cliente deshabilitado - Razón: {cliente.Razon}" : "Cliente deshabilitado";
                     _bitacora.Log($"CU-CLIE-04: Ingreso denegado para cliente DNI {dni} - Razón: {razonMsg}", "WARNING");
                     return new ResultadoIngresoDTO
                     {
                         Permitido = false,
                         Razon = razonMsg,
                         NombreCliente = $"{cliente.Nombre} {cliente.Apellido}"
                     };
                 }

                 var balance = _balanceService.ConsultarBalance(cliente.Id);
                 if (balance != null && balance.Saldo < 0)
                 {
                     string msg = $"Cliente debe ${Math.Abs(balance.Saldo):N2}";
                     _bitacora.Log($"CU-CLIE-04: Ingreso denegado para cliente DNI {dni} - Razón: {msg}", "WARNING");
                     return new ResultadoIngresoDTO
                     {
                         Permitido = false,
                         Razon = msg,
                         NombreCliente = $"{cliente.Nombre} {cliente.Apellido}",
                         Balance = balance.Saldo
                     };
                 }

                 var ingreso = new Ingreso
                 {
                     Id = Guid.NewGuid(),
                     ClienteID = cliente.Id,
                     FechaHora = DateTime.Now
                 };
                 DalFactory.IngresoRepository.Add(ingreso);

                 _bitacora.Log($"CU-CLIE-04: Ingreso autorizado para cliente DNI {dni}", "INFO");
                 return new ResultadoIngresoDTO
                 {
                     Permitido = true,
                     NombreCliente = $"{cliente.Nombre} {cliente.Apellido}",
                     Balance = balance?.Saldo ?? 0
                 };
             }
             catch (Exception ex)
             {
                 _bitacora.Log($"Error en CU-CLIE-04: {ex.Message}", "ERROR", ex);
                 throw;
             }
        }

        /// <summary>
        /// Returns all clients with their current balance, membership detail, and next payment date hydrated.
        /// </summary>
        /// <summary>
        /// Returns all clients with their balance, membership details, and next payment date hydrated.
        /// </summary>
        public List<ClienteDTO> ListarClientes()
        {
            var clientes = _repository.GetAll();
            var list = new List<ClienteDTO>();

            foreach (var c in clientes)
            {
                 var balance = _balanceService.ConsultarBalance(c.Id);
                 Membresia membresia = null;
                 DateTime? proximaFechaPago = null;
                 if (c.MembresiaID.HasValue)
                 {
                     membresia = DalFactory.MembresiaRepository.GetById(c.MembresiaID.Value);
                     var clMembresia = DalFactory.ClienteMembresiaRepository.GetActiveByClienteId(c.Id);
                     if (clMembresia != null)
                     {
                         proximaFechaPago = clMembresia.ProximaFechaPago;
                     }
                 }
                 list.Add(ClienteMapper.ToDTO(c, membresia, balance, proximaFechaPago));
            }
            return list;
        }

        /// <summary>
        /// Returns a single client by primary key with balance and membership detail hydrated.
        /// </summary>
        /// <param name="id">The client's unique identifier.</param>
        /// <returns>The hydrated <see cref="ClienteDTO"/>, or <c>null</c> if not found.</returns>
        /// <summary>
        /// Retrieves a single client by identifier with balance, membership, and next payment date hydrated.
        /// </summary>
        /// <returns>The hydrated <see cref="ClienteDTO"/>, or <c>null</c> if not found.</returns>
        public ClienteDTO ObtenerCliente(Guid id)
        {
             var cliente = _repository.GetById(id);
             if (cliente == null) return null;

             var balance = _balanceService.ConsultarBalance(cliente.Id);
             Membresia membresia = null;
             DateTime? proximaFechaPago = null;
             if (cliente.MembresiaID.HasValue)
             {
                 membresia = DalFactory.MembresiaRepository.GetById(cliente.MembresiaID.Value);
                 var clMembresia = DalFactory.ClienteMembresiaRepository.GetActiveByClienteId(cliente.Id);
                 if (clMembresia != null)
                 {
                     proximaFechaPago = clMembresia.ProximaFechaPago;
                 }
             }
             return ClienteMapper.ToDTO(cliente, membresia, balance, proximaFechaPago);
        }

        /// <summary>
        /// Returns a single client by DNI with balance and membership detail hydrated.
        /// </summary>
        /// <param name="dni">The client's national identity number.</param>
        /// <returns>The hydrated <see cref="ClienteDTO"/>, or <c>null</c> if not found.</returns>
        /// <summary>Retrieves a client by DNI with balance, membership, and next payment date hydrated.</summary>
        /// <returns>The hydrated <see cref="ClienteDTO"/>, or <c>null</c> if not found.</returns>
        public ClienteDTO ObtenerClientePorDNI(int dni)
        {
             var cliente = _repository.GetByDNI(dni);
             if (cliente == null) return null;

             var balance = _balanceService.ConsultarBalance(cliente.Id);
             Membresia membresia = null;
             DateTime? proximaFechaPago = null;
             if (cliente.MembresiaID.HasValue)
             {
                 membresia = DalFactory.MembresiaRepository.GetById(cliente.MembresiaID.Value);
                 var clMembresia = DalFactory.ClienteMembresiaRepository.GetActiveByClienteId(cliente.Id);
                 if (clMembresia != null)
                 {
                     proximaFechaPago = clMembresia.ProximaFechaPago;
                 }
             }
             return ClienteMapper.ToDTO(cliente, membresia, balance, proximaFechaPago);
        }
    }
}
