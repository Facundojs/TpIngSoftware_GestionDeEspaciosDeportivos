using Service.Domain.Composite;
using Service.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.DTO;

namespace Service.Contracts
{
    /// <summary>
    /// Interfaz que define las operaciones específicas para el repositorio de usuarios.
    /// Hereda de IGenericInterface para operaciones CRUD básicas.
    /// </summary>
    public interface IUsuarioRepository : IGenericRepository<Usuario>
    {

        /// <summary>
        /// Actualiza los accesos de un usuario.
        /// </summary>
        /// <param name="idUsuario">El identificador del usuario.</param>
        /// <param name="accesos">Lista de accesos a asignar al usuario.</param>
        void UpdateAccesos(Guid idUsuario, List<Acceso> accesos);

        /// <summary>
        /// Obtiene las familias asociadas a un usuario por su identificador.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario.</param>
        /// <returns>Lista de familias asociadas al usuario.</returns>
        List<Familia> GetFamiliasByUsuarioId(Guid usuarioId);

        /// <summary>
        /// Obtiene una lista de usuarios en formato DTO.
        /// </summary>
        /// <returns>Lista de usuarios como objetos DTO.</returns>
        List<UsuarioDTO> GetUsuariosDTO();

        /// <summary>
        /// Obtiene un usuario por su nombre de usuario.
        /// </summary>
        /// <param name="username">El nombre de usuario a buscar.</param>
        /// <returns>El objeto Usuario si se encuentra, null si no.</returns>
        Usuario GetByUsername(string username);
    }
}
