using Domain.Entities;

namespace DAL.Contracts
{
    /// <summary>
    /// Repository contract for ledger <see cref="Movimiento"/> (movement) records.
    /// </summary>
    /// <remarks>
    /// Movements are append-only; there are no update or delete operations.
    /// The client balance is derived from the aggregate of all movements via the <c>vw_Balance</c> SQL view.
    /// </remarks>
    public interface IMovimientoRepository
    {
        /// <summary>Appends a new ledger movement record.</summary>
        /// <param name="obj">The movement to insert.</param>
        void Insertar(Movimiento obj);
    }
}
