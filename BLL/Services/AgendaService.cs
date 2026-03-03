using BLL.DTOs;
using BLL.Mappers;
using DAL.Contracts;
using DAL.Factory;
using Domain.Entities;
using Service.Helpers;
using Service.Logic;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace BLL.Services
{
    public class AgendaService
    {
        private readonly IAgendaRepository _agendaRepository;
        private readonly BitacoraService _bitacora;

        public AgendaService()
        {
            _agendaRepository = DalFactory.AgendaRepository;
            _bitacora = new BitacoraService();
        }

        public List<AgendaDTO> GetAgendaPorEspacio(Guid espacioId)
        {
            var entities = _agendaRepository.GetByEspacio(espacioId);
            return entities.Select(AgendaMapper.ToDTO).ToList();
        }

        public void ConfigurarAgenda(Guid espacioId, List<AgendaDTO> agendasDto)
        {
            using (var uow = DalFactory.CreateUnitOfWork())
            {
                try
                {
                    uow.BeginTransaction();

                    uow.AgendaRepository.EliminarPorEspacio(espacioId);

                    for (int i = 0; i < agendasDto.Count; i++)
                    {
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

                        var entity = AgendaMapper.ToEntity(agendasDto[i]);
                        entity.EspacioID = espacioId;
                        uow.AgendaRepository.CrearAgenda(entity);
                    }

                    uow.Commit();
                    _bitacora.Log($"Agenda for space {espacioId} configured successfully.", "INFO");
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    _bitacora.Log($"Error configuring agenda: {ex.Message}", "ERROR", ex);
                    throw;
                }
            }
        }
    }
}
