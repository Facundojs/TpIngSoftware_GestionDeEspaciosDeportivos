using Domain.Entities;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.Contracts
{
    public interface IRutinaRepository : IGenericRepository<Rutina>
    {
        Rutina GetActivaByCliente(Guid clienteId);
        void FinalizarRutina(Guid rutinaId, SqlConnection conn, SqlTransaction tran);
        void Add(Rutina obj, SqlConnection conn, SqlTransaction tran);
    }
}
