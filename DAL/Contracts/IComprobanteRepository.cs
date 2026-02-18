using Domain.Entities;
using System;

namespace DAL.Contracts
{
    public interface IComprobanteRepository
    {
        void Agregar(Comprobante obj);
        Comprobante GetByPago(Guid pagoId);
    }
}
