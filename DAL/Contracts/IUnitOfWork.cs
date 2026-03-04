using System;

namespace DAL.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTransaction();
        void Commit();
        void Rollback();

        IAgendaRepository AgendaRepository { get; }
        IClienteRepository ClienteRepository { get; }
        IPagoRepository PagoRepository { get; }
        IReservaRepository ReservaRepository { get; }
        IRutinaRepository RutinaRepository { get; }
        IMovimientoRepository MovimientoRepository { get; }
        IClienteMembresiaRepository ClienteMembresiaRepository { get; }
        IRutinaEjercicioRepository RutinaEjercicioRepository { get; }
        IMembresiaRepository MembresiaRepository { get; }
        IEjercicioRepository EjercicioRepository { get; }
        IEspacioRepository EspacioRepository { get; }
    }
}
