using DAL.Contracts;
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

        public UnitOfWork()
        {
            _connection = new SqlConnection(ConnectionManager.GetBusinessConnectionString());
            _connection.Open();

            AgendaRepository = CreateRepo<AgendaSqlRepository>();
            ClienteRepository = CreateRepo<ClienteSqlRepository>();
            PagoRepository = CreateRepo<PagoSqlRepository>();
            ReservaRepository = CreateRepo<ReservaSqlRepository>();
            RutinaRepository = CreateRepo<RutinaSqlRepository>();
            MovimientoRepository = CreateRepo<MovimientoSqlRepository>();
            ClienteMembresiaRepository = CreateRepo<ClienteMembresiaSqlRepository>();
            RutinaEjercicioRepository = CreateRepo<RutinaEjercicioSqlRepository>();
            MembresiaRepository = CreateRepo<MembresiaSqlRepository>();
            EjercicioRepository = CreateRepo<EjercicioSqlRepository>();
            EspacioRepository = CreateRepo<EspacioSqlRepository>();
        }

        private T CreateRepo<T>() where T : BaseBusinessSqlRepository, new()
        {
            var repo = new T();
            repo.CurrentConnection = _connection;
            return repo;
        }

        public IAgendaRepository AgendaRepository { get; private set; }
        public IClienteRepository ClienteRepository { get; private set; }
        public IPagoRepository PagoRepository { get; private set; }
        public IReservaRepository ReservaRepository { get; private set; }
        public IRutinaRepository RutinaRepository { get; private set; }
        public IMovimientoRepository MovimientoRepository { get; private set; }
        public IClienteMembresiaRepository ClienteMembresiaRepository { get; private set; }
        public IRutinaEjercicioRepository RutinaEjercicioRepository { get; private set; }
        public IMembresiaRepository MembresiaRepository { get; private set; }
        public IEjercicioRepository EjercicioRepository { get; private set; }
        public IEspacioRepository EspacioRepository { get; private set; }

        public void BeginTransaction()
        {
            _transaction = _connection.BeginTransaction();
            UpdateRepoTransaction(AgendaRepository);
            UpdateRepoTransaction(ClienteRepository);
            UpdateRepoTransaction(PagoRepository);
            UpdateRepoTransaction(ReservaRepository);
            UpdateRepoTransaction(RutinaRepository);
            UpdateRepoTransaction(MovimientoRepository);
            UpdateRepoTransaction(ClienteMembresiaRepository);
            UpdateRepoTransaction(RutinaEjercicioRepository);
            UpdateRepoTransaction(MembresiaRepository);
            UpdateRepoTransaction(EjercicioRepository);
            UpdateRepoTransaction(EspacioRepository);
        }

        private void UpdateRepoTransaction(object repo)
        {
            if (repo is BaseBusinessSqlRepository sqlRepo)
                sqlRepo.CurrentTransaction = _transaction;
        }

        public void Commit()
        {
            _transaction?.Commit();
            _transaction = null;
            ResetTransactions();
        }

        public void Rollback()
        {
            _transaction?.Rollback();
            _transaction = null;
            ResetTransactions();
        }

        private void ResetTransactions()
        {
            UpdateRepoTransaction(AgendaRepository);
            UpdateRepoTransaction(ClienteRepository);
            UpdateRepoTransaction(PagoRepository);
            UpdateRepoTransaction(ReservaRepository);
            UpdateRepoTransaction(RutinaRepository);
            UpdateRepoTransaction(MovimientoRepository);
            UpdateRepoTransaction(ClienteMembresiaRepository);
            UpdateRepoTransaction(RutinaEjercicioRepository);
            UpdateRepoTransaction(MembresiaRepository);
            UpdateRepoTransaction(EjercicioRepository);
            UpdateRepoTransaction(EspacioRepository);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _transaction?.Dispose();
                _connection?.Close();
                _connection?.Dispose();
                _disposed = true;
            }
        }
    }
}
