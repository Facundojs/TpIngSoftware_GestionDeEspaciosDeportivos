using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.DTO;

namespace Service.Contracts
{
    /// <summary>
    /// Repository contract for <see cref="Usuario"/> entities, extending standard CRUD with
    /// user-specific permission and lookup operations.
    /// </summary>
    public interface IUsuarioRepository : IGenericRepository<Usuario>
    {
        /// <summary>
        /// Replaces the full set of <see cref="Acceso"/> nodes assigned to a user.
        /// The existing permission associations are deleted and re-inserted atomically.
        /// </summary>
        /// <param name="idUsuario">The user whose permissions will be replaced.</param>
        /// <param name="accesos">New list of <see cref="Acceso"/> roots to assign.</param>
        void UpdateAccesos(Guid idUsuario, List<Acceso> accesos);

        /// <summary>
        /// Retrieves the top-level <see cref="Familia"/> nodes assigned to a user.
        /// Each family is fully hydrated (recursive children loaded).
        /// </summary>
        /// <param name="usuarioId">The user to look up.</param>
        /// <returns>List of assigned families; empty list if none.</returns>
        List<Familia> GetFamiliasByUsuarioId(Guid usuarioId);

        /// <summary>
        /// Returns all users projected as <see cref="UsuarioDTO"/>, including their permission trees.
        /// </summary>
        /// <returns>List of user DTOs; empty list if no users exist.</returns>
        List<UsuarioDTO> GetUsuariosDTO();

        /// <summary>
        /// Finds a user by login name (case-insensitive comparison depends on database collation).
        /// </summary>
        /// <param name="username">The login name to search for.</param>
        /// <returns>The matching <see cref="Usuario"/>, or <c>null</c> if not found.</returns>
        Usuario GetByUsername(string username);
    }
}
