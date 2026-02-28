using Domain.Entities;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.Contracts
{
    public interface IReservaRepository : IGenericRepository<Reserva>
    {
        List<Reserva> GetByEspacio(Guid espacioId, DateTime desde, DateTime hasta);
        List<Reserva> GetByCliente(Guid clienteId);
        Reserva GetByCodigo(string codigoReserva);
        bool EspacioDisponible(Guid espacioId, DateTime fechaHora, int duracion);

        // Transactional Overloads
        void Add(Reserva obj, SqlConnection conn = null, SqlTransaction tran = null);
        void Update(Reserva obj, SqlConnection conn = null, SqlTransaction tran = null);
        bool EspacioDisponible(Guid espacioId, DateTime fechaHora, int duracion, SqlConnection conn = null, SqlTransaction tran = null);
        Reserva GetById(Guid id, SqlConnection conn = null, SqlTransaction tran = null);
    }
}
