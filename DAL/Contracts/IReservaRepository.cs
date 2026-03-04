using Domain.Entities;
using Service.Contracts;
using System;
using System.Collections.Generic;

namespace DAL.Contracts
{
    public interface IReservaRepository : IGenericRepository<Reserva>
    {
        List<Reserva> GetByEspacio(Guid espacioId, DateTime desde, DateTime hasta);
        List<Reserva> GetByCliente(Guid clienteId);
        Reserva GetByCodigo(string codigoReserva);
        bool EspacioDisponible(Guid espacioId, DateTime fechaHora, int duracion);
    }
}
