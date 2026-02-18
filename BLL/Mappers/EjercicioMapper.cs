using BLL.DTOs;
using Domain.Entities;

namespace BLL.Mappers
{
    public static class EjercicioMapper
    {
        public static Ejercicio ToEntity(EjercicioDTO dto)
        {
            return new Ejercicio
            {
                Id = dto.Id,
                RutinaID = dto.RutinaID,
                Nombre = dto.Nombre,
                Repeticiones = dto.Repeticiones,
                DiaSemana = dto.DiaSemana,
                Orden = dto.Orden
            };
        }

        public static EjercicioDTO ToDTO(Ejercicio entity)
        {
            return new EjercicioDTO
            {
                Id = entity.Id,
                RutinaID = entity.RutinaID,
                Nombre = entity.Nombre,
                Repeticiones = entity.Repeticiones,
                DiaSemana = entity.DiaSemana,
                Orden = entity.Orden
            };
        }
    }
}
