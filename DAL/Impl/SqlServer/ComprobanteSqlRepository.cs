using DAL.Contracts;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.Impl
{
    public class ComprobanteSqlRepository : BaseBusinessSqlRepository, IComprobanteRepository
    {
        public void Agregar(Comprobante obj)
        {
            Agregar(obj, null, null);
        }

        public Comprobante GetByPago(Guid pagoId)
        {
            return GetByPago(pagoId, null, null);
        }

        #region UoW Overloads

        public void Agregar(Comprobante obj, SqlConnection conn = null, SqlTransaction tran = null)
        {
            string query = "INSERT INTO Comprobante (Id, PagoID, NombreArchivo, RutaArchivo, FechaSubida) VALUES (@Id, @PagoID, @NombreArchivo, @RutaArchivo, @FechaSubida)";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@PagoID", obj.PagoID),
                new SqlParameter("@NombreArchivo", obj.NombreArchivo),
                new SqlParameter("@RutaArchivo", obj.RutaArchivo),
                new SqlParameter("@FechaSubida", obj.FechaSubida)
            };
            ExecuteNonQuery(query, parameters, conn, tran);
        }

        public Comprobante GetByPago(Guid pagoId, SqlConnection conn = null, SqlTransaction tran = null)
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
            }, conn, tran);
        }

        #endregion
    }
}
