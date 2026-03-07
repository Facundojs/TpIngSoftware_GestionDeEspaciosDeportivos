using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using BLL.DTOs;
using TpIngSoftware_GestionDeEspaciosDeportivos.Business;
using Service.Facade.Extension;
using Domain.Enums;

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
            this.Text = "FRM_AGENDA_TITLE".Translate();
            lblDesde.Text = "LBL_HORA_DESDE".Translate();
            lblHasta.Text = "LBL_HORA_HASTA".Translate();
            btnAgregar.Text = "BTN_AGREGAR".Translate();
            btnEliminar.Text = "BTN_ELIMINAR".Translate();
            btnGuardar.Text = "BTN_SAVE".Translate();
            lblDiaSemana.Text = "LBL_EJERCICIO_DIA".Translate();
        }

        private void FrmAgenda_Load(object sender, EventArgs e)
        {
            cmbDiaSemana.Items.Clear();
            cmbDiaSemana.Items.Add(new KeyValuePair<int, string>(0, "DAY_0".Translate()));
            cmbDiaSemana.Items.Add(new KeyValuePair<int, string>(1, "DAY_1".Translate()));
            cmbDiaSemana.Items.Add(new KeyValuePair<int, string>(2, "DAY_2".Translate()));
            cmbDiaSemana.Items.Add(new KeyValuePair<int, string>(3, "DAY_3".Translate()));
            cmbDiaSemana.Items.Add(new KeyValuePair<int, string>(4, "DAY_4".Translate()));
            cmbDiaSemana.Items.Add(new KeyValuePair<int, string>(5, "DAY_5".Translate()));
            cmbDiaSemana.Items.Add(new KeyValuePair<int, string>(6, "DAY_6".Translate()));

            cmbDiaSemana.DisplayMember = "Value";
            cmbDiaSemana.ValueMember = "Key";
            cmbDiaSemana.SelectedIndex = 0;

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
                if (dgvAgenda.Columns["DiaSemana"] != null) dgvAgenda.Columns["DiaSemana"].HeaderText = "LBL_EJERCICIO_DIA".Translate();
                if (dgvAgenda.Columns["HoraDesde"] != null) dgvAgenda.Columns["HoraDesde"].HeaderText = "LBL_HORA_DESDE".Translate();
                if (dgvAgenda.Columns["HoraHasta"] != null) dgvAgenda.Columns["HoraHasta"].HeaderText = "LBL_HORA_HASTA".Translate();

                dgvAgenda.CellFormatting -= DgvAgenda_CellFormatting;
                dgvAgenda.CellFormatting += DgvAgenda_CellFormatting;
            }
            catch (Exception ex)
            {
                MessageBox.Show("MSG_ERR_GENERIC".Translate() + " " + ex.Message, "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvAgenda_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvAgenda.Columns[e.ColumnIndex].Name == "DiaSemana" && e.Value is int diaInt)
            {
                // Use translations from the project
                string translationKey = $"DAY_{diaInt}";
                e.Value = translationKey.Translate();
                e.FormattingApplied = true;
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            var selectedDay = (KeyValuePair<int, string>)cmbDiaSemana.SelectedItem;
            int diaSemana = selectedDay.Key;

            TimeSpan desde = new TimeSpan(dtpDesde.Value.Hour, dtpDesde.Value.Minute, 0);
            TimeSpan hasta = new TimeSpan(dtpHasta.Value.Hour, dtpHasta.Value.Minute, 0);

            if (desde.Minutes != 0 && desde.Minutes != 30)
            {
                MessageBox.Show("ERR_HORA_MULTIPLO_30".Translate(), "TITLE_WARNING".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (hasta.Minutes != 0 && hasta.Minutes != 30)
            {
                MessageBox.Show("ERR_HORA_MULTIPLO_30".Translate(), "TITLE_WARNING".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (desde >= hasta)
            {
                MessageBox.Show("ERR_HORA_DESDE_MAYOR".Translate(), "TITLE_WARNING".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nueva = new AgendaDTO
            {
                EspacioID = _espacioId,
                DiaSemana = diaSemana,
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
                MessageBox.Show("MSG_AGENDA_UPDATED".Translate(), "TITLE_INFO".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (ArgumentException ex) when (ex.Message == "ERR_AGENDA_OVERLAP")
            {
                MessageBox.Show("ERR_AGENDA_OVERLAP".Translate(), "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("MSG_ERR_GENERIC".Translate() + " " + ex.Message, "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
