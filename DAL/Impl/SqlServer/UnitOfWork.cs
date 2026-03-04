using DAL.Contracts;
using DAL.Impl;
using Service.Helpers;
using System;
using System.Data.SqlClient;

namespace DAL.Impl.SqlServer
{
    /// <summary>
    /// Implements the Unit of Work pattern for SQL Server.
    /// Manages a single <see cref="SqlConnection"/> and <see cref="SqlTransaction"/> shared across
    /// all enrolled repositories for the duration of one logical operation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Obtain an instance via <see cref="DAL.Factory.DalFactory.CreateUnitOfWork"/>.
    /// Usage pattern:
    /// <code>
    /// using (var uow = DalFactory.CreateUnitOfWork())
    /// {
    ///     uow.BeginTransaction();
    ///     // ...operations on uow.*Repository...
    ///     uow.Commit();
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// After <see cref="Commit"/> or <see cref="Rollback"/>, the instance is <em>spent</em>
    /// and cannot be reused; <see cref="BeginTransaction"/> will throw <see cref="ObjectDisposedException"/>.
    /// <see cref="Dispose"/> will roll back any uncommitted transaction if the <c>using</c> block
    /// exits without an explicit commit.
    /// </para>
    /// </remarks>
    public class UnitOfWork : IUnitOfWork
    {
        private SqlConnection _connection;
        private SqlTransaction _transaction;
        private bool _disposed;
        private bool _spent;

        private readonly AgendaSqlRepository _agendaRepository;
        private readonly ClienteSqlRepository _clienteRepository;
        private readonly PagoSqlRepository _pagoRepository;
        private readonly ReservaSqlRepository _reservaRepository;
        private readonly RutinaSqlRepository _rutinaRepository;
        private readonly MovimientoSqlRepository _movimientoRepository;
        private readonly ClienteMembresiaSqlRepository _clienteMembresiaRepository;
        private readonly RutinaEjercicioSqlRepository _rutinaEjercicioRepository;
        private readonly MembresiaSqlRepository _membresiaRepository;
        private readonly EjercicioSqlRepository _ejercicioRepository;
        private readonly EspacioSqlRepository _espacioRepository;

        /// <summary>Constructs all repository instances without opening a connection.</summary>
        public UnitOfWork()
        {
            _agendaRepository = new AgendaSqlRepository();
            _clienteRepository = new ClienteSqlRepository();
            _pagoRepository = new PagoSqlRepository();
            _reservaRepository = new ReservaSqlRepository();
            _rutinaRepository = new RutinaSqlRepository();
            _movimientoRepository = new MovimientoSqlRepository();
            _clienteMembresiaRepository = new ClienteMembresiaSqlRepository();
            _rutinaEjercicioRepository = new RutinaEjercicioSqlRepository();
            _membresiaRepository = new MembresiaSqlRepository();
            _ejercicioRepository = new EjercicioSqlRepository();
            _espacioRepository = new EspacioSqlRepository();
        }

        /// <summary>
        /// Opens the shared connection and begins a database transaction, injecting both into all repositories.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the instance has already been disposed or spent.</exception>
        /// <exception cref="InvalidOperationException">Thrown if a transaction is already active on this instance.</exception>
        public void BeginTransaction()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(UnitOfWork));
            if (_spent) throw new ObjectDisposedException(nameof(UnitOfWork));
            if (_transaction != null) throw new InvalidOperationException("A transaction is already active.");

            _connection = new SqlConnection(ConnectionManager.GetBusinessConnectionString());
            _connection.Open();
            _transaction = _connection.BeginTransaction();

            _agendaRepository.CurrentConnection = _connection;
            _agendaRepository.CurrentTransaction = _transaction;
            _clienteRepository.CurrentConnection = _connection;
            _clienteRepository.CurrentTransaction = _transaction;
            _pagoRepository.CurrentConnection = _connection;
            _pagoRepository.CurrentTransaction = _transaction;
            _reservaRepository.CurrentConnection = _connection;
            _reservaRepository.CurrentTransaction = _transaction;
            _rutinaRepository.CurrentConnection = _connection;
            _rutinaRepository.CurrentTransaction = _transaction;
            _movimientoRepository.CurrentConnection = _connection;
            _movimientoRepository.CurrentTransaction = _transaction;
            _clienteMembresiaRepository.CurrentConnection = _connection;
            _clienteMembresiaRepository.CurrentTransaction = _transaction;
            _rutinaEjercicioRepository.CurrentConnection = _connection;
            _rutinaEjercicioRepository.CurrentTransaction = _transaction;
            _membresiaRepository.CurrentConnection = _connection;
            _membresiaRepository.CurrentTransaction = _transaction;
            _ejercicioRepository.CurrentConnection = _connection;
            _ejercicioRepository.CurrentTransaction = _transaction;
            _espacioRepository.CurrentConnection = _connection;
            _espacioRepository.CurrentTransaction = _transaction;
        }

        /// <summary>Commits the active transaction and closes the shared connection.</summary>
        /// <exception cref="InvalidOperationException">Thrown if no transaction is active.</exception>
        public void Commit()
        {
            if (_transaction == null) throw new InvalidOperationException("No active transaction to commit.");
            _transaction.Commit();
            Cleanup();
            _spent = true;
        }

        /// <summary>
        /// Rolls back the active transaction and closes the shared connection.
        /// Safe to call even when no transaction is active (no-op in that case).
        /// </summary>
        public void Rollback()
        {
            if (_transaction == null) return;
            _transaction.Rollback();
            Cleanup();
            _spent = true;
        }

        /// <summary>
        /// Rolls back any uncommitted transaction and releases all resources.
        /// Called automatically at the end of a <c>using</c> block.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (_transaction != null)
            {
                try { _transaction.Rollback(); } catch { }
            }
            Cleanup();
        }

        private void Cleanup()
        {
            _transaction?.Dispose();
            _transaction = null;
            _connection?.Close();
            _connection?.Dispose();
            _connection = null;
        }

        /// <inheritdoc/>
        public IAgendaRepository AgendaRepository => _agendaRepository;
        /// <inheritdoc/>
        public IClienteRepository ClienteRepository => _clienteRepository;
        /// <inheritdoc/>
        public IPagoRepository PagoRepository => _pagoRepository;
        /// <inheritdoc/>
        public IReservaRepository ReservaRepository => _reservaRepository;
        /// <inheritdoc/>
        public IRutinaRepository RutinaRepository => _rutinaRepository;
        /// <inheritdoc/>
        public IMovimientoRepository MovimientoRepository => _movimientoRepository;
        /// <inheritdoc/>
        public IClienteMembresiaRepository ClienteMembresiaRepository => _clienteMembresiaRepository;
        /// <inheritdoc/>
        public IRutinaEjercicioRepository RutinaEjercicioRepository => _rutinaEjercicioRepository;
        /// <inheritdoc/>
        public IMembresiaRepository MembresiaRepository => _membresiaRepository;
        /// <inheritdoc/>
        public IEjercicioRepository EjercicioRepository => _ejercicioRepository;
        /// <inheritdoc/>
        public IEspacioRepository EspacioRepository => _espacioRepository;
    }
}
