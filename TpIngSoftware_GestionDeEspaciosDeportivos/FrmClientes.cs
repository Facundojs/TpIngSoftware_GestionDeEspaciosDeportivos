using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BLL.DTOs;
using Service.Helpers;
using TpIngSoftware_GestionDeEspaciosDeportivos.Business;
using Service.Facade.Extension;
using Domain.Composite;
using Service.DTO;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmClientes : Form
    {
        private readonly ClienteManager _clienteManager;
        private readonly MembresiaManager _membresiaManager;
        private readonly UsuarioDTO _usuario;
        private ClienteDTO _clienteSeleccionado;

        public FrmClientes(UsuarioDTO usuario)
        {
            InitializeComponent();
            _usuario = usuario;
            _clienteManager = new ClienteManager();
            _membresiaManager = new MembresiaManager();
        }

        private void FrmClientes_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            UpdateLanguage();
            ApplyPermissions();
            CargarMembresias();
            CargarClientes();
            LimpiarControles();
        }

        private void ConfigurarGrid()
        {
            dgvClientes.AutoGenerateColumns = false;
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DNI", HeaderText = "LBL_DNI".Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "LBL_NOMBRE".Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Apellido", HeaderText = "LBL_APELLIDO".Translate() });

            // For complex properties like Membresia Name, we can use CellFormatting or a wrapper.
            // Simplified: We'll handle it in CellFormatting or DataBinding if possible.
            // Let's use CellFormatting for Membresia and Status.
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Membresia", HeaderText = "LBL_MEMBRESIA".Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Balance", HeaderText = "LBL_BALANCE".Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "LBL_ESTADO".Translate() });

            dgvClientes.CellFormatting += DgvClientes_CellFormatting;
        }

        private void DgvClientes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvClientes.Rows[e.RowIndex].DataBoundItem is ClienteDTO cliente)
            {
                if (dgvClientes.Columns[e.ColumnIndex].Name == "Membresia")
                {
                    e.Value = cliente.MembresiaDetalle?.Nombre ?? "-";
                }
            }
        }

        private void UpdateLanguage()
        {
            this.Text = "CLIENTE_TITLE".Translate();
            lblDNI.Text = "LBL_DNI".Translate();
            lblNombre.Text = "LBL_NOMBRE".Translate();
            lblApellido.Text = "LBL_APELLIDO".Translate();
            lblFechaNacimiento.Text = "LBL_FECHA_NAC".Translate();
            lblMembresia.Text = "LBL_MEMBRESIA".Translate();
            btnCrear.Text = "BTN_CREAR".Translate();
            btnActualizar.Text = "BTN_ACTUALIZAR".Translate();
            btnDeshabilitar.Text = "BTN_DESHABILITAR".Translate();
            btnHabilitar.Text = "BTN_HABILITAR".Translate();
            btnLimpiar.Text = "BTN_LIMPIAR".Translate();

            lblDNICheckIn.Text = "LBL_DNI".Translate();
            btnCheckIn.Text = "BTN_CHECK_IN".Translate();

            // Refresh headers
            if (dgvClientes.Columns.Count > 0)
            {
                dgvClientes.Columns[0].HeaderText = "LBL_DNI".Translate();
                dgvClientes.Columns[1].HeaderText = "LBL_NOMBRE".Translate();
                dgvClientes.Columns[2].HeaderText = "LBL_APELLIDO".Translate();
                dgvClientes.Columns[3].HeaderText = "LBL_MEMBRESIA".Translate();
                dgvClientes.Columns[4].HeaderText = "LBL_BALANCE".Translate();
                dgvClientes.Columns[5].HeaderText = "LBL_ESTADO".Translate();
            }
        }

        private void ApplyPermissions()
        {
            if (_usuario == null) return;

            btnCrear.Enabled = _usuario.TienePermiso(PermisoKeys.ClienteCrear);
            btnActualizar.Enabled = _usuario.TienePermiso(PermisoKeys.ClienteModificar);
            btnDeshabilitar.Enabled = _usuario.TienePermiso(PermisoKeys.ClienteDeshabilitar);
            btnHabilitar.Enabled = _usuario.TienePermiso(PermisoKeys.ClienteDeshabilitar); // Assuming same permission for Enable/Disable
            btnCheckIn.Enabled = _usuario.TienePermiso(PermisoKeys.ClienteCheckIn);

            // Edit panel visibility based on permissions? Usually we disable buttons.
        }

        private void CargarMembresias()
        {
            try
            {
                var membresias = _membresiaManager.ListarMembresias(soloActivas: true);
                cmbMembresia.DataSource = membresias;
                cmbMembresia.DisplayMember = "Nombre";
                cmbMembresia.ValueMember = "Id";
                cmbMembresia.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarClientes()
        {
            try
            {
                var clientes = _clienteManager.ListarClientes();
                dgvClientes.DataSource = null;
                dgvClientes.DataSource = clientes;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            try
            {
                var dto = new ClienteDTO
                {
                    DNI = int.Parse(txtDNI.Text),
                    Nombre = txtNombre.Text,
                    Apellido = txtApellido.Text,
                    FechaNacimiento = dtpFechaNacimiento.Value
                };

                if (cmbMembresia.SelectedValue != null)
                {
                    dto.MembresiaID = (Guid)cmbMembresia.SelectedValue;
                }

                _clienteManager.RegistrarCliente(dto);
                MessageBox.Show("MSG_CLIENTE_CREADO".Translate(), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarClientes();
                LimpiarControles();
            }
            catch (FormatException)
            {
                MessageBox.Show("ERR_INVALID_NUMBER".Translate(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (_clienteSeleccionado == null) return;

            try
            {
                if (cmbMembresia.SelectedValue == null)
                {
                    MessageBox.Show("ERR_REQUIRED_FIELD".Translate(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Guid nuevaMembresiaId = (Guid)cmbMembresia.SelectedValue;
                _clienteManager.ActualizarMembresia(_clienteSeleccionado.Id, nuevaMembresiaId);

                MessageBox.Show("MSG_MEMBRESIA_UPDATED".Translate(), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarClientes();
                LimpiarControles();
            }
            catch (Exception ex)
            {
                 // Handle explicit message for debt if it comes from service
                 MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeshabilitar_Click(object sender, EventArgs e)
        {
            if (_clienteSeleccionado == null) return;

            try
            {
                if (MessageBox.Show("MSG_CONFIRM_DESHABILITAR_CLIENTE".Translate(), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _clienteManager.DeshabilitarCliente(_clienteSeleccionado.Id, "Deshabilitado por usuario");
                    CargarClientes();
                    LimpiarControles();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHabilitar_Click(object sender, EventArgs e)
        {
            if (_clienteSeleccionado == null) return;

            try
            {
                _clienteManager.HabilitarCliente(_clienteSeleccionado.Id);
                CargarClientes();
                LimpiarControles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarControles();
        }

        private void LimpiarControles()
        {
            txtDNI.Text = "";
            txtNombre.Text = "";
            txtApellido.Text = "";
            dtpFechaNacimiento.Value = DateTime.Now;
            cmbMembresia.SelectedIndex = -1;
            txtDNICheckIn.Text = "";
            lblResultado.Text = "";

            _clienteSeleccionado = null;

            txtDNI.Enabled = true; // Allow DNI edit only for new

            // Reset button states based on permissions and selection
            btnCrear.Enabled = _usuario.TienePermiso(PermisoKeys.ClienteCrear);
            btnActualizar.Enabled = false;
            btnDeshabilitar.Enabled = false;
            btnHabilitar.Enabled = false;
        }

        private void dgvClientes_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count > 0)
            {
                var cliente = (ClienteDTO)dgvClientes.SelectedRows[0].DataBoundItem;
                _clienteSeleccionado = cliente;

                txtDNI.Text = cliente.DNI.ToString();
                txtNombre.Text = cliente.Nombre;
                txtApellido.Text = cliente.Apellido;
                dtpFechaNacimiento.Value = cliente.FechaNacimiento;

                if (cliente.MembresiaID.HasValue)
                {
                    cmbMembresia.SelectedValue = cliente.MembresiaID.Value;
                }
                else
                {
                    cmbMembresia.SelectedIndex = -1;
                }

                txtDNI.Enabled = false; // Cannot change DNI of existing client

                bool canModify = _usuario.TienePermiso(PermisoKeys.ClienteModificar);
                bool canDisable = _usuario.TienePermiso(PermisoKeys.ClienteDeshabilitar);

                btnCrear.Enabled = false;
                btnActualizar.Enabled = canModify;

                if (cliente.Status == ClienteStatus.Activo)
                {
                    btnDeshabilitar.Enabled = canDisable;
                    btnHabilitar.Enabled = false;
                }
                else
                {
                    btnDeshabilitar.Enabled = false;
                    btnHabilitar.Enabled = canDisable;
                }
            }
        }

        private void btnCheckIn_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtDNICheckIn.Text)) return;

                int dni = int.Parse(txtDNICheckIn.Text);
                var resultado = _clienteManager.ValidarIngreso(dni);

                if (resultado.Permitido)
                {
                    lblResultado.Text = $"MSG_INGRESO_PERMITIDO".Translate().Replace("{nombre}", resultado.NombreCliente);
                    lblResultado.ForeColor = Color.Green;
                }
                else
                {
                    lblResultado.Text = $"MSG_INGRESO_DENEGADO".Translate().Replace("{razon}", resultado.Razon);
                    lblResultado.ForeColor = Color.Red;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("ERR_INVALID_NUMBER".Translate(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
