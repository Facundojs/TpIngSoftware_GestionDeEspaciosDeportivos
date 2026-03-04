using System;
using System.Collections.Generic;
using System.Linq;
using BLL.DTOs;
using DAL.Factory;
using Domain.Entities;

namespace BLL.Services
{
    /// <summary>
    /// Read-only service for querying facility check-in (ingreso) history.
    /// </summary>
    /// <remarks>
    /// Check-in records are written by <see cref="ClienteService.ValidarIngreso"/>.
    /// This service only provides listing functionality.
    /// </remarks>
    public class IngresoService
    {
        /// <summary>
        /// Returns all check-in records sorted by most recent first, with <c>ClienteNombre</c> hydrated.
        /// </summary>
        /// <returns>List of all check-in records; empty list if none.</returns>
        public List<IngresoDTO> ListarIngresos()
        {
            var ingresos = DalFactory.IngresoRepository.GetAll();
            var result = new List<IngresoDTO>();

            foreach (var ingreso in ingresos.OrderByDescending(i => i.FechaHora))
            {
                var dto = new IngresoDTO
                {
                    Id = ingreso.Id,
                    ClienteID = ingreso.ClienteID,
                    FechaHora = ingreso.FechaHora
                };

                var cliente = DalFactory.ClienteRepository.GetById(ingreso.ClienteID);
                if (cliente != null)
                {
                    dto.ClienteNombre = $"{cliente.Nombre} {cliente.Apellido}";
                }
                else
                {
                    dto.ClienteNombre = "Desconocido";
                }

                result.Add(dto);
            }

            return result;
        }

        /// <summary>
        /// Returns all check-in records for a specific client, sorted by most recent first.
        /// </summary>
        /// <param name="clienteId">The client whose check-in history to fetch.</param>
        /// <returns>List of the client's check-in records; empty list if none.</returns>
        /// <summary>
        /// Returns check-in records for a specific client, ordered by timestamp descending.
        /// </summary>
        /// <param name="clienteId">The client whose check-in history to retrieve.</param>
        /// <returns>List of <see cref="IngresoDTO"/>; empty list if none exist.</returns>
        public List<IngresoDTO> ListarIngresosPorCliente(Guid clienteId)
        {
            var ingresos = DalFactory.IngresoRepository.GetAll().Where(i => i.ClienteID == clienteId);
            var result = new List<IngresoDTO>();

            var cliente = DalFactory.ClienteRepository.GetById(clienteId);
            string clienteNombre = cliente != null ? $"{cliente.Nombre} {cliente.Apellido}" : "Desconocido";

            foreach (var ingreso in ingresos.OrderByDescending(i => i.FechaHora))
            {
                var dto = new IngresoDTO
                {
                    Id = ingreso.Id,
                    ClienteID = ingreso.ClienteID,
                    FechaHora = ingreso.FechaHora,
                    ClienteNombre = clienteNombre
                };

                result.Add(dto);
            }

            return result;
        }
    }
}
