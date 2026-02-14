using DAL.Factory;
using Domain.Entities;
using Service.Logic;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BLL
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
            int skippedCount = 0;
            int errorCount = 0;

            try
            {
                // Retrieve all clients
                var clientes = DalFactory.ClienteRepository.GetAll();

                // Ensure repositories are initialized before parallel execution to avoid race conditions in Factory
                var membresiaRepo = DalFactory.MembresiaRepository;
                var movimientoRepo = DalFactory.MovimientoRepository;

                Parallel.ForEach(clientes, (cliente) =>
                {
                    try
                    {
                        if (!cliente.MembresiaID.HasValue)
                        {
                            Interlocked.Increment(ref skippedCount);
                            return;
                        }

                        var membresia = membresiaRepo.GetById(cliente.MembresiaID.Value);

                        if (membresia == null)
                        {
                            _bitacoraService.Log($"Membresia {cliente.MembresiaID} no encontrada para cliente {cliente.Id}", "WARNING");
                            Interlocked.Increment(ref errorCount);
                            return;
                        }

                        if (!membresia.Activa)
                        {
                            Interlocked.Increment(ref skippedCount);
                            return;
                        }

                        if (membresia.Precio == 0)
                        {
                            _bitacoraService.Log($"Membresia {membresia.Id} tiene precio 0, omitiendo cargo.", "WARNING");
                            Interlocked.Increment(ref skippedCount);
                            return;
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

                        movimientoRepo.Insertar(movimiento);
                        ActualizarBalance(cliente.Id);
                        Interlocked.Increment(ref processedCount);
                    }
                    catch (Exception ex)
                    {
                        _bitacoraService.Log($"Error procesando cliente {cliente.Id}: {ex.Message}", "ERROR", ex);
                        Interlocked.Increment(ref errorCount);
                    }
                });

                _bitacoraService.Log($"Job CalcularSaldoMensual finalizado. Procesados: {processedCount}, Omitidos: {skippedCount}, Errores: {errorCount}", "INFO");
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

        public Balance ConsultarBalance(Guid clienteId)
        {
            return DalFactory.BalanceRepository.ObtenerBalance(clienteId);
        }

        public void ValidarDeuda(Guid clienteId, string contexto = null)
        {
            var balance = ConsultarBalance(clienteId);

            if (balance != null && balance.Saldo < 0)
            {
                string msg = $"El cliente tiene una deuda de ${Math.Abs(balance.Saldo):N2}. Debe saldar antes de continuar.";
                if (!string.IsNullOrEmpty(contexto))
                {
                    msg = $"{contexto}: {msg}";
                }
                throw new InvalidOperationException(msg);
            }
        }
    }
}
