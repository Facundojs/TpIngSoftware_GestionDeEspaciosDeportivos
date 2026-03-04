namespace BLL.DTOs
{
    /// <summary>
    /// Categorized account balance state derived from the numeric balance value.
    /// </summary>
    /// <remarks>
    /// Used by <c>ClienteDTO.EstadoBalance</c> so the UI layer can apply distinct visual
    /// treatments (color coding, icons) without comparing raw decimal values.
    /// </remarks>
    public enum EstadoBalance
    {
        /// <summary>Balance is zero: no outstanding debt and no credit.</summary>
        AlDia,
        /// <summary>Balance is negative: the client has an outstanding debt.</summary>
        Deudor,
        /// <summary>Balance is positive: the client has a credit in their account.</summary>
        AFavor
    }
}
