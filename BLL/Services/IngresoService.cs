using System;
using System.Collections.Generic;
using System.Linq;
using BLL.DTOs;
using DAL.Factory;
using Domain.Entities;

namespace BLL.Services
{
    public class IngresoService
    {
        public List<IngresoDTO> ListarIngresos()
        {
            var ingresos = DalFactory.IngresoRepository.GetAll();
            var result = new List<IngresoDTO>();

            foreach (var ingreso in ingresos.OrderByDescending(i => i.FechaHora))
            {
                var dto = new IngresoDTO
                {
                    Id = ingreso.Id,
                    ClienteID = ingreso.ClienteID,
                    FechaHora = ingreso.FechaHora
                };

                var cliente = DalFactory.ClienteRepository.GetById(ingreso.ClienteID);
                if (cliente != null)
                {
                    dto.ClienteNombre = $"{cliente.Nombre} {cliente.Apellido}";
                }
                else
                {
                    dto.ClienteNombre = "Desconocido";
                }

                result.Add(dto);
            }

            return result;
        }

        public List<IngresoDTO> ListarIngresosPorCliente(Guid clienteId)
        {
            var ingresos = DalFactory.IngresoRepository.GetAll().Where(i => i.ClienteID == clienteId);
            var result = new List<IngresoDTO>();

            var cliente = DalFactory.ClienteRepository.GetById(clienteId);
            string clienteNombre = cliente != null ? $"{cliente.Nombre} {cliente.Apellido}" : "Desconocido";

            foreach (var ingreso in ingresos.OrderByDescending(i => i.FechaHora))
            {
                var dto = new IngresoDTO
                {
                    Id = ingreso.Id,
                    ClienteID = ingreso.ClienteID,
                    FechaHora = ingreso.FechaHora,
                    ClienteNombre = clienteNombre
                };

                result.Add(dto);
            }

            return result;
        }
    }
}
