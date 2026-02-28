using System;

namespace BLL.DTOs
{
    public class AgendaDTO
    {
        public Guid EspacioID { get; set; }
        public TimeSpan HoraDesde { get; set; }
        public TimeSpan HoraHasta { get; set; }
    }
}
