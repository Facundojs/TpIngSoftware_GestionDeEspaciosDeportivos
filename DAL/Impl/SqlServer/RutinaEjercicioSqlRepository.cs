using DAL.Contracts;
using Domain.Entities;
using Service.Impl;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.Impl
{
    public class RutinaEjercicioSqlRepository : BaseBusinessSqlRepository, IRutinaEjercicioRepository
    {
        public void Insertar(RutinaEjercicio obj, SqlConnection conn, SqlTransaction tran)
        {
            string query = "INSERT INTO RutinaEjercicio (RutinaID, EjercicioID, Repeticiones, DiaSemana, Orden) VALUES (@RutinaID, @EjercicioID, @Repeticiones, @DiaSemana, @Orden)";
            SqlParameter[] parameters = {
                new SqlParameter("@RutinaID", obj.RutinaId),
                new SqlParameter("@EjercicioID", obj.EjercicioId),
                new SqlParameter("@Repeticiones", obj.Repeticiones),
                new SqlParameter("@DiaSemana", obj.DiaSemana),
                new SqlParameter("@Orden", obj.Orden)
            };
            ExecuteNonQuery(query, parameters, conn, tran);
        }

        public List<RutinaEjercicio> GetByRutina(Guid rutinaId)
        {
            string query = "SELECT re.RutinaID, re.EjercicioID, re.Repeticiones, re.DiaSemana, re.Orden, e.Id, e.Nombre " +
                           "FROM RutinaEjercicio re " +
                           "JOIN Ejercicio e ON re.EjercicioID = e.Id " +
                           "WHERE re.RutinaID = @RutinaID " +
                           "ORDER BY re.DiaSemana, re.Orden";
            SqlParameter[] parameters = { new SqlParameter("@RutinaID", rutinaId) };

            return ExecuteReader(query, parameters, reader =>
            {
                var list = new List<RutinaEjercicio>();
                while (reader.Read())
                {
                    list.Add(Map(reader));
                }
                return list;
            });
        }

        public void EliminarPorRutina(Guid rutinaId, SqlConnection conn, SqlTransaction tran)
        {
            string query = "DELETE FROM RutinaEjercicio WHERE RutinaID = @RutinaID";
            SqlParameter[] parameters = { new SqlParameter("@RutinaID", rutinaId) };
            ExecuteNonQuery(query, parameters, conn, tran);
        }

        private RutinaEjercicio Map(SqlDataReader reader)
        {
            var re = new RutinaEjercicio
            {
                RutinaId = reader.GetGuid(0),
                EjercicioId = reader.GetGuid(1),
                Repeticiones = reader.GetInt32(2),
                DiaSemana = reader.GetInt32(3),
                Orden = reader.GetInt32(4),
                Ejercicio = new Ejercicio
                {
                    Id = reader.GetGuid(5),
                    Nombre = reader.GetString(6)
                }
            };
            return re;
        }
    }
}
