using Domain.Entities;
using System;
using System.Collections.Generic;

namespace DAL.Contracts
{
    public interface IRutinaEjercicioRepository
    {
        void Insertar(RutinaEjercicio obj);
        List<RutinaEjercicio> GetByRutina(Guid rutinaId);
        void EliminarPorRutina(Guid rutinaId);
    }
}
