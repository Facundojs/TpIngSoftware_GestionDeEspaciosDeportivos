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

        public void CrearAgenda(Agenda obj)
        {
            string query = "INSERT INTO Agenda (EspacioID, DiaSemana, HoraDesde, HoraHasta) VALUES (@EspacioID, @DiaSemana, @HoraDesde, @HoraHasta)";
            SqlParameter[] parameters = {
                new SqlParameter("@EspacioID", obj.EspacioID),
                new SqlParameter("@DiaSemana", obj.DiaSemana),
                new SqlParameter("@HoraDesde", obj.HoraDesde),
                new SqlParameter("@HoraHasta", obj.HoraHasta)
            };
            ExecuteNonQuery(query, parameters);
        }

        public void EliminarPorEspacio(Guid espacioId)
        {
            string query = "DELETE FROM Agenda WHERE EspacioID = @EspacioID";
            SqlParameter[] parameters = { new SqlParameter("@EspacioID", espacioId) };
            ExecuteNonQuery(query, parameters);
        }

        public List<Agenda> GetByEspacio(Guid espacioId)
        {
            string query = "SELECT EspacioID, DiaSemana, HoraDesde, HoraHasta FROM Agenda WHERE EspacioID = @EspacioID ORDER BY DiaSemana, HoraDesde";
            SqlParameter[] parameters = { new SqlParameter("@EspacioID", espacioId) };

            return ExecuteReader(query, parameters, reader =>
            {
                List<Agenda> list = new List<Agenda>();
                while (reader.Read())
                {
                    list.Add(new Agenda
                    {
                        EspacioID = reader.GetGuid(0),
                        DiaSemana = reader.GetInt32(1),
                        HoraDesde = reader.GetTimeSpan(2),
                        HoraHasta = reader.GetTimeSpan(3)
                    });
                }
                return list;
            });
        }
    }
}
