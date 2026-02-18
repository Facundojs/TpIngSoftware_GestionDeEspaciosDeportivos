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
                Nombre = dto.Nombre
            };
        }

        public static EjercicioDTO ToDTO(RutinaEjercicio entity)
        {
            return new EjercicioDTO
            {
                Id = entity.EjercicioId, // Using the Exercise ID
                RutinaID = entity.RutinaId,
                Nombre = entity.Ejercicio != null ? entity.Ejercicio.Nombre : "Desconocido",
                Repeticiones = entity.Repeticiones,
                DiaSemana = entity.DiaSemana,
                Orden = entity.Orden
            };
        }
    }
}
