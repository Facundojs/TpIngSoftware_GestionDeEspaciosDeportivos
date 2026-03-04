using Domain.Entities;
using Service.Contracts;
using System;
using System.Collections.Generic;

namespace DAL.Contracts
{
    /// <summary>
    /// Repository contract for <see cref="Membresia"/> (membership plan) entities, extending
    /// standard CRUD with plan-specific lookups.
    /// </summary>
    public interface IMembresiaRepository : IGenericRepository<Membresia>
    {
        /// <summary>Retrieves a membership plan by its sequential numeric code.</summary>
        /// <param name="codigo">The numeric membership code.</param>
        /// <returns>The matching <see cref="Membresia"/>, or <c>null</c> if not found.</returns>
        Membresia GetByCodigo(int codigo);

        /// <summary>Retrieves all membership plans that have <c>Activa = true</c>.</summary>
        /// <returns>List of active plans; empty list if none.</returns>
        List<Membresia> ListarActivas();
    }
}
