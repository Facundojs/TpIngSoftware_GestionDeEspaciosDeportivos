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

        private static IMembresiaRepository _membresiaRepository;
        public static IMembresiaRepository MembresiaRepository
        {
            get
            {
                if (_membresiaRepository == null)
                {
                    _membresiaRepository = new MembresiaRepository();
                }
                return _membresiaRepository;
            }
        }

        private static IMovimientoRepository _movimientoRepository;
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
