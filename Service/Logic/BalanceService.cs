using DAL.Factory;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Logic
{
    public class BalanceService
    {
        public void CalcularSaldoMensual()
        {
            var clienteRepo = DalFactory.ClienteRepository;
            var membresiaRepo = DalFactory.MembresiaRepository;
            var movimientoRepo = DalFactory.MovimientoRepository;

            var clientes = clienteRepo.ListarTodos();

            foreach (var cliente in clientes)
            {
                try
                {
                    if (cliente.MembresiaID.HasValue)
                    {
                        var membresia = membresiaRepo.GetById(cliente.MembresiaID.Value);

                        if (membresia != null && membresia.Activa && membresia.Precio > 0)
                        {
                            var now = DateTime.Now;
                            // Check if debt exists for this month/year
                            var existingDebt = movimientoRepo.GetByClienteAndMonth(cliente.Id, now.Month, now.Year, "DeudaMembresia");

                            if (existingDebt == null)
                            {
                                var movimiento = new Movimiento
                                {
                                    Id = Guid.NewGuid(),
                                    ClienteID = cliente.Id,
                                    Tipo = "DeudaMembresia",
                                    Monto = -membresia.Precio,
                                    Descripcion = $"Cuota Mensual - {now:MM/yyyy}",
                                    Fecha = now,
                                    PagoID = null
                                };
                                movimientoRepo.Insert(movimiento);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log error for this client and continue
                    System.Diagnostics.Debug.WriteLine($"Error creating monthly debt for client {cliente.Id}: {ex.Message}");
                }
            }
        }
    }
}
