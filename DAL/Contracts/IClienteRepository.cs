using System;
using System.Collections.Generic;
using Domain.Entities;
using Service.Contracts;

namespace DAL.Contracts
{
    /// <summary>
    /// Repository contract for <see cref="Cliente"/> entities, extending standard CRUD with
    /// client-specific lookup and membership operations.
    /// </summary>
    public interface IClienteRepository : IGenericRepository<Cliente>
    {
        /// <summary>Retrieves a client by their national identity number (DNI).</summary>
        /// <param name="dni">The DNI to search for.</param>
        /// <returns>The matching <see cref="Cliente"/>, or <c>null</c> if not found.</returns>
        Cliente GetByDNI(int dni);

        /// <summary>Determines whether a client with the given <paramref name="dni"/> already exists.</summary>
        /// <param name="dni">The DNI to check.</param>
        /// <returns><c>true</c> if a client with that DNI exists; otherwise <c>false</c>.</returns>
        bool ExistsByDNI(int dni);

        /// <summary>
        /// Updates the active membership reference on the client record.
        /// </summary>
        /// <param name="clienteId">The client to update.</param>
        /// <param name="membresiaId">The new membership identifier.</param>
        void AsignarMembresia(Guid clienteId, Guid membresiaId);

        /// <summary>
        /// Determines whether any active client is currently subscribed to the specified membership.
        /// Used to prevent disabling a membership that is still in use.
        /// </summary>
        /// <param name="membresiaId">The membership to check.</param>
        /// <returns><c>true</c> if at least one active client holds this membership.</returns>
        bool HasActiveClientsByMembresia(Guid membresiaId);
    }
}
