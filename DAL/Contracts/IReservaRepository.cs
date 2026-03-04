using Domain.Entities;
using Service.Contracts;
using System;
using System.Collections.Generic;

namespace DAL.Contracts
{
    /// <summary>
    /// Repository contract for <see cref="Reserva"/> entities, extending standard CRUD with
    /// reservation-specific query and availability check operations.
    /// </summary>
    public interface IReservaRepository : IGenericRepository<Reserva>
    {
        /// <summary>
        /// Retrieves all reservations for a given space within a date-time range.
        /// </summary>
        /// <param name="espacioId">The space to filter by.</param>
        /// <param name="desde">Inclusive lower bound of the date range.</param>
        /// <param name="hasta">Inclusive upper bound of the date range.</param>
        /// <returns>List of matching reservations; empty list if none.</returns>
        List<Reserva> GetByEspacio(Guid espacioId, DateTime desde, DateTime hasta);

        /// <summary>Retrieves all reservations belonging to a specific client.</summary>
        /// <param name="clienteId">The client whose reservations to fetch.</param>
        /// <returns>List of the client's reservations; empty list if none.</returns>
        List<Reserva> GetByCliente(Guid clienteId);

        /// <summary>Retrieves a reservation by its unique human-readable code.</summary>
        /// <param name="codigoReserva">The reservation code (e.g., <c>"RES-123456-ABCD"</c>).</param>
        /// <returns>The matching <see cref="Reserva"/>, or <c>null</c> if not found.</returns>
        Reserva GetByCodigo(string codigoReserva);

        /// <summary>
        /// Checks whether a space is available for a reservation starting at <paramref name="fechaHora"/>
        /// for <paramref name="duracion"/> minutes.
        /// </summary>
        /// <param name="espacioId">The space to check.</param>
        /// <param name="fechaHora">Proposed start date and time.</param>
        /// <param name="duracion">Duration in minutes.</param>
        /// <returns><c>true</c> if no conflicting active reservation exists; otherwise <c>false</c>.</returns>
        bool EspacioDisponible(Guid espacioId, DateTime fechaHora, int duracion);
    }
}
