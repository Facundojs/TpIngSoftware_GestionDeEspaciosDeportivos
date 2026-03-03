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
        private readonly string[] extensionesPermitidas = { ".pdf", ".jpg", ".jpeg", ".png" };
        private readonly int maxSizeBytes = 5 * 1024 * 1024; // 5MB
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

            if (dto.Contenido == null || dto.Contenido.Length == 0)
                throw new ArgumentException("El contenido del comprobante no puede estar vacío");

            if (dto.Contenido.Length > maxSizeBytes)
                throw new InvalidOperationException($"El archivo supera el tamaño máximo permitido (5MB). Tamaño actual: {dto.Contenido.Length / 1024} KB");

            if (string.IsNullOrWhiteSpace(dto.NombreArchivo))
                throw new ArgumentException("El nombre del archivo es requerido para validar el formato");

            string extension = Path.GetExtension(dto.NombreArchivo).ToLower();

            if (Array.IndexOf(extensionesPermitidas, extension) < 0)
                throw new InvalidOperationException($"Formato de archivo '{extension}' no permitido. Use: PDF, JPG o PNG");

            var entity = PagoMapper.ToEntity(dto);
            if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();

            entity.FechaSubida = DateTime.Now;

            _fileRepo.Agregar(entity);

            _sqlRepo.Agregar(entity);
        }

        public ComprobanteDTO Obtener(Guid pagoId)
        {
            if (pagoId == Guid.Empty) throw new ArgumentException("El ID del pago es requerido");

            var metadata = _sqlRepo.GetByPago(pagoId);
            if (metadata == null) return null;

            var fileData = _fileRepo.GetByPago(pagoId);

            var dto = PagoMapper.ToDTO(metadata);
            if (fileData != null && fileData.Contenido != null)
            {
                dto.Contenido = fileData.Contenido;
            }

            return dto;
        }
    }
}
