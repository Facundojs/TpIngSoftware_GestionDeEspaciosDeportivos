using BLL.DTOs;
using Domain.Entities;
using System;

namespace BLL.Mappers
{
    public static class ClienteMapper
    {
        public static ClienteDTO ToDTO(Cliente entity, Membresia membresia, Balance balance)
        {
            if (entity == null) return null;

            var dto = new ClienteDTO
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Apellido = entity.Apellido,
                DNI = entity.DNI,
                FechaNacimiento = entity.FechaNacimiento,
                MembresiaID = entity.MembresiaID,
                Activo = entity.Activo,
                Balance = balance != null ? balance.Saldo : 0,
                MembresiaDetalle = membresia != null ? MembresiaMapper.ToDTO(membresia) : null
            };

            if (dto.Balance < 0)
            {
                dto.EstadoBalance = EstadoBalance.Deudor;
            }
            else if (dto.Balance > 0)
            {
                dto.EstadoBalance = EstadoBalance.AFavor;
            }
            else
            {
                dto.EstadoBalance = EstadoBalance.AlDia;
            }

            return dto;
        }

        public static Cliente ToEntity(ClienteDTO dto)
        {
            if (dto == null) return null;

            return new Cliente
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                DNI = dto.DNI,
                FechaNacimiento = dto.FechaNacimiento,
                MembresiaID = dto.MembresiaID,
                Activo = dto.Activo
            };
        }
    }
}
