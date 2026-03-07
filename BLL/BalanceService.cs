using DAL.Factory;
using Domain.Entities;
using Domain.Enums;
using Service.Facade.Extension;
using Service.Logic;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BLL
{
    /// <summary>
    /// Manages client account balances: periodic membership billing, balance enquiry,
    /// debt validation, and movement history.
    /// </summary>
    public class BalanceService
    {
        private readonly BitacoraService _bitacoraService;

        /// <summary>Initializes the service with a new <see cref="BitacoraService"/> instance.</summary>
        public BalanceService()
        {
            _bitacoraService = new BitacoraService();
        }

        /// <summary>
        /// Iterates over all clients in parallel and inserts a <c>DeudaMembresia</c> movement for
        /// those whose <c>ProximaFechaPago</c> has passed. Advances <c>ProximaFechaPago</c> by
        /// the membership's <c>Regularidad</c> days after each charge.
        /// Clients without a membership, with an inactive or free membership, or whose next
        /// payment date is still in the future are skipped silently.
        /// </summary>
        public void CalcularSaldoMensual()
        {
            _bitacoraService.Log("Starting CalcularSaldoMensual Job", "INFO");
            int processedCount = 0;
            int skippedCount = 0;
            int errorCount = 0;

            try
            {
                var clientes = DalFactory.ClienteRepository.GetAll();
                var membresiaRepo = DalFactory.MembresiaRepository;
                var movimientoRepo = DalFactory.MovimientoRepository;
                var clienteMembresiaRepo = DalFactory.ClienteMembresiaRepository;

                Parallel.ForEach(clientes, (cliente) =>
                {
                    try
                    {
                        if (!cliente.MembresiaID.HasValue)
                        {
                            Interlocked.Increment(ref skippedCount);
                            return;
                        }

                        var activeMembresia = clienteMembresiaRepo.GetActiveByClienteId(cliente.Id);
                        if (activeMembresia == null || !activeMembresia.ProximaFechaPago.HasValue || activeMembresia.ProximaFechaPago.Value > DateTime.Now)
                        {
                            Interlocked.Increment(ref skippedCount);
                            return;
                        }

                        var membresia = membresiaRepo.GetById(cliente.MembresiaID.Value);

                        if (membresia == null)
                        {
                            _bitacoraService.Log($"Membership {cliente.MembresiaID} not found for client {cliente.Id}", "WARNING");
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
                            _bitacoraService.Log($"Membership {membresia.Id} has price 0, skipping charge.", "WARNING");
                            Interlocked.Increment(ref skippedCount);
                            return;
                        }

                        var movimiento = new Movimiento
                        {
                            Id = Guid.NewGuid(),
                            ClienteID = cliente.Id,
                            Monto = -membresia.Precio,
                            Tipo = TipoMovimiento.DeudaMembresia,
                            Descripcion = $"Monthly charge for membership {membresia.Nombre}",
                            Fecha = DateTime.Now
                        };

                        movimientoRepo.Insertar(movimiento);
                        ActualizarBalance(cliente.Id);

                        activeMembresia.ProximaFechaPago = activeMembresia.ProximaFechaPago.Value.AddDays(membresia.Regularidad);
                        clienteMembresiaRepo.Update(activeMembresia);

                        Interlocked.Increment(ref processedCount);
                    }
                    catch (Exception ex)
                    {
                        _bitacoraService.Log($"Error processing client {cliente.Id}: {ex.Message}", "ERROR", ex);
                        Interlocked.Increment(ref errorCount);
                    }
                });

                _bitacoraService.Log($"CalcularSaldoMensual Job finished. Processed: {processedCount}, Skipped: {skippedCount}, Errors: {errorCount}", "INFO");
            }
            catch (Exception ex)
            {
                _bitacoraService.Log($"General error in CalcularSaldoMensual: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        private void ActualizarBalance(Guid clienteId)
        {
            // Balance is a calculated view (vw_Balance) in SQL.
            _bitacoraService.Log($"Balance updated for client {clienteId}", "INFO");
        }

        /// <summary>
        /// Retrieves the current balance for a client from the <c>vw_Balance</c> SQL view.
        /// </summary>
        /// <param name="clienteId">The client identifier.</param>
        /// <returns>The <see cref="Balance"/> record, or <c>null</c> if no movements exist.</returns>
        public Balance ConsultarBalance(Guid clienteId)
        {
            return DalFactory.BalanceRepository.ObtenerBalance(clienteId);
        }

        /// <summary>
        /// Throws if the client's balance is negative, optionally prefixing the message with a context label.
        /// Used as a guard in operations that require the client to be current on payments.
        /// </summary>
        /// <param name="clienteId">The client to validate.</param>
        /// <param name="contexto">Optional operation name prepended to the error message for clarity.</param>
        /// <exception cref="InvalidOperationException">Thrown when the client has an outstanding debt.</exception>
        public void ValidarDeuda(Guid clienteId, string contexto = null)
        {
            var balance = ConsultarBalance(clienteId);

            if (balance != null && balance.Saldo < 0)
            {
                string msg = "ERR_CLIENTE_DEUDA_BLOQUEANTE".Translate();
                if (!string.IsNullOrEmpty(contexto))
                {
                    msg = $"{contexto}: {msg}";
                }
                throw new InvalidOperationException(msg);
            }
        }

        /// <summary>
        /// Returns the ledger movement history for a client, optionally filtered by date range.
        /// </summary>
        /// <param name="clienteId">The client whose movements to fetch.</param>
        /// <param name="desde">Optional inclusive lower bound on movement date.</param>
        /// <param name="hasta">Optional inclusive upper bound on movement date.</param>
        public System.Collections.Generic.List<Movimiento> ListarMovimientos(Guid clienteId, DateTime? desde, DateTime? hasta)
        {
            return DalFactory.BalanceRepository.ListarMovimientos(clienteId, desde, hasta);
        }
    }
}
