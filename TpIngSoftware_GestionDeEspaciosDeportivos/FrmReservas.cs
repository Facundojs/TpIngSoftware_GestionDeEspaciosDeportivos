using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BLL.DTOs;
using BLL.Services;
using TpIngSoftware_GestionDeEspaciosDeportivos.Business;
using Service.Facade.Extension;
using Domain.Composite;
using Service.DTO;
using Service.Helpers;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmReservas : Form
    {
        private readonly ReservaManager _reservaManager;
        private readonly EspacioManager _espacioManager;
        private readonly ClienteManager _clienteManager;
        private readonly UsuarioDTO _usuario;
        private Guid? _clienteIdSeleccionado = null;

        public FrmReservas(UsuarioDTO usuario)
        {
            InitializeComponent();
            _usuario = usuario;
            _reservaManager = new ReservaManager();
            _espacioManager = new EspacioManager();
            _clienteManager = new ClienteManager();

            Translate();
            LoadEspacios();
            LoadReservas();
            ApplyPermissions();
            EnableReservaControls(false); // Init state
        }

        private void ApplyPermissions()
        {
            if (_usuario == null) return;

            // btnGenerar is controlled by verification logic AND permission
            btnCancelar.Enabled = _usuario.TienePermiso(PermisoKeys.ReservaCancelar);
        }

        private void Translate()
        {
            this.Text = "RESERVA_TITLE".Translate();
            btnVerificar.Text = "BTN_VERIFICAR_DISPONIBILIDAD".Translate();
            btnGenerar.Text = "BTN_GENERAR_RESERVA".Translate();
            btnCancelar.Text = "BTN_CANCELAR_RESERVA".Translate();
            btnBuscarCliente.Text = "BTN_SELECCIONAR".Translate();

            lblEspacio.Text = "LBL_ESPACIO".Translate();
            lblFecha.Text = "LBL_FECHA".Translate();
            lblHora.Text = "LBL_HORA".Translate();
            lblDuracion.Text = "LBL_DURACION".Translate();
            lblAdelanto.Text = "LBL_ADELANTO".Translate();
            lblDni.Text = "LBL_DNI".Translate();
            lblNombreCliente.Text = "";
        }

        private void LoadEspacios()
        {
            try
            {
                var espacios = _espacioManager.ListarEspacios();
                cbEspacio.DataSource = espacios;
                cbEspacio.DisplayMember = "Nombre";
                cbEspacio.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadReservas()
        {
            try
            {
                var reservas = _reservaManager.ListarReservas(null, null, dtpFecha.Value.Date);
                dgvReservas.DataSource = null;
                dgvReservas.DataSource = reservas;
                FormatGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FormatGrid()
        {
            if (dgvReservas.Columns.Count == 0) return;

            if (dgvReservas.Columns.Contains("Id")) dgvReservas.Columns["Id"].Visible = false;
            if (dgvReservas.Columns.Contains("ClienteID")) dgvReservas.Columns["ClienteID"].Visible = false;
            if (dgvReservas.Columns.Contains("EspacioID")) dgvReservas.Columns["EspacioID"].Visible = false;

            if (dgvReservas.Columns.Contains("CodigoReserva")) dgvReservas.Columns["CodigoReserva"].HeaderText = "LBL_CODIGO_RESERVA".Translate();
            if (dgvReservas.Columns.Contains("ClienteNombre")) dgvReservas.Columns["ClienteNombre"].HeaderText = "LBL_CLIENTE".Translate();
            if (dgvReservas.Columns.Contains("EspacioNombre")) dgvReservas.Columns["EspacioNombre"].HeaderText = "LBL_ESPACIO".Translate();
            if (dgvReservas.Columns.Contains("FechaHora")) dgvReservas.Columns["FechaHora"].HeaderText = "LBL_FECHA".Translate();
            if (dgvReservas.Columns.Contains("Duracion")) dgvReservas.Columns["Duracion"].HeaderText = "LBL_DURACION".Translate();
            if (dgvReservas.Columns.Contains("Adelanto")) dgvReservas.Columns["Adelanto"].HeaderText = "LBL_ADELANTO".Translate();
            if (dgvReservas.Columns.Contains("Estado")) dgvReservas.Columns["Estado"].HeaderText = "LBL_ESTADO".Translate();
        }

        private void btnVerificar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbEspacio.SelectedValue == null)
                {
                    MessageBox.Show("Seleccione un espacio");
                    return;
                }

                Guid espacioId = (Guid)cbEspacio.SelectedValue;
                DateTime fecha = dtpFecha.Value.Date;
                DateTime hora = dtpHora.Value;
                DateTime fechaHora = new DateTime(fecha.Year, fecha.Month, fecha.Day, hora.Hour, hora.Minute, 0);
                int duracion = (int)numDuracion.Value;

                if (fechaHora < DateTime.Now)
                {
                    MessageBox.Show("La fecha no puede ser anterior a la actual");
                    return;
                }

                bool disponible = _reservaManager.VerificarDisponibilidad(espacioId, fechaHora, duracion);

                if (disponible)
                {
                    MessageBox.Show("MSG_ESPACIO_DISPONIBLE".Translate(), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    EnableReservaControls(true);
                }
                else
                {
                    MessageBox.Show("MSG_ESPACIO_NO_DISPONIBLE".Translate(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    EnableReservaControls(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EnableReservaControls(bool enable)
        {
            txtDniCliente.Enabled = enable;
            btnBuscarCliente.Enabled = enable;
            numAdelanto.Enabled = enable;
            btnGenerar.Enabled = enable && (_usuario != null && _usuario.TienePermiso(PermisoKeys.ReservaCrear));
        }

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(txtDniCliente.Text, out int dni))
                {
                    var cliente = _clienteManager.ObtenerClientePorDNI(dni);
                    if (cliente != null)
                    {
                        _clienteIdSeleccionado = cliente.Id;
                        lblNombreCliente.Text = $"{cliente.Nombre} {cliente.Apellido}";
                    }
                    else
                    {
                        _clienteIdSeleccionado = null;
                        lblNombreCliente.Text = "Cliente no encontrado";
                    }
                }
                else
                {
                    MessageBox.Show("Ingrese un DNI válido");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_clienteIdSeleccionado == null)
                {
                    MessageBox.Show("Seleccione un cliente válido");
                    return;
                }
                if (cbEspacio.SelectedValue == null)
                {
                    MessageBox.Show("Seleccione un espacio");
                    return;
                }

                Guid espacioId = (Guid)cbEspacio.SelectedValue;
                DateTime fecha = dtpFecha.Value.Date;
                DateTime hora = dtpHora.Value;
                DateTime fechaHora = new DateTime(fecha.Year, fecha.Month, fecha.Day, hora.Hour, hora.Minute, 0);
                int duracion = (int)numDuracion.Value;
                decimal adelanto = numAdelanto.Value;

                var dto = new GenerarReservaDTO
                {
                    ClienteId = _clienteIdSeleccionado.Value,
                    EspacioId = espacioId,
                    FechaHora = fechaHora,
                    Duracion = duracion,
                    Adelanto = adelanto
                };

                _reservaManager.GenerarReserva(dto);

                MessageBox.Show("MSG_RESERVA_GENERADA".Translate(), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear fields
                _clienteIdSeleccionado = null;
                lblNombreCliente.Text = "-";
                txtDniCliente.Text = "";
                numAdelanto.Value = 0;
                EnableReservaControls(false); // Reset availability check

                LoadReservas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvReservas.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Seleccione una reserva");
                    return;
                }

                var row = dgvReservas.SelectedRows[0];
                var reserva = (ReservaDTO)row.DataBoundItem;

                if (MessageBox.Show("MSG_CONFIRM_CANCELAR".Translate(), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _reservaManager.CancelarReserva(reserva.Id);
                    LoadReservas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
