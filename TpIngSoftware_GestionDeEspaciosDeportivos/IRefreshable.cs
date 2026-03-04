namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    /// <summary>
    /// Implemented by UI forms and user controls that can reload their displayed data on demand.
    /// </summary>
    /// <remarks>
    /// Called by parent forms after write operations (create, update, delete) complete
    /// so that child panels reflect the latest state without a full form re-open.
    /// </remarks>
    public interface IRefreshable
    {
        /// <summary>Reloads data from the BLL layer and rebinds all UI controls.</summary>
        void RefreshData();
    }
}
