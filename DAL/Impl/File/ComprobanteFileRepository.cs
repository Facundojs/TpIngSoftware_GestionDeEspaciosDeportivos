using DAL.Contracts;
using Domain.Entities;
using System;
using System.IO;

namespace DAL.Impl.File
{
    public class ComprobanteFileRepository : IComprobanteRepository
    {
        private readonly string _baseFolder;

        public ComprobanteFileRepository()
        {
            _baseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Comprobantes");
            if (!System.IO.Directory.Exists(_baseFolder))
            {
                System.IO.Directory.CreateDirectory(_baseFolder);
            }
        }

        public void Agregar(Comprobante obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (obj.PagoID == Guid.Empty) throw new ArgumentException("PagoID es requerido para generar la ruta del archivo");
            if (obj.Contenido == null || obj.Contenido.Length == 0) throw new ArgumentException("El contenido del archivo es requerido");

            try
            {
                // Deterministic path based on PagoID
                string filePath = GetFilePath(obj.PagoID);
                obj.RutaArchivo = filePath; // Update the entity with the generated path

                System.IO.File.WriteAllBytes(filePath, obj.Contenido);
            }
            catch (Exception ex)
            {
                throw new IOException($"Error al guardar el archivo de comprobante: {ex.Message}", ex);
            }
        }

        public Comprobante GetByPago(Guid pagoId)
        {
            if (pagoId == Guid.Empty) return null;

            string filePath = GetFilePath(pagoId);

            if (System.IO.File.Exists(filePath))
            {
                return new Comprobante
                {
                    PagoID = pagoId,
                    RutaArchivo = filePath,
                    Contenido = System.IO.File.ReadAllBytes(filePath)
                };
            }

            return null;
        }

        private string GetFilePath(Guid pagoId)
        {
            return Path.Combine(_baseFolder, $"{pagoId}.dat");
        }
    }
}
