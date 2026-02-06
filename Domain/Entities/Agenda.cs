using System;

namespace Domain.Entities
{
    public class Agenda
    {
        public Guid EspacioID { get; set; }
        public TimeSpan HoraDesde { get; set; }
        public TimeSpan HoraHasta { get; set; }
    }
}
