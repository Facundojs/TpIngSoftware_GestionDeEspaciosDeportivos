using Domain.Entities;

namespace DAL.Contracts
{
    public interface IMovimientoRepository
    {
        void Insertar(Movimiento obj);
    }
}
