using System;

namespace BLL.DTOs
{
    /// <summary>
    /// Represents a single availability window for a facility space on a given day of the week.
    /// </summary>
    /// <remarks>
    /// Used as the input/output model for <c>AgendaService.ConfigurarAgenda</c>.
    /// A space may have multiple <see cref="AgendaDTO"/> entries per day to model split availability windows.
    /// <see cref="DiaSemana"/> follows the <c>DayOfWeek</c> convention: 0 = Sunday … 6 = Saturday.
    /// Note: windows that cross midnight (i.e., <see cref="HoraHasta"/> &lt; <see cref="HoraDesde"/>) are
    /// not currently supported and will be rejected by the service.
    /// </remarks>
    public class AgendaDTO
    {
        /// <summary>FK to the space whose availability this window defines.</summary>
        public Guid EspacioID { get; set; }
        /// <summary>Day of the week this window applies to (0 = Sunday, 6 = Saturday).</summary>
        public int DiaSemana { get; set; }
        /// <summary>Start of the availability window (inclusive).</summary>
        public TimeSpan HoraDesde { get; set; }
        /// <summary>End of the availability window (exclusive). Must be greater than <see cref="HoraDesde"/>.</summary>
        public TimeSpan HoraHasta { get; set; }
    }
}
