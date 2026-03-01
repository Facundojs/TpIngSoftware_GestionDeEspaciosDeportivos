using System;
using System.Data.SqlClient;
using Domain.Entities;
using Service.Contracts;

namespace DAL.Contracts
{
    public interface IClienteMembresiaRepository : IGenericRepository<ClienteMembresia>
    {
        ClienteMembresia GetActiveByClienteId(Guid clienteId, SqlConnection conn = null, SqlTransaction tran = null);
        void Add(ClienteMembresia obj, SqlConnection conn, SqlTransaction tran);
        void Update(ClienteMembresia obj, SqlConnection conn, SqlTransaction tran);
    }
}
