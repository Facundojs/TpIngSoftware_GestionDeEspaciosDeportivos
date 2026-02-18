using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.Contracts
{
    public interface IAgendaRepository
    {
        void CrearAgenda(Agenda obj, SqlConnection conn, SqlTransaction tran);
        List<Agenda> GetByEspacio(Guid espacioId);
        void EliminarPorEspacio(Guid espacioId, SqlConnection conn, SqlTransaction tran);
    }
}
