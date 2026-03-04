using DAL.Contracts;
using DAL.Impl;
using DAL.Impl.File;
using DAL.Impl.SqlServer;
using System;

namespace DAL.Factory
{
    /// <summary>
    /// Static service locator / factory for all repository singletons.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each property follows a lazy-initialization pattern (<c>_field ?? (_field = new ImplClass())</c>),
    /// so repository instances are created on first access and reused thereafter within the process lifetime.
    /// </para>
    /// <para>
    /// For single-repository (read-only) operations, use the singleton properties directly.
    /// For multi-repository write operations that require atomicity, use
    /// <see cref="CreateUnitOfWork"/> to obtain a fresh <see cref="IUnitOfWork"/> instance that
    /// shares one connection and transaction across all enrolled repositories.
    /// </para>
    /// </remarks>
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

        /// <summary>Creates and returns a new <see cref="IUnitOfWork"/> instance for transactional, multi-repository operations.</summary>
        public static IUnitOfWork CreateUnitOfWork() => new UnitOfWork();

        /// <summary>Singleton client repository.</summary>
        public static IClienteRepository ClienteRepository =>
            _clienteRepository ?? (_clienteRepository = new ClienteSqlRepository());

        /// <summary>Singleton balance repository (backed by <c>vw_Balance</c> SQL view).</summary>
        public static IBalanceRepository BalanceRepository =>
            _balanceRepository ?? (_balanceRepository = new BalanceSqlRepository());

        /// <summary>Singleton ledger movement repository.</summary>
        public static IMovimientoRepository MovimientoRepository =>
            _movimientoRepository ?? (_movimientoRepository = new MovimientoSqlRepository());

        /// <summary>Singleton membership plan repository.</summary>
        public static IMembresiaRepository MembresiaRepository =>
            _membresiaRepository ?? (_membresiaRepository = new MembresiaSqlRepository());

        /// <summary>Singleton sports space repository.</summary>
        public static IEspacioRepository EspacioRepository =>
            _espacioRepository ?? (_espacioRepository = new EspacioSqlRepository());

        /// <summary>Singleton agenda (operating schedule) repository.</summary>
        public static IAgendaRepository AgendaRepository =>
            _agendaRepository ?? (_agendaRepository = new AgendaSqlRepository());

        /// <summary>Singleton workout routine repository.</summary>
        public static IRutinaRepository RutinaRepository =>
            _rutinaRepository ?? (_rutinaRepository = new RutinaSqlRepository());

        /// <summary>Singleton exercise catalog repository.</summary>
        public static IEjercicioRepository EjercicioRepository =>
            _ejercicioRepository ?? (_ejercicioRepository = new EjercicioSqlRepository());

        /// <summary>Singleton routine-exercise mapping repository.</summary>
        public static IRutinaEjercicioRepository RutinaEjercicioRepository =>
            _rutinaEjercicioRepository ?? (_rutinaEjercicioRepository = new RutinaEjercicioSqlRepository());

        /// <summary>Singleton payment repository.</summary>
        public static IPagoRepository PagoRepository =>
            _pagoRepository ?? (_pagoRepository = new PagoSqlRepository());

        /// <summary>Singleton receipt repository (SQL Server storage).</summary>
        public static IComprobanteRepository ComprobanteRepository =>
            _comprobanteRepository ?? (_comprobanteRepository = new ComprobanteSqlRepository());

        /// <summary>Singleton receipt repository (file system storage).</summary>
        public static IComprobanteRepository ComprobanteFileRepository =>
            _comprobanteFileRepository ?? (_comprobanteFileRepository = new ComprobanteFileRepository());

        /// <summary>Singleton reservation repository.</summary>
        public static IReservaRepository ReservaRepository =>
            _reservaRepository ?? (_reservaRepository = new ReservaSqlRepository());

        /// <summary>Singleton facility check-in (ingreso) repository.</summary>
        public static IIngresoRepository IngresoRepository =>
            _ingresoRepository ?? (_ingresoRepository = new IngresoSqlRepository());

        /// <summary>Singleton client-membership assignment repository.</summary>
        public static IClienteMembresiaRepository ClienteMembresiaRepository =>
            _clienteMembresiaRepository ?? (_clienteMembresiaRepository = new ClienteMembresiaSqlRepository());
    }
}
