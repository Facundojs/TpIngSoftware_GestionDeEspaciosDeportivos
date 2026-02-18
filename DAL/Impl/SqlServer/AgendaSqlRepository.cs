using DAL.Contracts;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.Impl
{
    public class AgendaSqlRepository : BaseBusinessSqlRepository, IAgendaRepository
    {
        public AgendaSqlRepository() : base()
        {
        }

        public void CrearAgenda(Agenda obj, SqlConnection conn, SqlTransaction tran)
        {
            string query = "INSERT INTO Agenda (EspacioID, HoraDesde, HoraHasta) VALUES (@EspacioID, @HoraDesde, @HoraHasta)";
            SqlParameter[] parameters = {
                new SqlParameter("@EspacioID", obj.EspacioID),
                new SqlParameter("@HoraDesde", obj.HoraDesde),
                new SqlParameter("@HoraHasta", obj.HoraHasta)
            };
            ExecuteNonQuery(query, parameters, conn, tran);
        }

        public void EliminarPorEspacio(Guid espacioId, SqlConnection conn, SqlTransaction tran)
        {
            string query = "DELETE FROM Agenda WHERE EspacioID = @EspacioID";
            SqlParameter[] parameters = { new SqlParameter("@EspacioID", espacioId) };
            ExecuteNonQuery(query, parameters, conn, tran);
        }

        public List<Agenda> GetByEspacio(Guid espacioId)
        {
            string query = "SELECT EspacioID, HoraDesde, HoraHasta FROM Agenda WHERE EspacioID = @EspacioID ORDER BY HoraDesde";
            SqlParameter[] parameters = { new SqlParameter("@EspacioID", espacioId) };

            return ExecuteReader(query, parameters, reader =>
            {
                List<Agenda> list = new List<Agenda>();
                while (reader.Read())
                {
                    list.Add(new Agenda
                    {
                        EspacioID = reader.GetGuid(0),
                        HoraDesde = reader.GetTimeSpan(1),
                        HoraHasta = reader.GetTimeSpan(2)
                    });
                }
                return list;
            }, null, null);
        }
    }
}
