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
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DNI", HeaderText = Domain.Enums.Translations.LBL_DNI.Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = Domain.Enums.Translations.LBL_NOMBRE.Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Apellido", HeaderText = Domain.Enums.Translations.LBL_APELLIDO.Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = Domain.Enums.Translations.LBL_EMAIL.Translate() });

            // For complex properties like Membresia Name, we can use CellFormatting or a wrapper.
            // Simplified: We'll handle it in CellFormatting or DataBinding if possible.
            // Let's use CellFormatting for Membresia and Status.
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Membresia", HeaderText = Domain.Enums.Translations.LBL_MEMBRESIA.Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Balance", HeaderText = Domain.Enums.Translations.LBL_BALANCE.Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProximaFechaPago", HeaderText = Domain.Enums.Translations.LBL_PROXIMA_FECHA_PAGO.Translate() });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = Domain.Enums.Translations.LBL_ESTADO.Translate() });

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
            this.Text = Domain.Enums.Translations.CLIENTE_TITLE.Translate();
            lblDNI.Text = Domain.Enums.Translations.LBL_DNI.Translate();
            lblNombre.Text = Domain.Enums.Translations.LBL_NOMBRE.Translate();
            lblApellido.Text = Domain.Enums.Translations.LBL_APELLIDO.Translate();
            lblFechaNacimiento.Text = Domain.Enums.Translations.LBL_FECHA_NAC.Translate();
            lblEmail.Text = Domain.Enums.Translations.LBL_EMAIL.Translate();
            lblMembresia.Text = Domain.Enums.Translations.LBL_MEMBRESIA.Translate();
            lblProximaFechaPago.Text = Domain.Enums.Translations.LBL_PROXIMA_FECHA_PAGO.Translate();
            btnCrear.Text = Domain.Enums.Translations.BTN_CREAR.Translate();
            btnActualizar.Text = Domain.Enums.Translations.BTN_ACTUALIZAR.Translate();
            btnDeshabilitar.Text = Domain.Enums.Translations.BTN_DESHABILITAR.Translate();
            btnHabilitar.Text = Domain.Enums.Translations.BTN_HABILITAR.Translate();
            btnLimpiar.Text = Domain.Enums.Translations.BTN_LIMPIAR.Translate();

            lblDNICheckIn.Text = Domain.Enums.Translations.LBL_DNI.Translate();
            btnCheckIn.Text = Domain.Enums.Translations.BTN_CHECK_IN.Translate();
            btnVerRutina.Text = Domain.Enums.Translations.BTN_VER_RUTINA.Translate();
            btnVerIngresos.Text = Domain.Enums.Translations.BTN_VER_INGRESOS.Translate();
            btnVerMovimientos.Text = Domain.Enums.Translations.BTN_VER_MOVIMIENTOS.Translate();

            // Refresh headers
            if (dgvClientes.Columns.Count > 0)
            {
                dgvClientes.Columns[0].HeaderText = Domain.Enums.Translations.LBL_DNI.Translate();
                dgvClientes.Columns[1].HeaderText = Domain.Enums.Translations.LBL_NOMBRE.Translate();
                dgvClientes.Columns[2].HeaderText = Domain.Enums.Translations.LBL_APELLIDO.Translate();
                dgvClientes.Columns[3].HeaderText = Domain.Enums.Translations.LBL_EMAIL.Translate();
                dgvClientes.Columns[4].HeaderText = Domain.Enums.Translations.LBL_MEMBRESIA.Translate();
                dgvClientes.Columns[5].HeaderText = Domain.Enums.Translations.LBL_BALANCE.Translate();
                dgvClientes.Columns[6].HeaderText = Domain.Enums.Translations.LBL_PROXIMA_FECHA_PAGO.Translate();
                dgvClientes.Columns[7].HeaderText = Domain.Enums.Translations.LBL_ESTADO.Translate();
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
                    Email = txtEmail.Text,
                    FechaNacimiento = dtpFechaNacimiento.Value
                };

                if (cmbMembresia.SelectedValue != null)
                {
                    dto.MembresiaID = (Guid)cmbMembresia.SelectedValue;
                }

                _clienteManager.RegistrarCliente(dto);
                MessageBox.Show(Domain.Enums.Translations.MSG_CLIENTE_CREADO.Translate(), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarClientes();
                LimpiarControles();
            }
            catch (FormatException)
            {
                MessageBox.Show(Domain.Enums.Translations.ERR_INVALID_NUMBER.Translate(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbMembresia.SelectedValue == null)
                {
                    MessageBox.Show(Domain.Enums.Translations.ERR_REQUIRED_FIELD.Translate(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (_clienteSeleccionado == null)
                {
                    if (string.IsNullOrWhiteSpace(txtDNI.Text))
                    {
                        MessageBox.Show(Domain.Enums.Translations.ERR_REQUIRED_FIELD.Translate(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int dni;
                    if (!int.TryParse(txtDNI.Text, out dni))
                    {
                        MessageBox.Show(Domain.Enums.Translations.ERR_INVALID_NUMBER.Translate(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var existente = _clienteManager.ObtenerClientePorDNI(dni);
                    if (existente == null)
                    {
                        if (!_usuario.TienePermiso(PermisoKeys.ClienteCrear))
                        {
                            MessageBox.Show(Domain.Enums.Translations.MSG_NO_PERMISSION.Translate(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (MessageBox.Show(Domain.Enums.Translations.MSG_CONFIRM_CREAR_CLIENTE_NUEVO.Translate(), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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

                MessageBox.Show(Domain.Enums.Translations.MSG_MEMBRESIA_UPDATED.Translate(), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarClientes();
                LimpiarControles();
            }
            catch (FormatException)
            {
                MessageBox.Show(Domain.Enums.Translations.ERR_INVALID_NUMBER.Translate(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (MessageBox.Show(Domain.Enums.Translations.MSG_CONFIRM_DESHABILITAR_CLIENTE.Translate(), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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

            string razon = SolicitarRazon();
            if (string.IsNullOrWhiteSpace(razon)) return;

            try
            {
                _clienteManager.HabilitarCliente(_clienteSeleccionado.Id, razon);
                CargarClientes();
                LimpiarControles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string SolicitarRazon()
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = Domain.Enums.Translations.MSG_INGRESE_RAZON.Translate(),
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false,
                MinimizeBox = false
            };
            Label textLabel = new Label() { Left = 20, Top = 20, Width = 340, Text = Domain.Enums.Translations.MSG_INGRESE_RAZON.Translate() };
            TextBox textBox = new TextBox() { Left = 20, Top = 40, Width = 340 };
            Button confirmation = new Button() { Text = "OK", Left = 260, Top = 70, Width = 100, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            while (true)
            {
                if (prompt.ShowDialog() == DialogResult.OK)
                {
                    if (string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        MessageBox.Show(Domain.Enums.Translations.MSG_RAZON_REQUERIDA.Translate(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        return textBox.Text;
                    }
                }
                else
                {
                    return null;
                }
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
                    lblResultado.Text = Domain.Enums.Translations.MSG_INGRESO_PERMITIDO.Translate().Replace("{nombre}", resultado.NombreCliente);
                    lblResultado.ForeColor = Color.Green;
                }
                else
                {
                    lblResultado.Text = Domain.Enums.Translations.MSG_INGRESO_DENEGADO.Translate().Replace("{razon}", resultado.Razon);
                    lblResultado.ForeColor = Color.Red;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show(Domain.Enums.Translations.ERR_INVALID_NUMBER.Translate(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
