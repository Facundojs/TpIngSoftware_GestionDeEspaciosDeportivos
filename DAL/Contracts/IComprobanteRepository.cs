using Domain.Entities;
using System;

namespace DAL.Contracts
{
    public interface IComprobanteRepository
    {
        void Agregar(Comprobante obj);
        Comprobante GetById(Guid comprobanteId);
        Comprobante GetByPago(Guid pagoId);
        Comprobante GetByReserva(Guid reservaId);
    }
}
