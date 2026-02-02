using Service.Contracts;
using Service.Domain;
using Service.DTO;
using Service.Factory;
using Service.Helpers;
using Service.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Logic
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioService()
        {
            _repository = FactoryDao.UsuarioRepository;
        }

        public UsuarioDTO Login(string username, string password)
        {
            var user = _repository.GetByUsername(username);
            if (user == null) throw new Exception("Usuario no encontrado");

            string hashedPassword = CryptographyHelper.HashPassword(password);
            if (user.Password != hashedPassword) throw new Exception("Contraseña incorrecta");

            if (!user.Estado) throw new Exception("Usuario bloqueado o inactivo");

            return new UsuarioDTO
            {
                Id = user.Id,
                Username = user.NombreUsuario,
                Estado = user.Estado ? "Activo" : "Bloqueado",
                Permisos = user.Permisos
            };
        }

        public void Register(UsuarioDTO dto, string password)
        {
             var existing = _repository.GetByUsername(dto.Username);
             if (existing != null) throw new Exception("El usuario ya existe");

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

        public void Update(UsuarioDTO dto)
        {
            var user = _repository.GetById(dto.Id);
            if (user == null) throw new Exception("Usuario no existe");

            user.NombreUsuario = dto.Username;
            user.Estado = dto.Estado == "Activo";

            UpdateDV(user);
            _repository.Update(user);

            if (dto.Permisos != null)
            {
                _repository.UpdateAccesos(user.Id, dto.Permisos);
            }
        }

        public void ResetPassword(Guid id, string newPassword)
        {
            var user = _repository.GetById(id);
            if (user == null) throw new Exception("Usuario no existe");

            user.Password = CryptographyHelper.HashPassword(newPassword);
            UpdateDV(user);
            _repository.Update(user);
        }

        public void Delete(Guid id)
        {
            _repository.Remove(id);
        }

        public List<UsuarioDTO> GetUsuarios()
        {
            return _repository.GetUsuariosDTO();
        }

        private void UpdateDV(Usuario user)
        {
            string raw = $"{user.Id}{user.NombreUsuario}{user.Password}{user.Estado}";
            user.DigitoVerificador = CryptographyHelper.HashPassword(raw);
        }
    }
}
