using System;

namespace DAL.Contracts
{
    /// <summary>
    /// Defines a transactional scope that shares a single database connection across multiple repositories.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Obtain a new instance from <see cref="DAL.Factory.DalFactory.CreateUnitOfWork"/>.
    /// The intended usage pattern is:
    /// <code>
    /// using (var uow = DalFactory.CreateUnitOfWork())
    /// {
    ///     uow.BeginTransaction();
    ///     // use uow.*Repository to perform coordinated writes
    ///     uow.Commit();
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// If <see cref="Commit"/> is never called, <see cref="IDisposable.Dispose"/> will automatically
    /// roll back any open transaction when the <c>using</c> block exits.
    /// </para>
    /// </remarks>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>Opens the shared database connection and begins a transaction.</summary>
        void BeginTransaction();

        /// <summary>Commits all pending changes and closes the connection.</summary>
        void Commit();

        /// <summary>Discards all pending changes and closes the connection.</summary>
        void Rollback();

        /// <summary>Repository for agenda (operating schedule) operations.</summary>
        IAgendaRepository AgendaRepository { get; }

        /// <summary>Repository for client operations.</summary>
        IClienteRepository ClienteRepository { get; }

        /// <summary>Repository for payment operations.</summary>
        IPagoRepository PagoRepository { get; }

        /// <summary>Repository for reservation operations.</summary>
        IReservaRepository ReservaRepository { get; }

        /// <summary>Repository for workout routine operations.</summary>
        IRutinaRepository RutinaRepository { get; }

        /// <summary>Repository for ledger movement (debit/credit) insertions.</summary>
        IMovimientoRepository MovimientoRepository { get; }

        /// <summary>Repository for client-membership assignment operations.</summary>
        IClienteMembresiaRepository ClienteMembresiaRepository { get; }

        /// <summary>Repository for routine-exercise mapping operations.</summary>
        IRutinaEjercicioRepository RutinaEjercicioRepository { get; }

        /// <summary>Repository for membership plan operations.</summary>
        IMembresiaRepository MembresiaRepository { get; }

        /// <summary>Repository for exercise catalog operations.</summary>
        IEjercicioRepository EjercicioRepository { get; }

        /// <summary>Repository for sports space operations.</summary>
        IEspacioRepository EspacioRepository { get; }
    }
}
