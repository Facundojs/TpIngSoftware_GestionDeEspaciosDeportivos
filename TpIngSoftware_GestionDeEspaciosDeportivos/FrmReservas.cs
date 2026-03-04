using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BLL.DTOs;
using TpIngSoftware_GestionDeEspaciosDeportivos.Business;
using Service.Facade.Extension;
using Domain.Composite;
using Service.DTO;
using Service.Helpers;
using Domain.Enums;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmReservas : Form, IRefreshable, ITranslatable
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
            dtpFecha.Value = DateTime.Now.AddDays(1).Date;
        }

        private void ApplyPermissions()
        {
            if (_usuario == null) return;

            // btnGenerar is controlled by verification logic AND permission
            btnCancelar.Enabled = _usuario.TienePermiso(PermisoKeys.ReservaCancelar);
        }

        private void Translate()
        {
            this.Text = Translations.RESERVA_TITLE.Translate();
            btnVerificar.Text = Translations.BTN_VERIFICAR_DISPONIBILIDAD.Translate();
            btnGenerar.Text = Translations.BTN_GENERAR_RESERVA.Translate();
            btnCancelar.Text = Translations.BTN_CANCELAR_RESERVA.Translate();
            btnBuscarCliente.Text = Translations.BTN_SELECCIONAR.Translate();
            btnVerComprobante.Text = Translations.BTN_VER_COMPROBANTE.Translate();

            lblEspacio.Text = Translations.LBL_ESPACIO.Translate();
            lblFecha.Text = Translations.LBL_FECHA.Translate();
            lblHora.Text = Translations.LBL_HORA.Translate();
            lblDuracion.Text = Translations.LBL_DURACION.Translate();
            lblAdelanto.Text = Translations.LBL_ADELANTO.Translate();
            lblDni.Text = Translations.LBL_DNI.Translate();
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
                if (ex.Message == "ERR_NO_AGENDA")
                {
                    MessageBox.Show(Translations.ERR_NO_AGENDA.Translate(), Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + " " + ex.Message, Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                if (ex.Message == "ERR_NO_AGENDA")
                {
                    MessageBox.Show(Translations.ERR_NO_AGENDA.Translate(), Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + " " + ex.Message, Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FormatGrid()
        {
            if (dgvReservas.Columns.Count == 0) return;

            if (dgvReservas.Columns.Contains("Id")) dgvReservas.Columns["Id"].Visible = false;
            if (dgvReservas.Columns.Contains("ClienteID")) dgvReservas.Columns["ClienteID"].Visible = false;
            if (dgvReservas.Columns.Contains("EspacioID")) dgvReservas.Columns["EspacioID"].Visible = false;

            if (dgvReservas.Columns.Contains("CodigoReserva")) dgvReservas.Columns["CodigoReserva"].HeaderText = Translations.LBL_CODIGO_RESERVA.Translate();
            if (dgvReservas.Columns.Contains("ClienteNombre")) dgvReservas.Columns["ClienteNombre"].HeaderText = Translations.LBL_CLIENTE.Translate();
            if (dgvReservas.Columns.Contains("EspacioNombre")) dgvReservas.Columns["EspacioNombre"].HeaderText = Translations.LBL_ESPACIO.Translate();
            if (dgvReservas.Columns.Contains("FechaHora")) dgvReservas.Columns["FechaHora"].HeaderText = Translations.LBL_FECHA.Translate();
            if (dgvReservas.Columns.Contains("Duracion")) dgvReservas.Columns["Duracion"].HeaderText = Translations.LBL_DURACION.Translate();
            if (dgvReservas.Columns.Contains("Adelanto")) dgvReservas.Columns["Adelanto"].HeaderText = Translations.LBL_ADELANTO.Translate();
            if (dgvReservas.Columns.Contains("Estado")) dgvReservas.Columns["Estado"].HeaderText = Translations.LBL_ESTADO.Translate();
            btnVerHorarios.Text = Translations.BTN_VER_HORARIOS.Translate();
        }

        private void btnVerificar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbEspacio.SelectedValue == null)
                {
                    MessageBox.Show(Translations.MSG_SELECCIONE_ESPACIO.Translate(), Translations.TITLE_WARNING.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Guid espacioId = (Guid)cbEspacio.SelectedValue;
                DateTime fecha = dtpFecha.Value.Date;
                DateTime hora = dtpHora.Value;
                if (hora.Minute != 0 && hora.Minute != 30)
                {
                    MessageBox.Show(Translations.ERR_HORA_MULTIPLO_30.Translate(), Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DateTime fechaHora = new DateTime(fecha.Year, fecha.Month, fecha.Day, hora.Hour, hora.Minute, 0);
                int duracion = (int)numDuracion.Value;
                if (duracion % 30 != 0)
                {
                    MessageBox.Show(Translations.ERR_DURACION_MULTIPLO_30.Translate(), Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (fechaHora < DateTime.Now)
                {
                    MessageBox.Show(Translations.ERR_FECHA_PASADA.Translate(), Translations.TITLE_WARNING.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool disponible = _reservaManager.VerificarDisponibilidad(espacioId, fechaHora, duracion);

                if (disponible)
                {
                    MessageBox.Show(Translations.MSG_ESPACIO_DISPONIBLE.Translate(), Translations.TITLE_INFO.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    EnableReservaControls(true);
                }
                else
                {
                    try
                    {
                        var horarios = _reservaManager.ObtenerHorariosDisponibles(espacioId, fechaHora.Date);
                        string formatMsj = Translations.MSG_HORARIOS_DISPONIBLES_EL.Translate() + Environment.NewLine;
                        string msgDate = formatMsj.Contains("{0}") ? string.Format(formatMsj, fecha.ToShortDateString()) : formatMsj;
                        string msj = Translations.MSG_ESPACIO_NO_DISPONIBLE.Translate() + Environment.NewLine + Environment.NewLine + msgDate;
                        if (horarios.Count > 0)
                        {
                            foreach (var h in horarios)
                            {
                                msj += $"{h:hh\\:mm} - {h.Add(TimeSpan.FromMinutes(30)):hh\\:mm}" + Environment.NewLine;
                            }
                        }
                        else
                        {
                            msj += Translations.MSG_NINGUNO_ESTE_DIA.Translate();
                        }
                        MessageBox.Show(msj, Translations.TITLE_WARNING.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(Translations.MSG_ESPACIO_NO_DISPONIBLE.Translate(), Translations.TITLE_WARNING.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    EnableReservaControls(false);
                }
            }
                        catch (Exception ex)
            {
                if (ex.Message == "ERR_NO_AGENDA")
                {
                    MessageBox.Show(Translations.ERR_NO_AGENDA.Translate(), Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + " " + ex.Message, Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnVerHorarios_Click(object sender, EventArgs e)
        {
            if (cbEspacio.SelectedValue == null)
            {
                MessageBox.Show(Translations.MSG_SELECCIONE_ESPACIO.Translate(), Translations.TITLE_WARNING.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Guid espacioId = (Guid)cbEspacio.SelectedValue;
            DateTime fecha = dtpFecha.Value.Date;
            try
            {
                var horarios = _reservaManager.ObtenerHorariosDisponibles(espacioId, fecha);
                string formatMsj = Translations.MSG_HORARIOS_DISPONIBLES_EL.Translate() + Environment.NewLine;
                string msj = formatMsj.Contains("{0}") ? string.Format(formatMsj, fecha.ToShortDateString()) : formatMsj;
                if (horarios.Count > 0)
                {
                    foreach (var h in horarios)
                    {
                        msj += $"{h:hh\\:mm} - {h.Add(TimeSpan.FromMinutes(30)):hh\\:mm}{Environment.NewLine}";
                    }
                }
                else
                {
                    msj += Translations.MSG_NINGUNO_ESTE_DIA.Translate();
                }
                MessageBox.Show(msj, Translations.BTN_VER_HORARIOS.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                if (ex.Message == "ERR_NO_AGENDA")
                {
                    MessageBox.Show(Translations.ERR_NO_AGENDA.Translate(), Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + " " + ex.Message, Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                        lblNombreCliente.Text = Translations.ERR_CLIENTE_NO_ENCONTRADO.Translate();
                    }
                }
                else
                {
                    MessageBox.Show(Translations.ERR_INGRESE_DNI_VALIDO.Translate(), Translations.TITLE_WARNING.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
                        catch (Exception ex)
            {
                if (ex.Message == "ERR_NO_AGENDA")
                {
                    MessageBox.Show(Translations.ERR_NO_AGENDA.Translate(), Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + " " + ex.Message, Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_clienteIdSeleccionado == null)
                {
                    MessageBox.Show(Translations.ERR_SELECCIONE_CLIENTE.Translate(), Translations.TITLE_WARNING.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cbEspacio.SelectedValue == null)
                {
                    MessageBox.Show(Translations.MSG_SELECCIONE_ESPACIO.Translate(), Translations.TITLE_WARNING.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Guid espacioId = (Guid)cbEspacio.SelectedValue;
                DateTime fecha = dtpFecha.Value.Date;
                DateTime hora = dtpHora.Value;
                if (hora.Minute != 0 && hora.Minute != 30)
                {
                    MessageBox.Show(Translations.ERR_HORA_MULTIPLO_30.Translate(), Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DateTime fechaHora = new DateTime(fecha.Year, fecha.Month, fecha.Day, hora.Hour, hora.Minute, 0);
                int duracion = (int)numDuracion.Value;
                if (duracion % 30 != 0)
                {
                    MessageBox.Show(Translations.ERR_DURACION_MULTIPLO_30.Translate(), Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                decimal adelanto = numAdelanto.Value;

                var dto = new GenerarReservaDTO
                {
                    ClienteId = _clienteIdSeleccionado.Value,
                    EspacioId = espacioId,
                    FechaHora = fechaHora,
                    Duracion = duracion,
                    Adelanto = adelanto
                };

                string codigoGenerado = _reservaManager.GenerarReserva(dto);

                string successMsg = Translations.MSG_RESERVA_GENERADA.Translate().Replace("{codigo}", codigoGenerado);
                MessageBox.Show(successMsg, Translations.TITLE_SUCCESS.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                if (ex.Message == "ERR_NO_AGENDA")
                {
                    MessageBox.Show(Translations.ERR_NO_AGENDA.Translate(), Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + " " + ex.Message, Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvReservas.SelectedRows.Count == 0)
                {
                    MessageBox.Show(Translations.ERR_SELECCIONE_RESERVA.Translate(), Translations.TITLE_WARNING.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var row = dgvReservas.SelectedRows[0];
                var reserva = (ReservaDTO)row.DataBoundItem;

                if (MessageBox.Show(Translations.MSG_CONFIRM_CANCELAR.Translate(), Translations.TITLE_CONFIRM.Translate(), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _reservaManager.CancelarReserva(reserva.Id);
                    LoadReservas();
                }
            }
                        catch (Exception ex)
            {
                if (ex.Message == "ERR_NO_AGENDA")
                {
                    MessageBox.Show(Translations.ERR_NO_AGENDA.Translate(), Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + " " + ex.Message, Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void RefreshData()
        {
            LoadEspacios();
            LoadReservas();
        }

        public void UpdateLanguage() => Translate();

        private void btnVerComprobante_Click(object sender, EventArgs e)
        {
            if (dgvReservas.SelectedRows.Count == 0) return;
            var reserva = (ReservaDTO)dgvReservas.SelectedRows[0].DataBoundItem;

            try
            {
                var comprobante = _reservaManager.ObtenerComprobantePorReserva(reserva.Id);
                if (comprobante == null)
                {
                    MessageBox.Show(Translations.ERR_NO_COMPROBANTE.Translate(), Translations.TITLE_INFO.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (comprobante.Contenido == null || comprobante.Contenido.Length == 0)
                {
                    MessageBox.Show(Translations.ERR_COMPROBANTE_SIN_CONTENIDO.Translate(), Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string extension = Path.GetExtension(comprobante.NombreArchivo);
                if (string.IsNullOrEmpty(extension)) extension = ".dat";

                string tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{extension}");
                File.WriteAllBytes(tempPath, comprobante.Contenido);

                var psi = new System.Diagnostics.ProcessStartInfo(tempPath)
                {
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
