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
                // Validaciones Técnicas
                if (dto.DNI <= 0) throw new ArgumentException("DNI debe ser mayor a cero");
                if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new ArgumentException("Nombre es requerido");
                if (string.IsNullOrWhiteSpace(dto.Apellido)) throw new ArgumentException("Apellido es requerido");
                if (dto.FechaNacimiento > DateTime.Now) throw new ArgumentException("Fecha de nacimiento no puede ser futura");

                // Validaciones de Negocio
                if (_repository.ExistsByDNI(dto.DNI))
                {
                    throw new InvalidOperationException($"Ya existe un cliente con DNI {dto.DNI}");
                }

                if (dto.MembresiaID.HasValue)
                {
                    var membresia = _membresiaService.ObtenerMembresia(dto.MembresiaID.Value);
                    if (membresia == null) throw new InvalidOperationException("La membresía seleccionada no existe");
                    if (!membresia.Activa) throw new InvalidOperationException("La membresía seleccionada no está activa");

                    _bitacora.Log($"CU-CLIE-01: Cliente DNI {dto.DNI} registrado con membresía {membresia.Nombre}", "INFO");
                }
                else
                {
                     _bitacora.Log($"CU-CLIE-01: Cliente DNI {dto.DNI} registrado sin membresía", "INFO");
                }

                var entity = ClienteMapper.ToEntity(dto);
                if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();
                entity.Activo = true;

                _repository.Add(entity);
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

                // Validar Deuda
                var balance = _balanceService.ConsultarBalance(clienteId);
                if (balance != null && balance.Saldo < 0)
                {
                     throw new InvalidOperationException($"Cliente tiene deuda de ${Math.Abs(balance.Saldo):N2}");
                }

                // Validar Membresia
                var membresia = _membresiaService.ObtenerMembresia(nuevaMembresiaId);
                if (membresia == null) throw new InvalidOperationException("La membresía no existe");
                if (!membresia.Activa) throw new InvalidOperationException("La membresía no está activa");

                _repository.AsignarMembresia(clienteId, nuevaMembresiaId, null, null);

                 _bitacora.Log($"Cliente {cliente.DNI} actualizó membresía a {membresia.Nombre}", "INFO");
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
                 var cliente = _repository.GetById(clienteId);
                 if (cliente == null) throw new InvalidOperationException("Cliente no encontrado");

                 cliente.Activo = false;
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

                 cliente.Activo = true;
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

                 if (!cliente.Activo)
                 {
                     _bitacora.Log($"CU-CLIE-04: Ingreso denegado para cliente DNI {dni} - Razón: Cliente deshabilitado", "WARNING");
                     return new ResultadoIngresoDTO
                     {
                         Permitido = false,
                         Razon = "Cliente deshabilitado",
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

            // Note: N+1 issue here is known but accepted for this architecture/scale.
            foreach (var c in clientes)
            {
                 var balance = _balanceService.ConsultarBalance(c.Id);
                 Membresia membresia = null;
                 if (c.MembresiaID.HasValue)
                 {
                     membresia = DalFactory.MembresiaRepository.GetById(c.MembresiaID.Value);
                 }
                 list.Add(ClienteMapper.ToDTO(c, membresia, balance));
            }
            return list;
        }

        public ClienteDTO ObtenerCliente(Guid id)
        {
             var cliente = _repository.GetById(id);
             if (cliente == null) return null;

             var balance = _balanceService.ConsultarBalance(cliente.Id);
             Membresia membresia = null;
             if (cliente.MembresiaID.HasValue)
             {
                 membresia = DalFactory.MembresiaRepository.GetById(cliente.MembresiaID.Value);
             }
             return ClienteMapper.ToDTO(cliente, membresia, balance);
        }
    }
}
