using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BLL.DTOs;
using Service.Helpers;
using TpIngSoftware_GestionDeEspaciosDeportivos.Business;
using Service.Facade.Extension;
using Domain;
using Service.DTO;
using Domain.Enums;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmClientes : Form, IRefreshable, ITranslatable
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

        public void RefreshData()
        {
            CargarMembresias();
            CargarClientes();
        }

        private void ConfigurarGrid()
        {
            dgvClientes.AutoGenerateColumns = false;
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DNI", HeaderText = "LBL_DNI".Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "LBL_NOMBRE".Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Apellido", HeaderText = "LBL_APELLIDO".Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = "LBL_EMAIL".Translate() });

            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Membresia", HeaderText = "LBL_MEMBRESIA".Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Balance", HeaderText = "LBL_BALANCE".Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProximaFechaPago", HeaderText = "LBL_PROXIMA_FECHA_PAGO".Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "LBL_ESTADO".Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Razon", HeaderText = "LBL_RAZON".Translate() });

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

        public void UpdateLanguage()
        {
            this.Text = "CLIENTE_TITLE".Translate();
            lblDNI.Text = "LBL_DNI".Translate();
            lblNombre.Text = "LBL_NOMBRE".Translate();
            lblApellido.Text = "LBL_APELLIDO".Translate();
            lblFechaNacimiento.Text = "LBL_FECHA_NAC".Translate();
            lblEmail.Text = "LBL_EMAIL".Translate();
            lblMembresia.Text = "LBL_MEMBRESIA".Translate();
            lblProximaFechaPago.Text = "LBL_PROXIMA_FECHA_PAGO".Translate();
            btnCrear.Text = "BTN_CREAR".Translate();
            btnActualizar.Text = "BTN_ACTUALIZAR".Translate();
            btnDeshabilitar.Text = "BTN_DESHABILITAR".Translate();
            btnHabilitar.Text = "BTN_HABILITAR".Translate();
            btnLimpiar.Text = "BTN_LIMPIAR".Translate();

            lblDNICheckIn.Text = "LBL_DNI".Translate();
            btnCheckIn.Text = "BTN_CHECK_IN".Translate();
            btnVerRutina.Text = "BTN_VER_RUTINA".Translate();
            btnVerIngresos.Text = "BTN_VER_INGRESOS".Translate();
            btnVerMovimientos.Text = "BTN_VER_MOVIMIENTOS".Translate();

            if (dgvClientes.Columns.Count >= 9)
            {
                dgvClientes.Columns[0].HeaderText = "LBL_DNI".Translate();
                dgvClientes.Columns[1].HeaderText = "LBL_NOMBRE".Translate();
                dgvClientes.Columns[2].HeaderText = "LBL_APELLIDO".Translate();
                dgvClientes.Columns[3].HeaderText = "LBL_EMAIL".Translate();
                dgvClientes.Columns[4].HeaderText = "LBL_MEMBRESIA".Translate();
                dgvClientes.Columns[5].HeaderText = "LBL_BALANCE".Translate();
                dgvClientes.Columns[6].HeaderText = "LBL_PROXIMA_FECHA_PAGO".Translate();
                dgvClientes.Columns[7].HeaderText = "LBL_ESTADO".Translate();
                dgvClientes.Columns[8].HeaderText = "LBL_RAZON".Translate();
            }
        }

        private void ApplyPermissions()
        {
            if (_usuario == null) return;

            btnCrear.Enabled = _usuario.TienePermiso(PermisoKeys.ClienteCrear);
            btnActualizar.Enabled = _usuario.TienePermiso(PermisoKeys.ClienteModificar);
            btnDeshabilitar.Enabled = _usuario.TienePermiso(PermisoKeys.ClienteDeshabilitar);
            btnHabilitar.Enabled = _usuario.TienePermiso(PermisoKeys.ClienteDeshabilitar);
            btnCheckIn.Enabled = _usuario.TienePermiso(PermisoKeys.ClienteCheckIn);
            btnVerRutina.Enabled = false;
            btnVerMovimientos.Enabled = false;
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
                MessageBox.Show("MSG_ERR_GENERIC".Translate() + " " + ex.Message, "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("MSG_ERR_GENERIC".Translate() + " " + ex.Message, "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    Email = txtEmail.Text,
                    FechaNacimiento = dtpFechaNacimiento.Value
                };

                if (cmbMembresia.SelectedValue != null)
                {
                    dto.MembresiaID = (Guid)cmbMembresia.SelectedValue;
                }

                _clienteManager.RegistrarCliente(dto);
                MessageBox.Show("MSG_CLIENTE_CREADO".Translate(), "TITLE_INFO".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarClientes();
                LimpiarControles();
            }
            catch (FormatException)
            {
                MessageBox.Show("ERR_INVALID_NUMBER".Translate(), "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("MSG_ERR_GENERIC".Translate() + " " + ex.Message, "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbMembresia.SelectedValue == null)
                {
                    MessageBox.Show("ERR_REQUIRED_FIELD".Translate(), "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (_clienteSeleccionado == null)
                {
                    if (string.IsNullOrWhiteSpace(txtDNI.Text))
                    {
                        MessageBox.Show("ERR_REQUIRED_FIELD".Translate(), "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int dni;
                    if (!int.TryParse(txtDNI.Text, out dni))
                    {
                        MessageBox.Show("ERR_INVALID_NUMBER".Translate(), "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var existente = _clienteManager.ObtenerClientePorDNI(dni);
                    if (existente == null)
                    {
                        if (!_usuario.TienePermiso(PermisoKeys.ClienteCrear))
                        {
                            MessageBox.Show("MSG_NO_PERMISSION".Translate(), "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (MessageBox.Show("MSG_CONFIRM_CREAR_CLIENTE_NUEVO".Translate(), "TITLE_CONFIRM".Translate(), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            var dto = new ClienteDTO
                            {
                                DNI = dni,
                                Nombre = txtNombre.Text,
                                Apellido = txtApellido.Text,
                                Email = txtEmail.Text,
                                FechaNacimiento = dtpFechaNacimiento.Value
                            };

                            dto.MembresiaID = (Guid)cmbMembresia.SelectedValue;

                            _clienteManager.RegistrarCliente(dto);
                            CargarClientes();
                            LimpiarControles();
                            return; // Done, no need to update membership as RegistrarCliente did it
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (existente != null)
                    {
                        _clienteSeleccionado = existente;
                    }
                }

                if (_clienteSeleccionado == null) return;

                Guid nuevaMembresiaId = (Guid)cmbMembresia.SelectedValue;
                _clienteManager.ActualizarMembresia(_clienteSeleccionado.Id, nuevaMembresiaId);

                MessageBox.Show("MSG_MEMBRESIA_UPDATED".Translate(), "TITLE_INFO".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarClientes();
                LimpiarControles();
            }
            catch (FormatException)
            {
                MessageBox.Show("ERR_INVALID_NUMBER".Translate(), "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                 MessageBox.Show("MSG_ERR_GENERIC".Translate() + " " + ex.Message, "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeshabilitar_Click(object sender, EventArgs e)
        {
            if (_clienteSeleccionado == null) return;

            try
            {
                if (MessageBox.Show("MSG_CONFIRM_DESHABILITAR_CLIENTE".Translate(), "TITLE_CONFIRM".Translate(), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (var prompt = new FrmPrompt("LBL_RAZON_DESHABILITAR".Translate(), "TITLE_DESHABILITAR".Translate()))
                    {
                        if (prompt.ShowDialog() == DialogResult.OK)
                        {
                            string razon = string.IsNullOrWhiteSpace(prompt.InputText) ? "MSG_DEFAULT_DISABLE_REASON".Translate() : prompt.InputText;
                            _clienteManager.DeshabilitarCliente(_clienteSeleccionado.Id, razon);
                            CargarClientes();
                            LimpiarControles();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("MSG_ERR_GENERIC".Translate() + " " + ex.Message, "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("MSG_ERR_GENERIC".Translate() + " " + ex.Message, "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            txtEmail.Text = "";
            dtpFechaNacimiento.Value = DateTime.Now;
            cmbMembresia.SelectedIndex = -1;
            txtProximaFechaPago.Text = "";
            txtDNICheckIn.Text = "";
            lblResultado.Text = "";

            _clienteSeleccionado = null;

            txtDNI.Enabled = true; // Allow DNI edit only for new clients
            btnCrear.Enabled = _usuario.TienePermiso(PermisoKeys.ClienteCrear);
            btnActualizar.Enabled = _usuario.TienePermiso(PermisoKeys.ClienteModificar);
            btnDeshabilitar.Enabled = false;
            btnHabilitar.Enabled = false;
            btnVerRutina.Enabled = false;
            btnVerMovimientos.Enabled = false;
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
                txtEmail.Text = cliente.Email;
                dtpFechaNacimiento.Value = cliente.FechaNacimiento;

                if (cliente.MembresiaID.HasValue)
                {
                    cmbMembresia.SelectedValue = cliente.MembresiaID.Value;
                }
                else
                {
                    cmbMembresia.SelectedIndex = -1;
                }

                if (cliente.ProximaFechaPago.HasValue)
                {
                    txtProximaFechaPago.Text = cliente.ProximaFechaPago.Value.ToString("d");
                }
                else
                {
                    txtProximaFechaPago.Text = "-";
                }

                txtDNI.Enabled = false; // Cannot change DNI of existing client

                bool canModify = _usuario.TienePermiso(PermisoKeys.ClienteModificar);
                bool canDisable = _usuario.TienePermiso(PermisoKeys.ClienteDeshabilitar);
                bool canViewRoutine = _usuario.TienePermiso(PermisoKeys.RutinaVer);

                btnCrear.Enabled = false;
                btnActualizar.Enabled = canModify;
                btnVerRutina.Enabled = canViewRoutine;
                btnVerMovimientos.Enabled = true;

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

        private void btnVerRutina_Click(object sender, EventArgs e)
        {
            if (_clienteSeleccionado == null) return;
            var frm = new FrmRutina(_clienteSeleccionado.Id, _usuario);
            frm.ShowDialog();
        }

        private void btnVerMovimientos_Click(object sender, EventArgs e)
        {
            if (_clienteSeleccionado == null) return;
            var frm = new FrmMovimientos(_clienteSeleccionado);
            frm.ShowDialog();
        }

        private void btnVerIngresos_Click(object sender, EventArgs e)
        {
            if (_clienteSeleccionado == null) return;
            var frm = new FrmIngresos(_usuario, _clienteSeleccionado);
            frm.ShowDialog();
        }

        private void cmbMembresia_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasMembresia = cmbMembresia.SelectedIndex != -1;
            lblProximaFechaPago.Visible = hasMembresia;
            txtProximaFechaPago.Visible = hasMembresia;
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
                    string msg = "MSG_INGRESO_PERMITIDO".Translate().Replace("{nombre}", resultado.NombreCliente);
                    lblResultado.Text = msg;
                    lblResultado.ForeColor = Color.Green;
                    MessageBox.Show(msg, "BTN_CHECK_IN".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtDNICheckIn.Text = string.Empty;
                }
                else
                {
                    string msg = "MSG_INGRESO_DENEGADO".Translate().Replace("{razon}", resultado.Razon);
                    lblResultado.Text = msg;
                    lblResultado.ForeColor = Color.Red;
                    MessageBox.Show(msg, "BTN_CHECK_IN".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("ERR_INVALID_NUMBER".Translate(), "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("MSG_ERR_GENERIC".Translate() + " " + ex.Message, "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
