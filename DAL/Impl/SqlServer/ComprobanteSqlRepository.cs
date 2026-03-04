using DAL.Contracts;
using Domain.Entities;
using System;
using System.Data.SqlClient;

namespace DAL.Impl
{
    public class ComprobanteSqlRepository : BaseBusinessSqlRepository, IComprobanteRepository
    {
        public void Agregar(Comprobante obj)
        {
            string query = "INSERT INTO Comprobante (Id, PagoID, ReservaID, NombreArchivo, RutaArchivo, FechaSubida) VALUES (@Id, @PagoID, @ReservaID, @NombreArchivo, @RutaArchivo, @FechaSubida)";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@PagoID", (object)obj.PagoID ?? DBNull.Value),
                new SqlParameter("@ReservaID", (object)obj.ReservaID ?? DBNull.Value),
                new SqlParameter("@NombreArchivo", obj.NombreArchivo),
                new SqlParameter("@RutaArchivo", obj.RutaArchivo),
                new SqlParameter("@FechaSubida", obj.FechaSubida)
            };
            ExecuteNonQuery(query, parameters);
        }

        public Comprobante GetById(Guid comprobanteId)
        {
            string query = "SELECT Id, PagoID, NombreArchivo, RutaArchivo, FechaSubida, ReservaID FROM Comprobante WHERE Id = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", comprobanteId) };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return new Comprobante
                    {
                        Id = reader.GetGuid(0),
                        PagoID = reader.IsDBNull(1) ? (Guid?)null : reader.GetGuid(1),
                        NombreArchivo = reader.GetString(2),
                        RutaArchivo = reader.GetString(3),
                        FechaSubida = reader.GetDateTime(4),
                        ReservaID = reader.IsDBNull(5) ? (Guid?)null : reader.GetGuid(5)
                    };
                }
                return null;
            });
        }

        public Comprobante GetByPago(Guid pagoId)
        {
            string query = "SELECT Id, PagoID, NombreArchivo, RutaArchivo, FechaSubida, ReservaID FROM Comprobante WHERE PagoID = @PagoID";
            SqlParameter[] parameters = { new SqlParameter("@PagoID", pagoId) };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return new Comprobante
                    {
                        Id = reader.GetGuid(0),
                        PagoID = reader.IsDBNull(1) ? (Guid?)null : reader.GetGuid(1),
                        NombreArchivo = reader.GetString(2),
                        RutaArchivo = reader.GetString(3),
                        FechaSubida = reader.GetDateTime(4),
                        ReservaID = reader.IsDBNull(5) ? (Guid?)null : reader.GetGuid(5)
                    };
                }
                return null;
            });
        }

        public Comprobante GetByReserva(Guid reservaId)
        {
            string query = "SELECT Id, PagoID, NombreArchivo, RutaArchivo, FechaSubida, ReservaID FROM Comprobante WHERE ReservaID = @ReservaID";
            SqlParameter[] parameters = { new SqlParameter("@ReservaID", reservaId) };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return new Comprobante
                    {
                        Id = reader.GetGuid(0),
                        PagoID = reader.IsDBNull(1) ? (Guid?)null : reader.GetGuid(1),
                        NombreArchivo = reader.GetString(2),
                        RutaArchivo = reader.GetString(3),
                        FechaSubida = reader.GetDateTime(4),
                        ReservaID = reader.IsDBNull(5) ? (Guid?)null : reader.GetGuid(5)
                    };
                }
                return null;
            });
        }
    }
}
