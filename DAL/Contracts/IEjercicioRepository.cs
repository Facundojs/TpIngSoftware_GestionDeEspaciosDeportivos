using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.Contracts
{
    public interface IEjercicioRepository
    {
        void Insertar(Ejercicio obj, SqlConnection conn, SqlTransaction tran);
        List<Ejercicio> GetByRutina(Guid rutinaId);
        void EliminarPorRutina(Guid rutinaId, SqlConnection conn, SqlTransaction tran);
    }
}
