using DAL.Contracts;
using DAL.Impl;
using DAL.Impl.File;
using DAL.Impl.SqlServer;
using System;

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
        private static IComprobanteRepository _comprobanteFileRepository;
        private static IReservaRepository _reservaRepository;
        private static IIngresoRepository _ingresoRepository;
        private static IClienteMembresiaRepository _clienteMembresiaRepository;

        public static IUnitOfWork CreateUnitOfWork() => new UnitOfWork();

        public static IClienteRepository ClienteRepository =>
            _clienteRepository ?? (_clienteRepository = new ClienteSqlRepository());

        public static IBalanceRepository BalanceRepository =>
            _balanceRepository ?? (_balanceRepository = new BalanceSqlRepository());

        public static IMovimientoRepository MovimientoRepository =>
            _movimientoRepository ?? (_movimientoRepository = new MovimientoSqlRepository());

        public static IMembresiaRepository MembresiaRepository =>
            _membresiaRepository ?? (_membresiaRepository = new MembresiaSqlRepository());

        public static IEspacioRepository EspacioRepository =>
            _espacioRepository ?? (_espacioRepository = new EspacioSqlRepository());

        public static IAgendaRepository AgendaRepository =>
            _agendaRepository ?? (_agendaRepository = new AgendaSqlRepository());

        public static IRutinaRepository RutinaRepository =>
            _rutinaRepository ?? (_rutinaRepository = new RutinaSqlRepository());

        public static IEjercicioRepository EjercicioRepository =>
            _ejercicioRepository ?? (_ejercicioRepository = new EjercicioSqlRepository());

        public static IRutinaEjercicioRepository RutinaEjercicioRepository =>
            _rutinaEjercicioRepository ?? (_rutinaEjercicioRepository = new RutinaEjercicioSqlRepository());

        public static IPagoRepository PagoRepository =>
            _pagoRepository ?? (_pagoRepository = new PagoSqlRepository());

        public static IComprobanteRepository ComprobanteRepository =>
            _comprobanteRepository ?? (_comprobanteRepository = new ComprobanteSqlRepository());

        public static IComprobanteRepository ComprobanteFileRepository =>
            _comprobanteFileRepository ?? (_comprobanteFileRepository = new ComprobanteFileRepository());

        public static IReservaRepository ReservaRepository =>
            _reservaRepository ?? (_reservaRepository = new ReservaSqlRepository());

        public static IIngresoRepository IngresoRepository =>
            _ingresoRepository ?? (_ingresoRepository = new IngresoSqlRepository());

        public static IClienteMembresiaRepository ClienteMembresiaRepository =>
            _clienteMembresiaRepository ?? (_clienteMembresiaRepository = new ClienteMembresiaSqlRepository());
    }
}
