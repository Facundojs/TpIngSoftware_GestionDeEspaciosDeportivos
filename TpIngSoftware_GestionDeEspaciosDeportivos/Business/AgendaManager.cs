using BLL.DTOs;
using BLL.Services;
using System;
using System.Collections.Generic;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Business
{
    /// <summary>
    /// UI-layer facade over <see cref="AgendaService"/> that decouples WinForms forms
    /// from direct BLL service instantiation.
    /// </summary>
    public class AgendaManager
    {
        private readonly AgendaService _agendaService;

        public AgendaManager()
        {
            _agendaService = new AgendaService();
        }

        /// <summary>Returns all availability windows configured for the given space.</summary>
        /// <param name="espacioId">The space identifier.</param>
        public List<AgendaDTO> ObtenerAgendaPorEspacio(Guid espacioId)
        {
            return _agendaService.GetAgendaPorEspacio(espacioId);
        }

        /// <inheritdoc cref="AgendaService.ConfigurarAgenda"/>
        public void ConfigurarAgenda(Guid espacioId, List<AgendaDTO> agendas)
        {
            _agendaService.ConfigurarAgenda(espacioId, agendas);
        }
    }
}
