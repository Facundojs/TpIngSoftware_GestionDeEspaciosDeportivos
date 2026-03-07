using Service.Contracts;
using Domain;
using Service.DTO;
using Service.Factory;
using Service.Helpers;
using Service.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Facade.Extension;

namespace Service.Logic
{
    /// <summary>
    /// Manages user authentication, registration, profile updates, and password management.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Passwords are stored as SHA-256 hashes via <see cref="CryptographyHelper.HashPassword"/>.
    /// Every write operation recomputes <c>DigitoVerificador</c> (a SHA-256 hash of all scalar
    /// fields) to enable tamper-detection on the user record.
    /// </para>
    /// <para>
    /// Permission assignments are delegated to <see cref="IUsuarioRepository.UpdateAccesos"/>,
    /// which performs a delete-and-reinsert of the user's access tree.
    /// </para>
    /// </remarks>
    public class UsuarioService
    {
        private readonly IUsuarioRepository _repository;

        /// <summary>Initializes the service with the <see cref="IUsuarioRepository"/> singleton from <see cref="FactoryDao"/>.</summary>
        public UsuarioService()
        {
            _repository = FactoryDao.UsuarioRepository;
        }

        /// <summary>
        /// Authenticates a user by username and plaintext password.
        /// </summary>
        /// <param name="username">Username to look up.</param>
        /// <param name="password">Plaintext password; hashed internally before comparison.</param>
        /// <returns>
        /// A <see cref="UsuarioDTO"/> with the authenticated user's identity, permissions, and
        /// resolved <c>RolNegocio</c> (<c>"Administrador"</c>, <c>"Operador"</c>, or a
        /// comma-separated list of permission names).
        /// </returns>
        /// <exception cref="Exception">Thrown with localized messages when the user is not found, the password is incorrect, or the account is blocked.</exception>
        public UsuarioDTO Login(string username, string password)
        {
            try
            {
                var user = _repository.GetByUsername(username);
                if (user == null) throw new Exception("ERR_USER_NOT_FOUND".Translate());

                string hashedPassword = CryptographyHelper.HashPassword(password);
                if (user.Password != hashedPassword) throw new Exception("ERR_INCORRECT_PASSWORD".Translate());

                if (!user.Estado) throw new Exception("ERR_USER_BLOCKED".Translate());

                var dto = new UsuarioDTO
                {
                    Id = user.Id,
                    Username = user.NombreUsuario,
                    Estado = user.Estado ? "Activo" : "Bloqueado",
                    Permisos = user.Permisos
                };

                if (user.Permisos.Any(p => p.Nombre == "Administrador"))
                {
                    dto.RolNegocio = "Administrador";
                }
                else if (user.Permisos.Any(p => p.Nombre == "Operador"))
                {
                    dto.RolNegocio = "Operador";
                }
                else
                {
                    dto.RolNegocio = string.Join(", ", user.Permisos.Select(p => p.Nombre));
                }

                return dto;
            }
            catch (Exception ex)
            {
                var bitacora = new BitacoraService();
                bitacora.Log($"Failed login for user '{username}' - Reason: {ex.Message}", "WARNING");
                throw;
            }
        }

        /// <summary>
        /// Creates a new user account with the given permissions.
        /// </summary>
        /// <param name="dto">User data including optional initial permission set.</param>
        /// <param name="password">Plaintext password; stored as a SHA-256 hash.</param>
        /// <exception cref="Exception">Thrown when a user with the same username already exists.</exception>
        public void Register(UsuarioDTO dto, string password)
        {
             var existing = _repository.GetByUsername(dto.Username);
             if (existing != null) throw new Exception("ERR_USER_ALREADY_EXISTS".Translate());

             var user = new Usuario
             {
                 Id = Guid.NewGuid(),
                 NombreUsuario = dto.Username,
                 Password = CryptographyHelper.HashPassword(password),
                 Estado = true,
                 DigitoVerificador = ""
             };

             UpdateDV(user);
             _repository.Add(user);

             if (dto.Permisos != null && dto.Permisos.Any())
             {
                 _repository.UpdateAccesos(user.Id, dto.Permisos);
             }
        }

        /// <summary>
        /// Updates a user's username, status, and permission set.
        /// </summary>
        /// <param name="dto">Updated user data. <see cref="UsuarioDTO.Id"/> must identify an existing user.</param>
        /// <exception cref="Exception">Thrown when the user does not exist.</exception>
        public void Update(UsuarioDTO dto)
        {
            var user = _repository.GetById(dto.Id);
            if (user == null) throw new Exception("ERR_USER_NOT_EXIST".Translate());

            user.NombreUsuario = dto.Username;
            user.Estado = dto.Estado == "Activo";

            UpdateDV(user);
            _repository.Update(user);

            if (dto.Permisos != null)
            {
                _repository.UpdateAccesos(user.Id, dto.Permisos);
            }
        }

        /// <summary>
        /// Replaces a user's password with a new hashed value and updates the integrity check digit.
        /// </summary>
        /// <param name="id">Primary key of the user whose password to reset.</param>
        /// <param name="newPassword">New plaintext password; stored as a SHA-256 hash.</param>
        /// <exception cref="Exception">Thrown when the user does not exist.</exception>
        public void ResetPassword(Guid id, string newPassword)
        {
            var user = _repository.GetById(id);
            if (user == null) throw new Exception("ERR_USER_NOT_EXIST".Translate());

            user.Password = CryptographyHelper.HashPassword(newPassword);
            UpdateDV(user);
            _repository.Update(user);
        }

        /// <summary>Permanently deletes the user with the given identifier.</summary>
        /// <param name="id">Primary key of the user to delete.</param>
        public void Delete(Guid id)
        {
            _repository.Remove(id);
        }

        /// <summary>Returns all users as DTOs via <see cref="IUsuarioRepository.GetUsuariosDTO"/>.</summary>
        public List<UsuarioDTO> GetUsuarios()
        {
            return _repository.GetUsuariosDTO();
        }

        /// <summary>
        /// Recomputes and stores the integrity check digit for a user entity.
        /// </summary>
        /// <param name="user">The user entity to update in place.</param>
        /// <remarks>
        /// The check digit is a SHA-256 hash of the concatenated <c>Id</c>, <c>NombreUsuario</c>,
        /// <c>Password</c>, and <c>Estado</c> values, providing tamper detection for the user record.
        /// </remarks>
        private void UpdateDV(Usuario user)
        {
            string raw = $"{user.Id}{user.NombreUsuario}{user.Password}{user.Estado}";
            user.DigitoVerificador = CryptographyHelper.HashPassword(raw);
        }
    }
}
