using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BLL.Services;
using BLL.DTOs;
using TpIngSoftware_GestionDeEspaciosDeportivos.Business;
using Service.DTO;
using Domain.Composite;
using Service.Facade.Extension;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmMembresias : Form
    {
        private readonly UsuarioDTO _currentUser;
        private readonly MembresiaManager _membresiaManager;
        private MembresiaDTO _selectedMembresia;

        public FrmMembresias(UsuarioDTO currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _membresiaManager = new MembresiaManager();
            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            this.Text = "FRM_MEMBRESIA_TITLE".Translate();
            lblCodigo.Text = "LBL_CODIGO".Translate();
            lblNombre.Text = "LBL_NOMBRE".Translate();
            lblPrecio.Text = "LBL_PRECIO".Translate();
            lblRegularidad.Text = "LBL_REGULARIDAD".Translate();
            lblDetalle.Text = "LBL_DETALLE".Translate();
            btnCrear.Text = "BTN_CREAR".Translate();
            btnActualizar.Text = "BTN_ACTUALIZAR".Translate();
            btnDeshabilitar.Text = "BTN_DESHABILITAR".Translate();
            btnLimpiar.Text = "BTN_LIMPIAR".Translate();
        }

        private void FrmMembresias_Load(object sender, EventArgs e)
        {
            ApplyPermissions();
            LoadMembresias();
        }

        private void ApplyPermissions()
        {
            if (_currentUser == null) return;

            btnCrear.Enabled = _currentUser.TienePermiso(PermisoKeys.MembresiaCrear);
            btnActualizar.Enabled = _currentUser.TienePermiso(PermisoKeys.MembresiaModificar);
            btnDeshabilitar.Enabled = _currentUser.TienePermiso(PermisoKeys.MembresiaDeshabilitar);

            if (!_currentUser.TienePermiso(PermisoKeys.MembresiaListar))
            {
                MessageBox.Show("MSG_NO_PERM_LIST".Translate());
                this.Close();
            }
        }

        private void LoadMembresias()
        {
            try
            {
                var list = _membresiaManager.ListarMembresias(false);
                dgvMembresias.DataSource = null;
                dgvMembresias.DataSource = list;

                // Hide ID column if exists
                if(dgvMembresias.Columns["Id"] != null) dgvMembresias.Columns["Id"].Visible = false;
                if(dgvMembresias.Columns["PrecioFormateado"] != null) dgvMembresias.Columns["PrecioFormateado"].HeaderText = "LBL_PRECIO".Translate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading memberships: " + ex.Message);
            }
        }

        private void dgvMembresias_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMembresias.SelectedRows.Count > 0)
            {
                _selectedMembresia = (MembresiaDTO)dgvMembresias.SelectedRows[0].DataBoundItem;
                PopulateForm(_selectedMembresia);
            }
            else
            {
                _selectedMembresia = null;
                ClearForm();
            }
        }

        private void PopulateForm(MembresiaDTO dto)
        {
            txtCodigo.Text = dto.Codigo.ToString();
            txtNombre.Text = dto.Nombre;
            txtPrecio.Text = dto.Precio.ToString("0.00");
            txtRegularidad.Text = dto.Regularidad.ToString();
            txtDetalle.Text = dto.Detalle;

            // Lock code for editing if it's supposed to be unique/immutable, but here I'll leave it open as per requirements imply only updates validation.
            // Usually ID is not editable, but requirements said "Actualizar... validar cambios".
            // If code is business key, maybe we shouldn't change it, but I'll follow standard ABM.
        }

        private void ClearForm()
        {
            txtCodigo.Text = "";
            txtNombre.Text = "";
            txtPrecio.Text = "";
            txtRegularidad.Text = "";
            txtDetalle.Text = "";
            errorProvider.Clear();
        }

        private bool ValidateForm()
        {
            bool isValid = true;
            errorProvider.Clear();

            if (string.IsNullOrWhiteSpace(txtCodigo.Text) || !int.TryParse(txtCodigo.Text, out int cod) || cod <= 0)
            {
                errorProvider.SetError(txtCodigo, "ERR_INVALID_NUMBER".Translate());
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                errorProvider.SetError(txtNombre, "ERR_REQUIRED_FIELD".Translate());
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtPrecio.Text) || !decimal.TryParse(txtPrecio.Text, out decimal precio) || precio <= 0)
            {
                errorProvider.SetError(txtPrecio, "ERR_INVALID_NUMBER".Translate());
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtRegularidad.Text) || !int.TryParse(txtRegularidad.Text, out int reg) || reg <= 0)
            {
                errorProvider.SetError(txtRegularidad, "ERR_INVALID_NUMBER".Translate());
                isValid = false;
            }

            return isValid;
        }

        private MembresiaDTO GetFormData()
        {
            return new MembresiaDTO
            {
                Codigo = int.Parse(txtCodigo.Text),
                Nombre = txtNombre.Text,
                Precio = decimal.Parse(txtPrecio.Text),
                Regularidad = int.Parse(txtRegularidad.Text),
                Detalle = txtDetalle.Text,
                Activa = true // Default for new, existing will override
            };
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                var dto = GetFormData();
                // ID is handled in service
                _membresiaManager.CrearMembresia(dto);
                MessageBox.Show("MSG_MEMBRESIA_CREATED".Translate());
                LoadMembresias();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (_selectedMembresia == null) return;
            if (!ValidateForm()) return;

            try
            {
                var dto = GetFormData();
                dto.Id = _selectedMembresia.Id;
                dto.Activa = _selectedMembresia.Activa; // Preserve state

                _membresiaManager.ActualizarMembresia(dto);
                MessageBox.Show("MSG_MEMBRESIA_UPDATED".Translate());
                LoadMembresias();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnDeshabilitar_Click(object sender, EventArgs e)
        {
            if (_selectedMembresia == null) return;

            if (MessageBox.Show("MSG_CONFIRM_DESHABILITAR".Translate(), "TITLE_CONFIRM".Translate(), MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    _membresiaManager.DeshabilitarMembresia(_selectedMembresia.Id);
                    MessageBox.Show("MSG_MEMBRESIA_DISABLED".Translate());
                    LoadMembresias();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            dgvMembresias.ClearSelection();
            ClearForm();
            _selectedMembresia = null;
        }
    }
}
