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
            string query = "INSERT INTO Comprobante (Id, PagoID, NombreArchivo, RutaArchivo, FechaSubida) VALUES (@Id, @PagoID, @NombreArchivo, @RutaArchivo, @FechaSubida)";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@PagoID", obj.PagoID),
                new SqlParameter("@NombreArchivo", obj.NombreArchivo),
                new SqlParameter("@RutaArchivo", obj.RutaArchivo),
                new SqlParameter("@FechaSubida", obj.FechaSubida)
            };
            ExecuteNonQuery(query, parameters);
        }

        public Comprobante GetByPago(Guid pagoId)
        {
            string query = "SELECT Id, PagoID, NombreArchivo, RutaArchivo, FechaSubida FROM Comprobante WHERE PagoID = @PagoID";
            SqlParameter[] parameters = { new SqlParameter("@PagoID", pagoId) };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return new Comprobante
                    {
                        Id = reader.GetGuid(0),
                        PagoID = reader.GetGuid(1),
                        NombreArchivo = reader.GetString(2),
                        RutaArchivo = reader.GetString(3),
                        FechaSubida = reader.GetDateTime(4)
                    };
                }
                return null;
            });
        }
    }
}
