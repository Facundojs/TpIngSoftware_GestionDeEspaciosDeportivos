using DAL.Contracts;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Factory
{
    public static class DalFactory
    {
        private static IClienteRepository _clienteRepository;
        private static IBalanceRepository _balanceRepository;
        private static IMovimientoRepository _movimientoRepository;

        public static IClienteRepository ClienteRepository
        {
            get
            {
                if (_clienteRepository == null)
                {
                    _clienteRepository = new ClienteRepository();
                }
                return _clienteRepository;
            }
        }

        public static IBalanceRepository BalanceRepository
        {
            get
            {
                if (_balanceRepository == null)
                {
                    _balanceRepository = new BalanceRepository();
                }
                return _balanceRepository;
            }
        }

        public static IMovimientoRepository MovimientoRepository
        {
            get
            {
                if (_movimientoRepository == null)
                {
                    _movimientoRepository = new MovimientoRepository();
                }
                return _movimientoRepository;
            }
        }
    }
}
