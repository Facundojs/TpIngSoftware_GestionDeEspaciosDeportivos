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
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DNI", HeaderText = Translations.LBL_DNI.Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = Translations.LBL_NOMBRE.Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Apellido", HeaderText = Translations.LBL_APELLIDO.Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = Translations.LBL_EMAIL.Translate() });

            // For complex properties like Membresia Name, we can use CellFormatting or a wrapper.
            // Simplified: We'll handle it in CellFormatting or DataBinding if possible.
            // Let's use CellFormatting for Membresia and Status.
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Membresia", HeaderText = Translations.LBL_MEMBRESIA.Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Balance", HeaderText = Translations.LBL_BALANCE.Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProximaFechaPago", HeaderText = Translations.LBL_PROXIMA_FECHA_PAGO.Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = Translations.LBL_ESTADO.Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Razon", HeaderText = Translations.LBL_RAZON.Translate() });

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
            this.Text = Translations.CLIENTE_TITLE.Translate();
            lblDNI.Text = Translations.LBL_DNI.Translate();
            lblNombre.Text = Translations.LBL_NOMBRE.Translate();
            lblApellido.Text = Translations.LBL_APELLIDO.Translate();
            lblFechaNacimiento.Text = Translations.LBL_FECHA_NAC.Translate();
            lblEmail.Text = Translations.LBL_EMAIL.Translate();
            lblMembresia.Text = Translations.LBL_MEMBRESIA.Translate();
            lblProximaFechaPago.Text = Translations.LBL_PROXIMA_FECHA_PAGO.Translate();
            btnCrear.Text = Translations.BTN_CREAR.Translate();
            btnActualizar.Text = Translations.BTN_ACTUALIZAR.Translate();
            btnDeshabilitar.Text = Translations.BTN_DESHABILITAR.Translate();
            btnHabilitar.Text = Translations.BTN_HABILITAR.Translate();
            btnLimpiar.Text = Translations.BTN_LIMPIAR.Translate();

            lblDNICheckIn.Text = Translations.LBL_DNI.Translate();
            btnCheckIn.Text = Translations.BTN_CHECK_IN.Translate();
            btnVerRutina.Text = Translations.BTN_VER_RUTINA.Translate();
            btnVerIngresos.Text = Translations.BTN_VER_INGRESOS.Translate();
            btnVerMovimientos.Text = Translations.BTN_VER_MOVIMIENTOS.Translate();

            if (dgvClientes.Columns.Count >= 9)
            {
                dgvClientes.Columns[0].HeaderText = Translations.LBL_DNI.Translate();
                dgvClientes.Columns[1].HeaderText = Translations.LBL_NOMBRE.Translate();
                dgvClientes.Columns[2].HeaderText = Translations.LBL_APELLIDO.Translate();
                dgvClientes.Columns[3].HeaderText = Translations.LBL_EMAIL.Translate();
                dgvClientes.Columns[4].HeaderText = Translations.LBL_MEMBRESIA.Translate();
                dgvClientes.Columns[5].HeaderText = Translations.LBL_BALANCE.Translate();
                dgvClientes.Columns[6].HeaderText = Translations.LBL_PROXIMA_FECHA_PAGO.Translate();
                dgvClientes.Columns[7].HeaderText = Translations.LBL_ESTADO.Translate();
                dgvClientes.Columns[8].HeaderText = Translations.LBL_RAZON.Translate();
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
            btnVerRutina.Enabled = false; // Disabled by default until selection
            btnVerMovimientos.Enabled = false; // Disabled by default until selection

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
                MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + " " + ex.Message, Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + " " + ex.Message, Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(Translations.MSG_CLIENTE_CREADO.Translate(), Translations.TITLE_INFO.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarClientes();
                LimpiarControles();
            }
            catch (FormatException)
            {
                MessageBox.Show(Translations.ERR_INVALID_NUMBER.Translate(), Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + " " + ex.Message, Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbMembresia.SelectedValue == null)
                {
                    MessageBox.Show(Translations.ERR_REQUIRED_FIELD.Translate(), Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (_clienteSeleccionado == null)
                {
                    if (string.IsNullOrWhiteSpace(txtDNI.Text))
                    {
                        MessageBox.Show(Translations.ERR_REQUIRED_FIELD.Translate(), Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int dni;
                    if (!int.TryParse(txtDNI.Text, out dni))
                    {
                        MessageBox.Show(Translations.ERR_INVALID_NUMBER.Translate(), Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var existente = _clienteManager.ObtenerClientePorDNI(dni);
                    if (existente == null)
                    {
                        if (!_usuario.TienePermiso(PermisoKeys.ClienteCrear))
                        {
                            MessageBox.Show(Translations.MSG_NO_PERMISSION.Translate(), Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (MessageBox.Show(Translations.MSG_CONFIRM_CREAR_CLIENTE_NUEVO.Translate(), Translations.TITLE_CONFIRM.Translate(), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
                            return; // Cancelled
                        }
                    }

                    if (existente != null)
                    {
                        _clienteSeleccionado = existente;
                    }
                }

                if (_clienteSeleccionado == null) return; // safety check

                Guid nuevaMembresiaId = (Guid)cmbMembresia.SelectedValue;
                _clienteManager.ActualizarMembresia(_clienteSeleccionado.Id, nuevaMembresiaId);

                MessageBox.Show(Translations.MSG_MEMBRESIA_UPDATED.Translate(), Translations.TITLE_INFO.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarClientes();
                LimpiarControles();
            }
            catch (FormatException)
            {
                MessageBox.Show(Translations.ERR_INVALID_NUMBER.Translate(), Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                 // Handle explicit message for debt if it comes from service
                 MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + " " + ex.Message, Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeshabilitar_Click(object sender, EventArgs e)
        {
            if (_clienteSeleccionado == null) return;

            try
            {
                if (MessageBox.Show(Translations.MSG_CONFIRM_DESHABILITAR_CLIENTE.Translate(), Translations.TITLE_CONFIRM.Translate(), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (var prompt = new FrmPrompt(Translations.LBL_RAZON_DESHABILITAR.Translate(), Translations.TITLE_DESHABILITAR.Translate()))
                    {
                        if (prompt.ShowDialog() == DialogResult.OK)
                        {
                            string razon = string.IsNullOrWhiteSpace(prompt.InputText) ? Translations.MSG_DEFAULT_DISABLE_REASON.Translate() : prompt.InputText;
                            _clienteManager.DeshabilitarCliente(_clienteSeleccionado.Id, razon);
                            CargarClientes();
                            LimpiarControles();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + " " + ex.Message, Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + " " + ex.Message, Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            txtDNI.Enabled = true; // Allow DNI edit only for new

            // Reset button states based on permissions and selection
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
                    string msg = Translations.MSG_INGRESO_PERMITIDO.Translate().Replace("{nombre}", resultado.NombreCliente);
                    lblResultado.Text = msg;
                    lblResultado.ForeColor = Color.Green;
                    MessageBox.Show(msg, Translations.BTN_CHECK_IN.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtDNICheckIn.Text = string.Empty; // Clear after success
                }
                else
                {
                    string msg = Translations.MSG_INGRESO_DENEGADO.Translate().Replace("{razon}", resultado.Razon);
                    lblResultado.Text = msg;
                    lblResultado.ForeColor = Color.Red;
                    MessageBox.Show(msg, Translations.BTN_CHECK_IN.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show(Translations.ERR_INVALID_NUMBER.Translate(), Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + " " + ex.Message, Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
