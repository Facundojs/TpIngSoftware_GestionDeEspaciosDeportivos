using Domain.Entities;
using Service.Contracts;
using System.Collections.Generic;

namespace DAL.Contracts
{
    /// <summary>
    /// Repository contract for <see cref="Espacio"/> (sports space) entities, extending standard CRUD
    /// with availability-filtered listing.
    /// </summary>
    public interface IEspacioRepository : IGenericRepository<Espacio>
    {
        /// <summary>
        /// Retrieves only spaces whose status is <c>Activo</c>.
        /// Used when presenting spaces available for reservation.
        /// </summary>
        /// <returns>List of active spaces; empty list if none.</returns>
        List<Espacio> ListarDisponibles();
    }
}
