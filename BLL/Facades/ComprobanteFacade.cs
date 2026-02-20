using BLL.DTOs;
using BLL.Mappers;
using DAL.Contracts;
using DAL.Factory;
using Domain.Entities;
using System;
using System.IO;

namespace BLL.Facades
{
    public class ComprobanteFacade
    {
        private readonly IComprobanteRepository _sqlRepo;
        private readonly IComprobanteRepository _fileRepo;

        public ComprobanteFacade()
        {
            _sqlRepo = DalFactory.ComprobanteRepository;
            _fileRepo = DalFactory.ComprobanteFileRepository;
        }

        public void Adjuntar(ComprobanteDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.PagoID == Guid.Empty) throw new ArgumentException("El ID del pago es requerido");

            // Map to entity
            var entity = PagoMapper.ToEntity(dto);
            if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();

            // Set file path if not provided or to standardize
            // Using a "Comprobantes" folder in the base directory
            string baseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Comprobantes");

            // Generate a safe file name
            string extension = Path.GetExtension(dto.NombreArchivo);
            if (string.IsNullOrEmpty(extension)) extension = ".dat"; // Fallback

            string fileName = $"{entity.Id}{extension}";
            string fullPath = Path.Combine(baseFolder, fileName);

            entity.RutaArchivo = fullPath;
            entity.FechaSubida = DateTime.Now;

            // 1. Save File to Disk
            // The file repo uses the entity's RutaArchivo and Contenido
            _fileRepo.Agregar(entity);

            // 2. Save Metadata to DB
            // The SQL repo uses the entity's properties (Id, PagoId, RutaArchivo, etc)
            _sqlRepo.Agregar(entity);
        }
    }
}
