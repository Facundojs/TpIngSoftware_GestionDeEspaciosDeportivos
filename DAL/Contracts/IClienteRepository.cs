using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Domain.Entities;
using Service.Contracts;

namespace DAL.Contracts
{
    public interface IClienteRepository : IGenericRepository<Cliente>
    {
        Cliente GetByDNI(int dni);
        bool ExistsByDNI(int dni);
        void AsignarMembresia(Guid clienteId, Guid membresiaId, SqlConnection conn, SqlTransaction tran);
    }
}
