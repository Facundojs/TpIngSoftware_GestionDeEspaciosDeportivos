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
            try
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
                            var membresia = _membresiaService.ObtenerMembresia(dto.MembresiaID.Value);
                            if (membresia == null) throw new InvalidOperationException(Domain.Enums.Translations.ERR_MEMBRESIA_NO_EXISTE.Translate());
                            if (!membresia.Activa) throw new InvalidOperationException(Domain.Enums.Translations.ERR_MEMBRESIA_NO_ACTIVA.Translate());

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
