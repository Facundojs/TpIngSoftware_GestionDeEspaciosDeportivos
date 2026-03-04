using Domain.Entities;
using System;
using System.Collections.Generic;

namespace DAL.Contracts
{
    /// <summary>
    /// Repository contract for <see cref="Agenda"/> records that define the weekly operating
    /// schedule of a sports space.
    /// </summary>
    /// <remarks>
    /// Agenda records are managed as a complete set per space: configuration always deletes
    /// all existing records via <see cref="EliminarPorEspacio"/> and re-inserts the new set
    /// within the same transaction.
    /// </remarks>
    public interface IAgendaRepository
    {
        /// <summary>Inserts a single agenda time block.</summary>
        /// <param name="obj">The agenda block to create.</param>
        void CrearAgenda(Agenda obj);

        /// <summary>Retrieves all agenda blocks configured for a given space.</summary>
        /// <param name="espacioId">The space whose schedule to fetch.</param>
        /// <returns>List of agenda blocks; empty list if no schedule is configured.</returns>
        List<Agenda> GetByEspacio(Guid espacioId);

        /// <summary>
        /// Deletes all agenda blocks for the specified space in preparation for a full re-configuration.
        /// </summary>
        /// <param name="espacioId">The space whose agenda will be cleared.</param>
        void EliminarPorEspacio(Guid espacioId);
    }
}
