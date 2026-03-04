namespace BLL.DTOs
{
    /// <summary>
    /// Indicates whether a client account is active or has been deactivated.
    /// </summary>
    /// <remarks>
    /// Inactive clients are blocked from facility entry by <c>ClienteService.ValidarIngreso</c>
    /// and from creating new reservations. The status is persisted as an integer column.
    /// </remarks>
    public enum ClienteStatus : int
    {
        /// <summary>The client account is active and fully operational.</summary>
        Activo = 0,
        /// <summary>The client account has been deactivated. See <c>ClienteDTO.Razon</c> for the reason.</summary>
        Inactivo = 1
    }
}
