using DAL.Contracts;
using Domain.Entities;
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

        #region IGenericRepository Implementation

        public void Add(Reserva obj)
        {
            Add(obj, null, null);
        }

        public void Update(Reserva obj)
        {
            Update(obj, null, null);
        }

        public void Remove(Guid id)
        {
            Remove(id, null, null);
        }

        public Reserva GetById(Guid id)
        {
            return GetById(id, null, null);
        }

        public List<Reserva> GetAll()
        {
            return GetAll(null, null);
        }

        #endregion

        #region Custom Methods Implementation

        public List<Reserva> GetByEspacio(Guid espacioId, DateTime desde, DateTime hasta)
        {
            return GetByEspacio(espacioId, desde, hasta, null, null);
        }

        public List<Reserva> GetByCliente(Guid clienteId)
        {
            return GetByCliente(clienteId, null, null);
        }

        public Reserva GetByCodigo(string codigoReserva)
        {
            return GetByCodigo(codigoReserva, null, null);
        }

        public bool EspacioDisponible(Guid espacioId, DateTime fechaHora, int duracion)
        {
            return EspacioDisponible(espacioId, fechaHora, duracion, null, null);
        }

        #endregion

        #region UoW Overloads & Implementation

        public void Add(Reserva obj, SqlConnection conn = null, SqlTransaction tran = null)
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
            ExecuteNonQuery(query, parameters, conn, tran);
        }

        public void Update(Reserva obj, SqlConnection conn = null, SqlTransaction tran = null)
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
            ExecuteNonQuery(query, parameters, conn, tran);
        }

        public void Remove(Guid id, SqlConnection conn = null, SqlTransaction tran = null)
        {
            string query = "DELETE FROM Reserva WHERE Id = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", id) };
            ExecuteNonQuery(query, parameters, conn, tran);
        }

        public Reserva GetById(Guid id, SqlConnection conn = null, SqlTransaction tran = null)
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
            }, conn, tran);
        }

        public List<Reserva> GetAll(SqlConnection conn = null, SqlTransaction tran = null)
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
            }, conn, tran);
        }

        public List<Reserva> GetByEspacio(Guid espacioId, DateTime desde, DateTime hasta, SqlConnection conn = null, SqlTransaction tran = null)
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
            }, conn, tran);
        }

        public List<Reserva> GetByCliente(Guid clienteId, SqlConnection conn = null, SqlTransaction tran = null)
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
            }, conn, tran);
        }

        public Reserva GetByCodigo(string codigoReserva, SqlConnection conn = null, SqlTransaction tran = null)
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
            }, conn, tran);
        }

        public bool EspacioDisponible(Guid espacioId, DateTime fechaHora, int duracion, SqlConnection conn = null, SqlTransaction tran = null)
        {
            string query = @"
                SELECT COUNT(*)
                FROM Reserva
                WHERE EspacioID = @EspacioId
                  AND Estado != 'Cancelada'
                  AND FechaHora < DATEADD(minute, @Duracion, @FechaHoraInicio)
                  AND DATEADD(minute, Duracion, FechaHora) > @FechaHoraInicio";

            SqlParameter[] parameters = {
                new SqlParameter("@EspacioId", espacioId),
                new SqlParameter("@FechaHoraInicio", fechaHora),
                new SqlParameter("@Duracion", duracion)
            };

            int count = ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return reader.GetInt32(0);
                }
                return 0;
            }, conn, tran);

            return count == 0;
        }

        #endregion

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
