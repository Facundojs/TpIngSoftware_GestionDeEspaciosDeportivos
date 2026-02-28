using BLL.DTOs;
using BLL.Services;
using System;
using System.Collections.Generic;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Business
{
    public class AgendaManager
    {
        private readonly AgendaService _agendaService;

        public AgendaManager()
        {
            _agendaService = new AgendaService();
        }

        public List<AgendaDTO> ObtenerAgendaPorEspacio(Guid espacioId)
        {
            return _agendaService.GetAgendaPorEspacio(espacioId);
        }

        public void ConfigurarAgenda(Guid espacioId, List<AgendaDTO> agendas)
        {
            _agendaService.ConfigurarAgenda(espacioId, agendas);
        }
    }
}
