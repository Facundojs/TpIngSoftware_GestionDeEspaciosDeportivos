using Domain.Entities;
using Service.Contracts;
using System;
using System.Collections.Generic;

namespace DAL.Contracts
{
    /// <summary>
    /// Repository contract for <see cref="Rutina"/> entities, extending standard CRUD with
    /// routine lifecycle operations.
    /// </summary>
    public interface IRutinaRepository : IGenericRepository<Rutina>
    {
        /// <summary>
        /// Retrieves the currently active (open-ended) routine for a client.
        /// A routine is active when its <c>Hasta</c> field is <c>null</c>.
        /// </summary>
        /// <param name="clienteId">The client to look up.</param>
        /// <returns>The active <see cref="Rutina"/>, or <c>null</c> if the client has no active routine.</returns>
        Rutina GetActivaByCliente(Guid clienteId);

        /// <summary>
        /// Closes a routine by setting its <c>Hasta</c> field to the current date-time.
        /// Called when creating a new routine for a client who already has one active.
        /// </summary>
        /// <param name="rutinaId">The routine to finalize.</param>
        void FinalizarRutina(Guid rutinaId);
    }
}
