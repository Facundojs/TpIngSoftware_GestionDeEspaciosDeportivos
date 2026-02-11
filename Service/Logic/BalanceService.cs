using DAL.Factory;
using Domain.Entities;
using Service.Logic;
using System;
using System.Linq;

namespace Service.Logic
{
    public class BalanceService
    {
        private readonly BitacoraService _bitacoraService;

        public BalanceService()
        {
            _bitacoraService = new BitacoraService();
        }

        public void CalcularSaldoMensual()
        {
            _bitacoraService.Log("Inicio de Job CalcularSaldoMensual", "INFO");
            int processedCount = 0;

            try
            {
                var clientes = DalFactory.ClienteRepository.ListarTodos();

                foreach (var cliente in clientes)
                {
                    try
                    {
                        if (!cliente.MembresiaID.HasValue)
                            continue;

                        var membresia = DalFactory.MembresiaRepository.GetById(cliente.MembresiaID.Value);

                        if (membresia == null)
                        {
                            _bitacoraService.Log($"Membresia {cliente.MembresiaID} no encontrada para cliente {cliente.Id}", "WARNING");
                            continue;
                        }

                        if (!membresia.Activa)
                            continue;

                        if (membresia.Precio == 0)
                        {
                            _bitacoraService.Log($"Membresia {membresia.Id} tiene precio 0, omitiendo cargo.", "WARNING");
                            continue;
                        }

                        var movimiento = new Movimiento
                        {
                            Id = Guid.NewGuid(),
                            ClienteID = cliente.Id,
                            Monto = -membresia.Precio, // Negative for debt
                            Tipo = "DeudaMembresia",
                            Descripcion = $"Cargo mensual membresía {membresia.Nombre}",
                            Fecha = DateTime.Now
                        };

                        DalFactory.MovimientoRepository.Insertar(movimiento);
                        ActualizarBalance(cliente.Id);
                        processedCount++;
                    }
                    catch (Exception ex)
                    {
                        _bitacoraService.Log($"Error procesando cliente {cliente.Id}: {ex.Message}", "ERROR", ex);
                        // Continue with next client
                    }
                }

                _bitacoraService.Log($"Job CalcularSaldoMensual finalizado. Clientes procesados: {processedCount}", "INFO");
            }
            catch (Exception ex)
            {
                _bitacoraService.Log($"Error general en CalcularSaldoMensual: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        private void ActualizarBalance(Guid clienteId)
        {
            // En esta arquitectura, el balance es una vista calculada (vw_Balance).
            // No se requiere actualización explícita de una tabla de saldos.
            // Este método sirve como punto de extensión o notificación.
            _bitacoraService.Log($"Balance actualizado para el cliente {clienteId}", "INFO");
        }
    }
}
