using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BLL.DTOs;
using Domain.Composite;
using Service.DTO;
using Service.Helpers;
using Service.Facade.Extension;
using TpIngSoftware_GestionDeEspaciosDeportivos.Business;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmGestionRutinas : Form
    {
        private readonly UsuarioDTO _usuario;
        private readonly RutinaManager _rutinaManager;
        private RutinaDTO _rutinaSeleccionada;

        public FrmGestionRutinas(UsuarioDTO usuario)
        {
            InitializeComponent();
            _usuario = usuario;
            _rutinaManager = new RutinaManager();
        }

        private void FrmGestionRutinas_Load(object sender, EventArgs e)
        {
            ConfigurarUI();
            CargarRutinas();
            ApplyPermissions();
        }

        private void ConfigurarUI()
        {
            this.Text = "FRM_GESTION_RUTINAS_TITLE".Translate();
            btnGestionarEjercicios.Text = "BTN_GESTIONAR_EJERCICIOS".Translate();
            chkVerHistorial.Text = "CHK_VER_HISTORIAL".Translate();
            btnNueva.Text = "BTN_NUEVA_RUTINA".Translate();
            btnModificar.Text = "BTN_MODIFICAR".Translate();
            btnEliminar.Text = "BTN_ELIMINAR".Translate();

            dgvRutinas.AutoGenerateColumns = false;
            dgvRutinas.Columns.Clear();
            dgvRutinas.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ClienteNombre", HeaderText = "LBL_CLIENTE".Translate(), Width = 200 });
            dgvRutinas.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Desde", HeaderText = "LBL_RUTINA_DESDE".Translate() });
            dgvRutinas.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Hasta", HeaderText = "LBL_RUTINA_HASTA".Translate() });
        }

        private void ApplyPermissions()
        {
            btnNueva.Enabled = _usuario.TienePermiso(PermisoKeys.RutinaCrear);
            btnGestionarEjercicios.Enabled = _usuario.TienePermiso(PermisoKeys.EjercicioListar);

            // Modify/Delete enabled only when selected
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;
        }

        private void CargarRutinas()
        {
            try
            {
                // Logic: if checked -> show all (active and history). If unchecked -> show only active (Hasta == null)
                bool soloActivas = !chkVerHistorial.Checked;
                var rutinas = _rutinaManager.ListarRutinas(soloActivas);
                dgvRutinas.DataSource = null;
                dgvRutinas.DataSource = rutinas;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGestionarEjercicios_Click(object sender, EventArgs e)
        {
            var frm = new FrmEjercicios(_usuario);
            frm.ShowDialog();
        }

        private void btnNueva_Click(object sender, EventArgs e)
        {
            // First select client
            var frmClient = new FrmSeleccionarCliente();
            if (frmClient.ShowDialog() == DialogResult.OK && frmClient.ClienteIdSeleccionado.HasValue)
            {
                var frmRutina = new FrmRutina(frmClient.ClienteIdSeleccionado.Value, _usuario);
                frmRutina.ShowDialog();
                CargarRutinas();
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (_rutinaSeleccionada == null) return;

            // Open Routine Form with specific Routine ID
            var frm = new FrmRutina(_rutinaSeleccionada.ClienteID, _usuario, _rutinaSeleccionada.Id);
            frm.ShowDialog();
            CargarRutinas();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_rutinaSeleccionada == null) return;

            if (!_usuario.TienePermiso(PermisoKeys.RutinaEliminar))
            {
                MessageBox.Show("MSG_NO_PERM_USERS".Translate(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("MSG_CONFIRM_BORRAR_RUTINA".Translate(), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    _rutinaManager.BorrarRutina(_rutinaSeleccionada.Id);
                    MessageBox.Show("MSG_RUTINA_ELIMINADA".Translate(), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarRutinas();
                }
                catch (Exception ex)
                {
                     MessageBox.Show($"Error al borrar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvRutinas_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRutinas.SelectedRows.Count > 0)
            {
                _rutinaSeleccionada = (RutinaDTO)dgvRutinas.SelectedRows[0].DataBoundItem;
                btnModificar.Enabled = _usuario.TienePermiso(PermisoKeys.RutinaModificar);
                btnEliminar.Enabled = _usuario.TienePermiso(PermisoKeys.RutinaEliminar);
            }
            else
            {
                _rutinaSeleccionada = null;
                btnModificar.Enabled = false;
                btnEliminar.Enabled = false;
            }
        }

        private void chkVerHistorial_CheckedChanged(object sender, EventArgs e)
        {
            CargarRutinas();
        }
    }
}
