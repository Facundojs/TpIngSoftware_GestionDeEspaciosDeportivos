using DAL.Contracts;
using Domain.Entities;
using Service.Impl;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.Impl
{
    public class EjercicioSqlRepository : BaseBusinessSqlRepository, IEjercicioRepository
    {
        public void Insertar(Ejercicio obj, SqlConnection conn, SqlTransaction tran)
        {
            string query = "INSERT INTO Ejercicio (Id, RutinaID, Nombre, Repeticiones, DiaSemana, Orden) VALUES (@Id, @RutinaID, @Nombre, @Repeticiones, @DiaSemana, @Orden)";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@RutinaID", obj.RutinaID),
                new SqlParameter("@Nombre", obj.Nombre),
                new SqlParameter("@Repeticiones", obj.Repeticiones),
                new SqlParameter("@DiaSemana", obj.DiaSemana),
                new SqlParameter("@Orden", obj.Orden)
            };
            ExecuteNonQuery(query, parameters, conn, tran);
        }

        public List<Ejercicio> GetByRutina(Guid rutinaId)
        {
            string query = "SELECT Id, RutinaID, Nombre, Repeticiones, DiaSemana, Orden FROM Ejercicio WHERE RutinaID = @RutinaID ORDER BY DiaSemana, Orden";
            SqlParameter[] parameters = { new SqlParameter("@RutinaID", rutinaId) };

            return ExecuteReader(query, parameters, reader =>
            {
                var list = new List<Ejercicio>();
                while (reader.Read())
                {
                    list.Add(Map(reader));
                }
                return list;
            });
        }

        public void EliminarPorRutina(Guid rutinaId, SqlConnection conn, SqlTransaction tran)
        {
            string query = "DELETE FROM Ejercicio WHERE RutinaID = @RutinaID";
            SqlParameter[] parameters = { new SqlParameter("@RutinaID", rutinaId) };
            ExecuteNonQuery(query, parameters, conn, tran);
        }

        private Ejercicio Map(SqlDataReader reader)
        {
            return new Ejercicio
            {
                Id = reader.GetGuid(0),
                RutinaID = reader.GetGuid(1),
                Nombre = reader.GetString(2),
                Repeticiones = reader.GetInt32(3),
                DiaSemana = reader.GetInt32(4),
                Orden = reader.GetInt32(5)
            };
        }
    }
}
