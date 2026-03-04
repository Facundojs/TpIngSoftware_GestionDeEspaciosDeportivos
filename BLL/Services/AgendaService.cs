using BLL.DTOs;
using BLL.Mappers;
using DAL.Contracts;
using DAL.Factory;
using Domain.Entities;
using Domain.Enums;
using Service.Facade.Extension;
using Service.Logic;
using System;
using System.Collections.Generic;

namespace BLL.Services
{
    /// <summary>
    /// Business logic service for configuring and querying a sports space's weekly operating schedule (agenda).
    /// </summary>
    public class AgendaService
    {
        private readonly IAgendaRepository _agendaRepo;
        private readonly BitacoraService _bitacora;

        /// <summary>Initializes dependencies from <see cref="DAL.Factory.DalFactory"/> singletons.</summary>
        public AgendaService()
        {
            _agendaRepo = DalFactory.AgendaRepository;
            _bitacora = new BitacoraService();
        }

        /// <summary>
        /// Returns all agenda blocks configured for the specified space.
        /// </summary>
        /// <param name="espacioId">The space to query.</param>
        /// <returns>List of <see cref="AgendaDTO"/> records; empty list if no schedule is configured.</returns>
        public List<AgendaDTO> GetAgendaPorEspacio(Guid espacioId)
        {
            var agendas = _agendaRepo.GetByEspacio(espacioId);
            return AgendaMapper.Map(agendas);
        }

        /// <summary>
        /// Atomically replaces the entire agenda of a space by deleting all existing blocks
        /// and inserting the new set in a single transaction.
        /// </summary>
        /// <param name="espacioId">The space whose agenda will be replaced.</param>
        /// <param name="agendasDto">
        /// The new schedule blocks. Each block's <c>HoraDesde</c> must be earlier than <c>HoraHasta</c>.
        /// Blocks on the same day must not overlap.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="agendasDto"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// Thrown when a block has <c>HoraDesde >= HoraHasta</c>, or when two blocks on the same day overlap.
        /// </exception>
        /// <remarks>
        /// Known limitation: Agenda ranges crossing midnight (e.g., 22:00-02:00) are explicitly prevented by the UI validation here. They must be split into two ranges: 22:00-23:59 and 00:00-02:00.
        /// </remarks>
        /// <summary>
        /// Replaces the entire weekly schedule for a space within a single transaction.
        /// All existing agenda blocks are deleted and the new set is inserted atomically.
        /// </summary>
        /// <param name="espacioId">The space whose schedule is being configured.</param>
        /// <param name="agendasDto">New schedule blocks. Must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="agendasDto"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// Thrown if any block has <c>HoraDesde &gt;= HoraHasta</c>, or if two blocks for the same
        /// day overlap. Ranges crossing midnight are not supported and must be split.
        /// </exception>
        public void ConfigurarAgenda(Guid espacioId, List<AgendaDTO> agendasDto)
        {
            if (agendasDto == null) throw new ArgumentNullException(nameof(agendasDto));

            for (int i = 0; i < agendasDto.Count; i++)
            {
                // Known limitation: Agenda ranges crossing midnight (e.g., 22:00-02:00) are explicitly prevented by the UI validation here. They must be split into two ranges: 22:00-23:59 and 00:00-02:00.
                if (agendasDto[i].HoraDesde >= agendasDto[i].HoraHasta)
                {
                    throw new ArgumentException(Translations.ERR_HORA_DESDE_MAYOR.Translate());
                }
                for (int j = i + 1; j < agendasDto.Count; j++)
                {
                    if (agendasDto[i].DiaSemana == agendasDto[j].DiaSemana &&
                        agendasDto[i].HoraDesde < agendasDto[j].HoraHasta &&
                        agendasDto[j].HoraDesde < agendasDto[i].HoraHasta)
                    {
                        throw new ArgumentException(Translations.ERR_AGENDA_OVERLAP.Translate());
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
