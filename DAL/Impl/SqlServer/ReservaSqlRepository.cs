using DAL.Contracts;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.Impl
{
    public class ReservaSqlRepository : BaseBusinessSqlRepository, IReservaRepository
    {
        public ReservaSqlRepository() : base()
        {
        }

        public void Add(Reserva obj)
        {
            string query = "INSERT INTO Reserva (Id, ClienteID, EspacioID, Fecha, FechaHora, Duracion, Adelanto, CodigoReserva, Estado) VALUES (@Id, @ClienteID, @EspacioID, @Fecha, @FechaHora, @Duracion, @Adelanto, @CodigoReserva, @Estado)";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@ClienteID", obj.ClienteID),
                new SqlParameter("@EspacioID", obj.EspacioID),
                new SqlParameter("@Fecha", obj.Fecha),
                new SqlParameter("@FechaHora", obj.FechaHora),
                new SqlParameter("@Duracion", obj.Duracion),
                new SqlParameter("@Adelanto", obj.Adelanto),
                new SqlParameter("@CodigoReserva", obj.CodigoReserva),
                new SqlParameter("@Estado", obj.Estado)
            };
            ExecuteNonQuery(query, parameters);
        }

        public void Update(Reserva obj)
        {
            string query = "UPDATE Reserva SET ClienteID = @ClienteID, EspacioID = @EspacioID, Fecha = @Fecha, FechaHora = @FechaHora, Duracion = @Duracion, Adelanto = @Adelanto, CodigoReserva = @CodigoReserva, Estado = @Estado WHERE Id = @Id";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@ClienteID", obj.ClienteID),
                new SqlParameter("@EspacioID", obj.EspacioID),
                new SqlParameter("@Fecha", obj.Fecha),
                new SqlParameter("@FechaHora", obj.FechaHora),
                new SqlParameter("@Duracion", obj.Duracion),
                new SqlParameter("@Adelanto", obj.Adelanto),
                new SqlParameter("@CodigoReserva", obj.CodigoReserva),
                new SqlParameter("@Estado", obj.Estado)
            };
            ExecuteNonQuery(query, parameters);
        }

        public void Remove(Guid id)
        {
            string query = "DELETE FROM Reserva WHERE Id = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", id) };
            ExecuteNonQuery(query, parameters);
        }

        public Reserva GetById(Guid id)
        {
            string query = "SELECT Id, ClienteID, EspacioID, Fecha, FechaHora, Duracion, Adelanto, CodigoReserva, Estado FROM Reserva WHERE Id = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", id) };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return MapFromReader(reader);
                }
                return null;
            });
        }

        public List<Reserva> GetAll()
        {
            string query = "SELECT Id, ClienteID, EspacioID, Fecha, FechaHora, Duracion, Adelanto, CodigoReserva, Estado FROM Reserva ORDER BY FechaHora DESC";
            return ExecuteReader(query, null, reader =>
            {
                List<Reserva> list = new List<Reserva>();
                while (reader.Read())
                {
                    list.Add(MapFromReader(reader));
                }
                return list;
            });
        }

        public List<Reserva> GetByEspacio(Guid espacioId, DateTime desde, DateTime hasta)
        {
            string query = "SELECT Id, ClienteID, EspacioID, Fecha, FechaHora, Duracion, Adelanto, CodigoReserva, Estado FROM Reserva WHERE EspacioID = @EspacioID AND Fecha >= @Desde AND Fecha <= @Hasta ORDER BY FechaHora";
            SqlParameter[] parameters = {
                new SqlParameter("@EspacioID", espacioId),
                new SqlParameter("@Desde", desde),
                new SqlParameter("@Hasta", hasta)
            };

            return ExecuteReader(query, parameters, reader =>
            {
                List<Reserva> list = new List<Reserva>();
                while (reader.Read())
                {
                    list.Add(MapFromReader(reader));
                }
                return list;
            });
        }

        public List<Reserva> GetByCliente(Guid clienteId)
        {
            string query = "SELECT Id, ClienteID, EspacioID, Fecha, FechaHora, Duracion, Adelanto, CodigoReserva, Estado FROM Reserva WHERE ClienteID = @ClienteID ORDER BY FechaHora DESC";
            SqlParameter[] parameters = { new SqlParameter("@ClienteID", clienteId) };

            return ExecuteReader(query, parameters, reader =>
            {
                List<Reserva> list = new List<Reserva>();
                while (reader.Read())
                {
                    list.Add(MapFromReader(reader));
                }
                return list;
            });
        }

        public Reserva GetByCodigo(string codigoReserva)
        {
            string query = "SELECT Id, ClienteID, EspacioID, Fecha, FechaHora, Duracion, Adelanto, CodigoReserva, Estado FROM Reserva WHERE CodigoReserva = @CodigoReserva";
            SqlParameter[] parameters = { new SqlParameter("@CodigoReserva", codigoReserva) };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return MapFromReader(reader);
                }
                return null;
            });
        }

        public bool EspacioDisponible(Guid espacioId, DateTime fechaHora, int duracion)
        {
            string agendaQuery = "SELECT HoraDesde, HoraHasta FROM Agenda WHERE EspacioID = @EspacioId AND DiaSemana = @DiaSemana";
            SqlParameter[] agendaParams = {
                new SqlParameter("@EspacioId", espacioId),
                new SqlParameter("@DiaSemana", (int)fechaHora.DayOfWeek)
            };

            var agendas = ExecuteReader(agendaQuery, agendaParams, reader =>
            {
                var list = new List<Tuple<TimeSpan, TimeSpan>>();
                while (reader.Read())
                {
                    list.Add(Tuple.Create(reader.GetTimeSpan(0), reader.GetTimeSpan(1)));
                }
                return list;
            });

            if (agendas.Count == 0)
            {
                throw new InvalidOperationException("ERR_NO_AGENDA");
            }

            TimeSpan reqDesde = fechaHora.TimeOfDay;
            TimeSpan reqHasta = fechaHora.AddMinutes(duracion).TimeOfDay;
            bool withinAgenda = false;

            foreach (var a in agendas)
            {
                if (reqDesde >= a.Item1 && reqHasta <= a.Item2)
                {
                    withinAgenda = true;
                    break;
                }
            }

            if (!withinAgenda)
            {
                return false;
            }

            string query = @"
                SELECT COUNT(*)
                FROM Reserva
                WHERE EspacioID = @EspacioId
                  AND Estado != @EstadoCancelado
                  AND FechaHora < DATEADD(minute, @Duracion, @FechaHoraInicio)
                  AND DATEADD(minute, Duracion, FechaHora) > @FechaHoraInicio";

            SqlParameter[] parameters = {
                new SqlParameter("@EspacioId", espacioId),
                new SqlParameter("@FechaHoraInicio", fechaHora),
                new SqlParameter("@Duracion", duracion),
                new SqlParameter("@EstadoCancelado", EstadoReserva.Cancelada.ToString())
            };

            int count = ExecuteScalar<int>(query, parameters);

            return count == 0;
        }

        private Reserva MapFromReader(SqlDataReader reader)
        {
            return new Reserva
            {
                Id = reader.GetGuid(0),
                ClienteID = reader.GetGuid(1),
                EspacioID = reader.GetGuid(2),
                Fecha = reader.GetDateTime(3),
                FechaHora = reader.GetDateTime(4),
                Duracion = reader.GetInt32(5),
                Adelanto = reader.GetDecimal(6),
                CodigoReserva = reader.GetString(7),
                Estado = reader.GetString(8)
            };
        }
    }
}
