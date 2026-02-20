using Domain.Entities;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.Contracts
{
    public interface IPagoRepository : IGenericRepository<Pago>
    {
        List<Pago> GetByCliente(Guid clienteId, DateTime? desde, DateTime? hasta);
        Pago GetByCodigo(int codigo);
        Pago GetByReserva(Guid reservaId);

        void Add(Pago obj, SqlConnection conn = null, SqlTransaction tran = null);
        void Update(Pago obj, SqlConnection conn = null, SqlTransaction tran = null);
        Pago GetById(Guid id, SqlConnection conn = null, SqlTransaction tran = null);
    }
}
