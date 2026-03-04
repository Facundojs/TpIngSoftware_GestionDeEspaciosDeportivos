using BLL.DTOs;
using BLL.Mappers;
using DAL.Contracts;
using DAL.Factory;
using Domain.Entities;
using Service.Logic;
using System;
using System.Collections.Generic;

namespace BLL.Services
{
    public class AgendaService
    {
        private readonly IAgendaRepository _agendaRepo;
        private readonly BitacoraService _bitacora;

        public AgendaService()
        {
            _agendaRepo = DalFactory.AgendaRepository;
            _bitacora = new BitacoraService();
        }

        public List<AgendaDTO> GetAgendaPorEspacio(Guid espacioId)
        {
            var agendas = _agendaRepo.GetByEspacio(espacioId);
            return AgendaMapper.Map(agendas);
        }

        public void ConfigurarAgenda(Guid espacioId, List<AgendaDTO> agendasDto)
        {
            if (agendasDto == null) throw new ArgumentNullException(nameof(agendasDto));

            // Validate no overlaps within the new list
            for (int i = 0; i < agendasDto.Count; i++)
            {
                // Known limitation: Agenda ranges crossing midnight (e.g., 22:00-02:00) are explicitly prevented by the UI validation here. They must be split into two ranges: 22:00-23:59 and 00:00-02:00.
                if (agendasDto[i].HoraDesde >= agendasDto[i].HoraHasta)
                {
                    throw new ArgumentException(Domain.Enums.Translations.ERR_HORA_DESDE_MAYOR.Translate());
                }
                for (int j = i + 1; j < agendasDto.Count; j++)
                {
                    if (agendasDto[i].DiaSemana == agendasDto[j].DiaSemana &&
                        agendasDto[i].HoraDesde < agendasDto[j].HoraHasta &&
                        agendasDto[j].HoraDesde < agendasDto[i].HoraHasta)
                    {
                        throw new ArgumentException(Domain.Enums.Translations.ERR_AGENDA_OVERLAP.Translate());
                    }
                }
            }

            var entities = AgendaMapper.Map(agendasDto);

            try
            {
                using (var uow = DalFactory.CreateUnitOfWork())
                {
                    try
                    {
                        uow.BeginTransaction();

                        uow.AgendaRepository.EliminarPorEspacio(espacioId);

                        foreach (var agenda in entities)
                        {
                            agenda.EspacioID = espacioId;
                            uow.AgendaRepository.CrearAgenda(agenda);
                        }

                        uow.Commit();
                        _bitacora.Log($"Agenda configurada para el espacio {espacioId}.", "INFO");
                    }
                    catch
                    {
                        uow.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error configurando agenda: {ex.Message}", "ERROR", ex);
                throw;
            }
        }
    }
}
