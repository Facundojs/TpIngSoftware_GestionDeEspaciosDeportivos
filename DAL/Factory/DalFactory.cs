using DAL.Contracts;
using DAL.Impl;
using DAL.Impl.SqlServer;

namespace DAL.Factory
{
    public static class DalFactory
    {
        public static IUnitOfWork CreateUnitOfWork()
        {
            return new UnitOfWork();
        }

        // Keep existing singleton-style access for non-transactional reads if preferred
        private static IClienteRepository _clienteRepository;
        public static IClienteRepository ClienteRepository => _clienteRepository ?? (_clienteRepository = new ClienteSqlRepository());

        private static IMembresiaRepository _membresiaRepository;
        public static IMembresiaRepository MembresiaRepository => _membresiaRepository ?? (_membresiaRepository = new MembresiaSqlRepository());

        private static IReservaRepository _reservaRepository;
        public static IReservaRepository ReservaRepository => _reservaRepository ?? (_reservaRepository = new ReservaSqlRepository());

        private static IEspacioRepository _espacioRepository;
        public static IEspacioRepository EspacioRepository => _espacioRepository ?? (_espacioRepository = new EspacioSqlRepository());

        private static IAgendaRepository _agendaRepository;
        public static IAgendaRepository AgendaRepository => _agendaRepository ?? (_agendaRepository = new AgendaSqlRepository());

        private static IPagoRepository _pagoRepository;
        public static IPagoRepository PagoRepository => _pagoRepository ?? (_pagoRepository = new PagoSqlRepository());

        private static IMovimientoRepository _movimientoRepository;
        public static IMovimientoRepository MovimientoRepository => _movimientoRepository ?? (_movimientoRepository = new MovimientoSqlRepository());

        private static IBalanceRepository _balanceRepository;
        public static IBalanceRepository BalanceRepository => _balanceRepository ?? (_balanceRepository = new BalanceSqlRepository());

        private static IRutinaRepository _rutinaRepository;
        public static IRutinaRepository RutinaRepository => _rutinaRepository ?? (_rutinaRepository = new RutinaSqlRepository());

        private static IEjercicioRepository _ejercicioRepository;
        public static IEjercicioRepository EjercicioRepository => _ejercicioRepository ?? (_ejercicioRepository = new EjercicioSqlRepository());

        private static IRutinaEjercicioRepository _rutinaEjercicioRepository;
        public static IRutinaEjercicioRepository RutinaEjercicioRepository => _rutinaEjercicioRepository ?? (_rutinaEjercicioRepository = new RutinaEjercicioSqlRepository());

        private static IClienteMembresiaRepository _clienteMembresiaRepository;
        public static IClienteMembresiaRepository ClienteMembresiaRepository => _clienteMembresiaRepository ?? (_clienteMembresiaRepository = new ClienteMembresiaSqlRepository());

        private static IIngresoRepository _ingresoRepository;
        public static IIngresoRepository IngresoRepository => _ingresoRepository ?? (_ingresoRepository = new IngresoSqlRepository());

        private static IComprobanteRepository _comprobanteRepository;
        public static IComprobanteRepository ComprobanteRepository => _comprobanteRepository ?? (_comprobanteRepository = new ComprobanteSqlRepository());
    }
}
