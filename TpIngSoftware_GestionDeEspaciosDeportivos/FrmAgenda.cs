using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using BLL.DTOs;
using TpIngSoftware_GestionDeEspaciosDeportivos.Business;
using Service.Facade.Extension;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmAgenda : Form
    {
        private readonly AgendaManager _agendaManager;
        private readonly Guid _espacioId;
        private BindingList<AgendaDTO> _agendas;

        public FrmAgenda(Guid espacioId)
        {
            InitializeComponent();
            _agendaManager = new AgendaManager();
            _espacioId = espacioId;
            _agendas = new BindingList<AgendaDTO>();

            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            this.Text = Domain.Enums.Translations.FRM_AGENDA_TITLE.Translate();
            lblDesde.Text = Domain.Enums.Translations.LBL_HORA_DESDE.Translate();
            lblHasta.Text = Domain.Enums.Translations.LBL_HORA_HASTA.Translate();
            btnAgregar.Text = Domain.Enums.Translations.BTN_AGREGAR.Translate();
            btnEliminar.Text = Domain.Enums.Translations.BTN_ELIMINAR.Translate();
            btnGuardar.Text = Domain.Enums.Translations.BTN_SAVE.Translate();
        }

        private void FrmAgenda_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var list = _agendaManager.ObtenerAgendaPorEspacio(_espacioId);
                _agendas = new BindingList<AgendaDTO>(list);
                dgvAgenda.DataSource = _agendas;

                if (dgvAgenda.Columns["EspacioID"] != null) dgvAgenda.Columns["EspacioID"].Visible = false;
                if (dgvAgenda.Columns["HoraDesde"] != null) dgvAgenda.Columns["HoraDesde"].HeaderText = Domain.Enums.Translations.LBL_HORA_DESDE.Translate();
                if (dgvAgenda.Columns["HoraHasta"] != null) dgvAgenda.Columns["HoraHasta"].HeaderText = Domain.Enums.Translations.LBL_HORA_HASTA.Translate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading agenda: " + ex.Message);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            TimeSpan desde = new TimeSpan(dtpDesde.Value.Hour, dtpDesde.Value.Minute, 0);
            TimeSpan hasta = new TimeSpan(dtpHasta.Value.Hour, dtpHasta.Value.Minute, 0);

            if (desde >= hasta)
            {
                MessageBox.Show("La hora 'Desde' debe ser menor a la hora 'Hasta'");
                return;
            }

            var nueva = new AgendaDTO
            {
                EspacioID = _espacioId,
                HoraDesde = desde,
                HoraHasta = hasta
            };

            _agendas.Add(nueva);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvAgenda.SelectedRows.Count > 0)
            {
                var selected = (AgendaDTO)dgvAgenda.SelectedRows[0].DataBoundItem;
                _agendas.Remove(selected);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                var list = _agendas.ToList();
                _agendaManager.ConfigurarAgenda(_espacioId, list);
                MessageBox.Show(Domain.Enums.Translations.MSG_AGENDA_UPDATED.Translate());
                this.Close();
            }
            catch (ArgumentException ex) when (ex.Message == "ERR_AGENDA_OVERLAP")
            {
                MessageBox.Show(Domain.Enums.Translations.ERR_AGENDA_OVERLAP.Translate());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
