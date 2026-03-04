using Domain.Entities;
using Service.Contracts;
using System;
using System.Collections.Generic;

namespace DAL.Contracts
{
    /// <summary>
    /// Repository contract for <see cref="Pago"/> entities, extending standard CRUD with
    /// payment-specific query operations.
    /// </summary>
    public interface IPagoRepository : IGenericRepository<Pago>
    {
        /// <summary>
        /// Retrieves payments for a client, optionally filtered by a date range.
        /// </summary>
        /// <param name="clienteId">The client whose payments to fetch.</param>
        /// <param name="desde">Optional inclusive lower bound on payment date.</param>
        /// <param name="hasta">Optional inclusive upper bound on payment date.</param>
        /// <returns>List of matching payments; empty list if none.</returns>
        List<Pago> GetByCliente(Guid clienteId, DateTime? desde, DateTime? hasta);

        /// <summary>Retrieves a payment by its sequential numeric code.</summary>
        /// <param name="codigo">The numeric payment code.</param>
        /// <returns>The matching <see cref="Pago"/>, or <c>null</c> if not found.</returns>
        Pago GetByCodigo(int codigo);

        /// <summary>Retrieves all payments associated with a specific reservation.</summary>
        /// <param name="reservaId">The reservation whose payments to fetch.</param>
        /// <returns>List of payments for the reservation; empty list if none.</returns>
        List<Pago> GetByReserva(Guid reservaId);
    }
}
