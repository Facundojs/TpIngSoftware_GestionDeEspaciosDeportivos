using DAL.Contracts;
using Domain.Entities;
using Domain.Enums;
using Service.Facade.Extension;
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
            if (obj.Contenido == null || obj.Contenido.Length == 0) throw new ArgumentException(Translations.ERR_COMPROBANTE_VACIO.Translate());

            try
            {
                string filePath = GetFilePath(obj.Id);
                obj.RutaArchivo = filePath;

                System.IO.File.WriteAllBytes(filePath, obj.Contenido);
            }
            catch (Exception ex)
            {
                throw new IOException($"Error saving receipt file: {ex.Message}", ex);
            }
        }

        public Comprobante GetById(Guid comprobanteId)
        {
            if (comprobanteId == Guid.Empty) return null;

            string filePath = GetFilePath(comprobanteId);

            if (System.IO.File.Exists(filePath))
            {
                return new Comprobante
                {
                    RutaArchivo = filePath,
                    Contenido = System.IO.File.ReadAllBytes(filePath)
                };
            }

            return null;
        }

        public Comprobante GetByPago(Guid pagoId)
        {
            return null;
        }

        public Comprobante GetByReserva(Guid reservaId)
        {
            return null;
        }

        private string GetFilePath(Guid id)
        {
            return Path.Combine(_baseFolder, $"{id}.dat");
        }
    }
}
