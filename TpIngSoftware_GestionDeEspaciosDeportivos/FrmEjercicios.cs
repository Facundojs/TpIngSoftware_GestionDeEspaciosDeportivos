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
    public partial class FrmEjercicios : Form
    {
        private readonly UsuarioDTO _usuario;
        private readonly EjercicioManager _ejercicioManager;
        private EjercicioDTO _ejercicioSeleccionado;

        public FrmEjercicios(UsuarioDTO usuario)
        {
            InitializeComponent();
            _usuario = usuario;
            _ejercicioManager = new EjercicioManager();
        }

        private void FrmEjercicios_Load(object sender, EventArgs e)
        {
            ConfigurarUI();
            CargarEjercicios();
            ApplyPermissions();
            LimpiarControles();
        }

        private void ConfigurarUI()
        {
            this.Text = "FRM_EJERCICIOS_TITLE".Translate();
            lblNombre.Text = "LBL_NOMBRE_EJERCICIO".Translate();
            btnAgregar.Text = "BTN_AGREGAR".Translate();
            btnActualizar.Text = "BTN_ACTUALIZAR".Translate();
            btnEliminar.Text = "BTN_ELIMINAR".Translate();
            btnLimpiar.Text = "BTN_LIMPIAR".Translate();

            dgvEjercicios.AutoGenerateColumns = false;
            dgvEjercicios.Columns.Clear();
            dgvEjercicios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "LBL_NOMBRE_EJERCICIO".Translate(), Width = 200 });
        }

        private void ApplyPermissions()
        {
            btnAgregar.Enabled = _usuario.TienePermiso(PermisoKeys.EjercicioCrear);
            btnActualizar.Enabled = false; // Until selected
            btnEliminar.Enabled = false; // Until selected
        }

        private void CargarEjercicios()
        {
            try
            {
                var lista = _ejercicioManager.ListarEjercicios();
                dgvEjercicios.DataSource = null;
                dgvEjercicios.DataSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    MessageBox.Show("ERR_NOMBRE_EJERCICIO".Translate(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var dto = new EjercicioDTO { Nombre = txtNombre.Text.Trim() };
                _ejercicioManager.CrearEjercicio(dto);

                MessageBox.Show("MSG_EJERCICIO_CREADO".Translate(), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarEjercicios();
                LimpiarControles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (_ejercicioSeleccionado == null) return;

            try
            {
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    MessageBox.Show("ERR_NOMBRE_EJERCICIO".Translate(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _ejercicioSeleccionado.Nombre = txtNombre.Text.Trim();
                _ejercicioManager.ModificarEjercicio(_ejercicioSeleccionado);

                MessageBox.Show("MSG_EJERCICIO_ACTUALIZADO".Translate(), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarEjercicios();
                LimpiarControles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_ejercicioSeleccionado == null) return;

            try
            {
                if (MessageBox.Show("MSG_CONFIRM_ELIMINAR_EJERCICIO".Translate(), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _ejercicioManager.EliminarEjercicio(_ejercicioSeleccionado.Id);
                    MessageBox.Show("MSG_EJERCICIO_ELIMINADO".Translate(), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarEjercicios();
                    LimpiarControles();
                }
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
            txtNombre.Text = "";
            _ejercicioSeleccionado = null;

            btnAgregar.Enabled = _usuario.TienePermiso(PermisoKeys.EjercicioCrear);
            btnActualizar.Enabled = false;
            btnEliminar.Enabled = false;

            dgvEjercicios.ClearSelection();
        }

        private void dgvEjercicios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvEjercicios.SelectedRows.Count > 0)
            {
                _ejercicioSeleccionado = (EjercicioDTO)dgvEjercicios.SelectedRows[0].DataBoundItem;
                txtNombre.Text = _ejercicioSeleccionado.Nombre;

                btnAgregar.Enabled = false;
                btnActualizar.Enabled = _usuario.TienePermiso(PermisoKeys.EjercicioModificar);
                btnEliminar.Enabled = _usuario.TienePermiso(PermisoKeys.EjercicioEliminar);
            }
        }
    }
}
