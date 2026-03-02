using BLL.DTOs;
using Domain.Entities;
using System;

namespace BLL.Mappers
{
    public static class ClienteMapper
    {
        public static ClienteDTO ToDTO(Cliente entity, Membresia membresia, Balance balance, DateTime? proximaFechaPago = null)
        {
            if (entity == null) return null;

            ClienteStatus status;
            if (!Enum.TryParse(entity.Estado, out status))
            {
                status = ClienteStatus.Activo;
                new Service.Logic.BitacoraService().Log($"Valor de estado no reconocido: '{entity.Estado}' para el cliente ID: {entity.Id}. Se asume 'Activo'.", "WARNING");
            }

            var dto = new ClienteDTO
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Apellido = entity.Apellido,
                DNI = entity.DNI,
                FechaNacimiento = entity.FechaNacimiento,
                Email = entity.Email,
                CreatedAt = entity.CreatedAt,
                MembresiaID = entity.MembresiaID,
                Razon = entity.Razon,
                Status = status,
                ProximaFechaPago = proximaFechaPago,
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
                Email = dto.Email,
                CreatedAt = dto.CreatedAt,
                MembresiaID = dto.MembresiaID,
                Estado = dto.Status.ToString(),
                Razon = dto.Razon
            };
        }
    }
}
