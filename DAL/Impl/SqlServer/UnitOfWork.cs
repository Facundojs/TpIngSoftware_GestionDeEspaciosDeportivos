using DAL.Contracts;
using DAL.Impl;
using Service.Helpers;
using System;
using System.Data.SqlClient;

namespace DAL.Impl.SqlServer
{
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

        public void Commit()
        {
            if (_transaction == null) throw new InvalidOperationException("No active transaction to commit.");
            _transaction.Commit();
            Cleanup();
            _spent = true;
        }

        public void Rollback()
        {
            if (_transaction == null) return;
            _transaction.Rollback();
            Cleanup();
            _spent = true;
        }

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

        public IAgendaRepository AgendaRepository => _agendaRepository;
        public IClienteRepository ClienteRepository => _clienteRepository;
        public IPagoRepository PagoRepository => _pagoRepository;
        public IReservaRepository ReservaRepository => _reservaRepository;
        public IRutinaRepository RutinaRepository => _rutinaRepository;
        public IMovimientoRepository MovimientoRepository => _movimientoRepository;
        public IClienteMembresiaRepository ClienteMembresiaRepository => _clienteMembresiaRepository;
        public IRutinaEjercicioRepository RutinaEjercicioRepository => _rutinaEjercicioRepository;
        public IMembresiaRepository MembresiaRepository => _membresiaRepository;
        public IEjercicioRepository EjercicioRepository => _ejercicioRepository;
        public IEspacioRepository EspacioRepository => _espacioRepository;
    }
}
