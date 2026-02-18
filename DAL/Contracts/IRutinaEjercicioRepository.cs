using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.Contracts
{
    public interface IRutinaEjercicioRepository
    {
        void Insertar(RutinaEjercicio obj, SqlConnection conn, SqlTransaction tran);
        List<RutinaEjercicio> GetByRutina(Guid rutinaId);
        void EliminarPorRutina(Guid rutinaId, SqlConnection conn, SqlTransaction tran);
    }
}
