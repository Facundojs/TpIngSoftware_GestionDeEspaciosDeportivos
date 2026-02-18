using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BLL.DTOs;
using Domain.Composite;
using Service.Facade.Extension;
using TpIngSoftware_GestionDeEspaciosDeportivos.Business;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmGestionRutinas : Form
    {
        private readonly ClienteManager _clienteManager;
        private readonly UsuarioDTO _usuario;
        private List<ClienteDTO> _todosClientes;
        private ClienteDTO _clienteSeleccionado;

        public FrmGestionRutinas(UsuarioDTO usuario)
        {
            InitializeComponent();
            _usuario = usuario;
            _clienteManager = new ClienteManager();
        }

        private void FrmGestionRutinas_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            UpdateLanguage();
            CargarClientes();
            ApplyPermissions();
        }

        private void ConfigurarGrid()
        {
            dgvClientes.AutoGenerateColumns = false;
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DNI", HeaderText = "LBL_DNI".Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "LBL_NOMBRE".Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Apellido", HeaderText = "LBL_APELLIDO".Translate() });
        }

        private void UpdateLanguage()
        {
            this.Text = "FRM_GESTION_RUTINAS_TITLE".Translate();
            lblBuscar.Text = "LBL_DNI".Translate(); // Or "Search" if available
            btnGestionarRutina.Text = "BTN_GESTIONAR_RUTINA".Translate();

             if (dgvClientes.Columns.Count > 0)
            {
                dgvClientes.Columns[0].HeaderText = "LBL_DNI".Translate();
                dgvClientes.Columns[1].HeaderText = "LBL_NOMBRE".Translate();
                dgvClientes.Columns[2].HeaderText = "LBL_APELLIDO".Translate();
            }
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
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void ApplyPermissions()
        {
            bool canView = _usuario.TienePermiso(PermisoKeys.RutinaVer);
            btnGestionarRutina.Enabled = canView && _clienteSeleccionado != null;
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            FiltrarClientes();
        }

        private void dgvClientes_SelectionChanged(object sender, EventArgs e)
        {
             if (dgvClientes.SelectedRows.Count > 0)
            {
                _clienteSeleccionado = (ClienteDTO)dgvClientes.SelectedRows[0].DataBoundItem;
            }
            else
            {
                _clienteSeleccionado = null;
            }
            ApplyPermissions();
        }

        private void dgvClientes_DoubleClick(object sender, EventArgs e)
        {
            if (_clienteSeleccionado != null && btnGestionarRutina.Enabled)
            {
                AbrirRutina();
            }
        }

        private void btnGestionarRutina_Click(object sender, EventArgs e)
        {
            AbrirRutina();
        }

        private void AbrirRutina()
        {
            if (_clienteSeleccionado == null) return;

            var frm = new FrmRutina(_clienteSeleccionado.Id, _usuario);
            frm.ShowDialog();
            // Optional: Refresh if needed, but managing routine doesn't change client list usually
        }
    }
}
