using DAL.Contracts;
using DAL.Impl;
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
        private static IMembresiaRepository _membresiaRepository;
        private static IEspacioRepository _espacioRepository;
        private static IAgendaRepository _agendaRepository;
        private static IRutinaRepository _rutinaRepository;
        private static IEjercicioRepository _ejercicioRepository;
        private static IRutinaEjercicioRepository _rutinaEjercicioRepository;
        private static IPagoRepository _pagoRepository;
        private static IComprobanteRepository _comprobanteRepository;

        public static IClienteRepository ClienteRepository
        {
            get
            {
                if (_clienteRepository == null)
                {
                    _clienteRepository = new ClienteSqlRepository();
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
                    _balanceRepository = new BalanceSqlRepository();
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
                    _movimientoRepository = new MovimientoSqlRepository();
                }
                return _movimientoRepository;
            }
        }

        public static IMembresiaRepository MembresiaRepository
        {
            get
            {
                if (_membresiaRepository == null)
                {
                    _membresiaRepository = new MembresiaSqlRepository();
                }
                return _membresiaRepository;
            }
        }

        public static IEspacioRepository EspacioRepository
        {
            get
            {
                if (_espacioRepository == null)
                {
                    _espacioRepository = new EspacioSqlRepository();
                }
                return _espacioRepository;
            }
        }

        public static IAgendaRepository AgendaRepository
        {
            get
            {
                if (_agendaRepository == null)
                {
                    _agendaRepository = new AgendaSqlRepository();
                }
                return _agendaRepository;
            }
        }

        public static IRutinaRepository RutinaRepository
        {
            get
            {
                if (_rutinaRepository == null)
                {
                    _rutinaRepository = new RutinaSqlRepository();
                }
                return _rutinaRepository;
            }
        }

        public static IEjercicioRepository EjercicioRepository
        {
            get
            {
                if (_ejercicioRepository == null)
                {
                    _ejercicioRepository = new EjercicioSqlRepository();
                }
                return _ejercicioRepository;
            }
        }

        public static IRutinaEjercicioRepository RutinaEjercicioRepository
        {
            get
            {
                if (_rutinaEjercicioRepository == null)
                {
                    _rutinaEjercicioRepository = new RutinaEjercicioSqlRepository();
                }
                return _rutinaEjercicioRepository;
            }
        }

        public static IPagoRepository PagoRepository
        {
            get
            {
                if (_pagoRepository == null)
                {
                    _pagoRepository = new PagoSqlRepository();
                }
                return _pagoRepository;
            }
        }

        public static IComprobanteRepository ComprobanteRepository
        {
            get
            {
                if (_comprobanteRepository == null)
                {
                    _comprobanteRepository = new ComprobanteSqlRepository();
                }
                return _comprobanteRepository;
            }
        }
    }
}
