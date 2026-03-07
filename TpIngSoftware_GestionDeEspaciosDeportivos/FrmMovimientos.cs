using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BLL.DTOs;
using TpIngSoftware_GestionDeEspaciosDeportivos.Business;
using Service.Facade.Extension;
using Domain.Entities;
using Domain.Enums;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmMovimientos : Form
    {
        private readonly BalanceManager _balanceManager;
        private readonly ClienteDTO _cliente;

        public FrmMovimientos(ClienteDTO cliente)
        {
            InitializeComponent();
            _cliente = cliente;
            _balanceManager = new BalanceManager();
        }

        private void FrmMovimientos_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            UpdateLanguage();

            // Default filter: last 30 days to today
            dtpDesde.Value = DateTime.Now.AddDays(-30);
            dtpHasta.Value = DateTime.Now;

            CargarMovimientos();
        }

        private void ConfigurarGrid()
        {
            dgvMovimientos.AutoGenerateColumns = false;
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Monto", HeaderText = "LBL_MONTO".Translate() });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Tipo", HeaderText = "LBL_TIPO_MOVIMIENTO".Translate() });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Fecha", HeaderText = "LBL_FECHA".Translate() });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Descripcion", HeaderText = "LBL_DESCRIPCION".Translate() });
        }

        private void UpdateLanguage()
        {
            this.Text = "FRM_MOVIMIENTOS_TITLE".Translate();
            lblCliente.Text = $"{"LBL_CLIENTE".Translate()}: {_cliente.NombreCompleto}";
            lblDesde.Text = "LBL_DATE_FROM".Translate();
            lblHasta.Text = "LBL_DATE_TO".Translate();
            btnFiltrar.Text = "BTN_FILTER".Translate();

            if (dgvMovimientos.Columns.Count > 0)
            {
                dgvMovimientos.Columns[0].HeaderText = "LBL_MONTO".Translate();
                dgvMovimientos.Columns[1].HeaderText = "LBL_TIPO_MOVIMIENTO".Translate();
                dgvMovimientos.Columns[2].HeaderText = "LBL_FECHA".Translate();
                dgvMovimientos.Columns[3].HeaderText = "LBL_DESCRIPCION".Translate();
            }
        }

        private void CargarMovimientos()
        {
            try
            {
                DateTime? desde = chkFiltroFechas.Checked ? (DateTime?)dtpDesde.Value.Date : null;
                DateTime? hasta = chkFiltroFechas.Checked ? (DateTime?)dtpHasta.Value.Date.AddDays(1).AddSeconds(-1) : null;

                var movimientos = _balanceManager.ListarMovimientos(_cliente.Id, desde, hasta);
                dgvMovimientos.DataSource = movimientos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("MSG_ERR_GENERIC".Translate() + " " + ex.Message, "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            CargarMovimientos();
        }

        private void chkFiltroFechas_CheckedChanged(object sender, EventArgs e)
        {
            dtpDesde.Enabled = chkFiltroFechas.Checked;
            dtpHasta.Enabled = chkFiltroFechas.Checked;
        }
    }
}
