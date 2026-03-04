using Domain.Entities;
using System;
using System.Collections.Generic;

namespace DAL.Contracts
{
    public interface IAgendaRepository
    {
        void CrearAgenda(Agenda obj);
        List<Agenda> GetByEspacio(Guid espacioId);
        void EliminarPorEspacio(Guid espacioId);
    }
}
