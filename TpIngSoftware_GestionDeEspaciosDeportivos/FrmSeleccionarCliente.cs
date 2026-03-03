using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BLL.DTOs;
using Service.Facade.Extension;
using TpIngSoftware_GestionDeEspaciosDeportivos.Business;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmSeleccionarCliente : Form
    {
        private readonly ClienteManager _clienteManager;
        private List<ClienteDTO> _todosClientes;
        public Guid? ClienteIdSeleccionado { get; private set; }

        public FrmSeleccionarCliente()
        {
            InitializeComponent();
            _clienteManager = new ClienteManager();
        }

        private void FrmSeleccionarCliente_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            UpdateLanguage();
            CargarClientes();
            btnSeleccionar.Enabled = false;
        }

        private void ConfigurarGrid()
        {
            dgvClientes.AutoGenerateColumns = false;
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DNI", HeaderText = Domain.Enums.Translations.LBL_DNI.Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = Domain.Enums.Translations.LBL_NOMBRE.Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Apellido", HeaderText = Domain.Enums.Translations.LBL_APELLIDO.Translate() });
        }

        private void UpdateLanguage()
        {
            this.Text = Domain.Enums.Translations.FRM_SELECCIONAR_CLIENTE_TITLE.Translate();
            lblBuscar.Text = Domain.Enums.Translations.LBL_DNI.Translate();
            btnSeleccionar.Text = Domain.Enums.Translations.BTN_SELECCIONAR.Translate();
        }

        private void CargarClientes()
        {
            try
            {
                _todosClientes = _clienteManager.ListarClientes();
                FiltrarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Domain.Enums.Translations.MSG_ERR_GENERIC.Translate() + " " + ex.Message, Domain.Enums.Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FiltrarClientes()
        {
            if (_todosClientes == null) return;

            string filtro = txtBuscar.Text.Trim().ToLower();

            var clientesFiltrados = _todosClientes.Where(c =>
                c.DNI.ToString().Contains(filtro) ||
                c.Nombre.ToLower().Contains(filtro) ||
                c.Apellido.ToLower().Contains(filtro)
            ).ToList();

            dgvClientes.DataSource = clientesFiltrados;
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            FiltrarClientes();
        }

        private void dgvClientes_SelectionChanged(object sender, EventArgs e)
        {
            btnSeleccionar.Enabled = dgvClientes.SelectedRows.Count > 0;
        }

        private void dgvClientes_DoubleClick(object sender, EventArgs e)
        {
            Seleccionar();
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            Seleccionar();
        }

        private void Seleccionar()
        {
            if (dgvClientes.SelectedRows.Count > 0)
            {
                var cliente = (ClienteDTO)dgvClientes.SelectedRows[0].DataBoundItem;
                ClienteIdSeleccionado = cliente.Id;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
