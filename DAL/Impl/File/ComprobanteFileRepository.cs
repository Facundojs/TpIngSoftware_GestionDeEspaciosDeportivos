using DAL.Contracts;
using Domain.Entities;
using System;
using System.IO;

namespace DAL.Impl.File
{
    public class ComprobanteFileRepository : IComprobanteRepository
    {
        public void Agregar(Comprobante obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrWhiteSpace(obj.RutaArchivo)) throw new ArgumentException("RutaArchivo es requerido");
            if (obj.Contenido == null || obj.Contenido.Length == 0) throw new ArgumentException("El contenido del archivo es requerido");

            try
            {
                // Ensure directory exists
                string directory = Path.GetDirectoryName(obj.RutaArchivo);
                if (!string.IsNullOrEmpty(directory) && !System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }

                System.IO.File.WriteAllBytes(obj.RutaArchivo, obj.Contenido);
            }
            catch (Exception ex)
            {
                throw new IOException($"Error al guardar el archivo de comprobante: {ex.Message}", ex);
            }
        }

        public Comprobante GetByPago(Guid pagoId)
        {
            // This method is primarily for retrieval. In a file-based repo, we might not be able to query by PagoId directly
            // without an index. However, the requirement is to orchestrate storage.
            // If the caller provides the path, we can read it. But GetByPago implies we know the path from PagoId.
            // Typically, the SQL repo holds the metadata (Path) and the File repo reads using the Path.
            // For this implementation, returning null or throwing NotImplemented might be acceptable if not used,
            // but let's implement a basic read if we assume a standard path convention or if this method isn't the primary way to read content.

            // Since we can't look up the file path from PagoId without the SQL DB, this method is limited in a standalone FileRepo
            // unless we enforce a naming convention like "Comprobantes/{PagoId}.ext".

            // Returning null as this responsibility (metadata lookup) belongs to SQL repo.
            // The Facade will handle the coordination: Get from SQL (get path) -> Read from File.
            return null;
        }

        // Helper method to read file if path is known
        public byte[] LeerArchivo(string ruta)
        {
            if (System.IO.File.Exists(ruta))
            {
                return System.IO.File.ReadAllBytes(ruta);
            }
            return null;
        }
    }
}
