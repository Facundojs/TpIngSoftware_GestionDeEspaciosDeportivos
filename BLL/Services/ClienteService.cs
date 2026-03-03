using BLL.DTOs;
using BLL.Mappers;
using DAL.Contracts;
using DAL.Factory;
using Domain.Entities;
using Service.Logic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Services
{
    public class ClienteService
    {
        private readonly IClienteRepository _repository;
        private readonly BalanceService _balanceService;
        private readonly MembresiaService _membresiaService;
        private readonly BitacoraService _bitacora;

        public ClienteService()
        {
            _repository = DalFactory.ClienteRepository;
            _balanceService = new BalanceService();
            _membresiaService = new MembresiaService();
            _bitacora = new BitacoraService();
        }

        public void RegistrarCliente(ClienteDTO dto)
        {
            if (dto.DNI <= 0) throw new ArgumentException(Domain.Enums.Translations.ERR_INVALID_NUMBER.Translate());
            if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new ArgumentException(Domain.Enums.Translations.ERR_NOMBRE_REQUERIDO.Translate());
            if (string.IsNullOrWhiteSpace(dto.Apellido)) throw new ArgumentException(Domain.Enums.Translations.ERR_APELLIDO_REQUERIDO.Translate());
            if (dto.FechaNacimiento > DateTime.Now) throw new ArgumentException(Domain.Enums.Translations.ERR_FECHA_NAC_FUTURA.Translate());
            if (!string.IsNullOrWhiteSpace(dto.Email) && !dto.Email.Contains("@")) throw new ArgumentException(Domain.Enums.Translations.ERR_EMAIL_INVALIDO.Translate());

            if (_repository.ExistsByDNI(dto.DNI))
            {
                throw new InvalidOperationException(Domain.Enums.Translations.ERR_DNI_DUPLICADO_MSG.Translate());
            }

            using (var uow = DalFactory.CreateUnitOfWork())
            {
                try
                {
                    uow.BeginTransaction();

                    if (dto.MembresiaID.HasValue)
                    {
                        var membresia = uow.MembresiaRepository.GetById(dto.MembresiaID.Value);
                        if (membresia == null) throw new InvalidOperationException(Domain.Enums.Translations.ERR_MEMBRESIA_NO_EXISTE.Translate());
                        if (!membresia.Activa) throw new InvalidOperationException(Domain.Enums.Translations.ERR_MEMBRESIA_NO_ACTIVA.Translate());
                    }

                    var entity = ClienteMapper.ToEntity(dto);
                    if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();
                    entity.Estado = ClienteStatus.Activo.ToString();
                    entity.CreatedAt = DateTime.Now;

                    uow.ClienteRepository.Add(entity);

                    if (dto.MembresiaID.HasValue)
                    {
                        var membresia = uow.MembresiaRepository.GetById(dto.MembresiaID.Value);
                        var clienteMembresia = new ClienteMembresia
                        {
                            Id = Guid.NewGuid(),
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
                            Descripcion = $"Initial charge membership {membresia.Nombre}",
                            Fecha = DateTime.Now
                        };
                        uow.MovimientoRepository.Insertar(movimiento);
                    }

                    uow.Commit();
                    _bitacora.Log($"CU-CLIE-01: Client ID {entity.DNI} registered", "INFO");
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    _bitacora.Log($"Error in CU-CLIE-01: {ex.Message}", "ERROR", ex);
                    throw;
                }
            }
        }

        public void ActualizarMembresia(Guid clienteId, Guid nuevaMembresiaId)
        {
            var cliente = _repository.GetById(clienteId);
            if (cliente == null) throw new InvalidOperationException(Domain.Enums.Translations.ERR_CLIENTE_NO_ENCONTRADO.Translate());

            var balance = _balanceService.ConsultarBalance(clienteId);
            if (balance != null && balance.Saldo < 0)
            {
                throw new InvalidOperationException(Domain.Enums.Translations.ERR_CLIENTE_CON_DEUDA_MSG.Translate());
            }

            using (var uow = DalFactory.CreateUnitOfWork())
            {
                try
                {
                    uow.BeginTransaction();

                    var membresia = uow.MembresiaRepository.GetById(nuevaMembresiaId);
                    if (membresia == null) throw new InvalidOperationException(Domain.Enums.Translations.ERR_MEMBRESIA_NO_EXISTE.Translate());
                    if (!membresia.Activa) throw new InvalidOperationException(Domain.Enums.Translations.ERR_MEMBRESIA_NO_ACTIVA.Translate());

                    var activeMembresia = uow.ClienteMembresiaRepository.GetActiveByClienteId(clienteId);
                    if (activeMembresia != null)
                    {
                        activeMembresia.FechaBaja = DateTime.Now;
                        uow.ClienteMembresiaRepository.Update(activeMembresia);
                    }

                    var newMembresia = new ClienteMembresia
                    {
                        Id = Guid.NewGuid(),
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
                        Descripcion = $"Membership Update - {membresia.Nombre}",
                        Fecha = DateTime.Now
                    };
                    uow.MovimientoRepository.Insertar(movimiento);

                    uow.ClienteRepository.AsignarMembresia(clienteId, nuevaMembresiaId);

                    uow.Commit();
                    _bitacora.Log($"Client {cliente.DNI} updated membership to {membresia.Nombre}", "INFO");
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    _bitacora.Log($"Error updating membership: {ex.Message}", "ERROR", ex);
                    throw;
                }
            }
        }

        public void DeshabilitarCliente(Guid clienteId, string razon)
        {
            if (string.IsNullOrWhiteSpace(razon)) throw new ArgumentException(Domain.Enums.Translations.ERR_RAZON_REQUERIDA.Translate());

            var cliente = _repository.GetById(clienteId);
            if (cliente == null) throw new InvalidOperationException(Domain.Enums.Translations.ERR_CLIENTE_NO_ENCONTRADO.Translate());

            cliente.Estado = ClienteStatus.Inactivo.ToString();
            cliente.Razon = razon;
            _repository.Update(cliente);

            _bitacora.Log($"CU-CLIE-02: Client DNI {cliente.DNI} disabled - Reason: {razon}", "INFO");
        }

        public void HabilitarCliente(Guid clienteId)
        {
            var cliente = _repository.GetById(clienteId);
            if (cliente == null) throw new InvalidOperationException(Domain.Enums.Translations.ERR_CLIENTE_NO_ENCONTRADO.Translate());

            cliente.Estado = ClienteStatus.Activo.ToString();
            cliente.Razon = null;
            _repository.Update(cliente);

            _bitacora.Log($"CU-CLIE-03: Client DNI {cliente.DNI} enabled", "INFO");
        }

        public ResultadoIngresoDTO ValidarIngreso(int dni)
        {
            var cliente = _repository.GetByDNI(dni);
            if (cliente == null)
            {
                _bitacora.Log($"CU-CLIE-04: Access denied for ID {dni} - Reason: Client not found", "WARNING");
                return new ResultadoIngresoDTO { Permitido = false, Razon = "Client not found" };
            }

            if (cliente.Estado != ClienteStatus.Activo.ToString())
            {
                string razonMsg = !string.IsNullOrEmpty(cliente.Razon) ? $"Client disabled - Reason: {cliente.Razon}" : "Client disabled";
                _bitacora.Log($"CU-CLIE-04: Access denied for client DNI {dni} - Reason: {razonMsg}", "WARNING");
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
                string msg = $"Client owes ${Math.Abs(balance.Saldo):N2}";
                _bitacora.Log($"CU-CLIE-04: Access denied for client DNI {dni} - Reason: {msg}", "WARNING");
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

            _bitacora.Log($"CU-CLIE-04: Access authorized for client ID {dni}", "INFO");
            return new ResultadoIngresoDTO
            {
                Permitido = true,
                NombreCliente = $"{cliente.Nombre} {cliente.Apellido}",
                Balance = balance?.Saldo ?? 0
            };
        }

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
