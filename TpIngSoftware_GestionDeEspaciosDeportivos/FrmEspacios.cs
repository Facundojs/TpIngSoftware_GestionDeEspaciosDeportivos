using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BLL.DTOs;
using TpIngSoftware_GestionDeEspaciosDeportivos.Business;
using Service.DTO;
using Service.Facade.Extension;
using Domain;
using Service.Helpers;
using Domain.Enums;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmEspacios : Form, IRefreshable, ITranslatable
    {
        private readonly UsuarioDTO _currentUser;
        private readonly EspacioManager _espacioManager;
        private EspacioDTO _selectedEspacio;

        public FrmEspacios(UsuarioDTO currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _espacioManager = new EspacioManager();
            UpdateLanguage();
        }

        public void UpdateLanguage()
        {
            this.Text = "FRM_ESPACIO_TITLE".Translate();
            lblNombre.Text = "LBL_NOMBRE".Translate();
            lblDescripcion.Text = "LBL_DESCRIPCION".Translate();
            lblPrecioHora.Text = "LBL_PRECIO_HORA".Translate();
            btnCrear.Text = "BTN_CREAR".Translate();
            btnActualizar.Text = "BTN_ACTUALIZAR".Translate();
            btnEliminar.Text = "BTN_DELETE".Translate();
            btnLimpiar.Text = "BTN_LIMPIAR".Translate();
            btnAgenda.Text = "BTN_CONFIGURAR_AGENDA".Translate();
        }

        private void FrmEspacios_Load(object sender, EventArgs e)
        {
            ApplyPermissions();
            LoadEspacios();
        }

        public void RefreshData() => LoadEspacios();

        private void ApplyPermissions()
        {
            if (_currentUser == null) return;

            btnCrear.Enabled = _currentUser.TienePermiso(PermisoKeys.EspacioCrear);
            btnActualizar.Enabled = _currentUser.TienePermiso(PermisoKeys.EspacioModificar);
            btnEliminar.Enabled = _currentUser.TienePermiso(PermisoKeys.EspacioEliminar);
            btnAgenda.Enabled = _currentUser.TienePermiso(PermisoKeys.EspacioModificar);

            if (!_currentUser.TienePermiso(PermisoKeys.EspacioListar))
            {
                MessageBox.Show("MSG_NO_PERM_LIST".Translate(), "TITLE_WARNING".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }

        private void LoadEspacios()
        {
            try
            {
                var list = _espacioManager.ListarTodos();
                dgvEspacios.DataSource = null;
                dgvEspacios.DataSource = list;

                if (dgvEspacios.Columns["Id"] != null) dgvEspacios.Columns["Id"].Visible = false;
                if (dgvEspacios.Columns["Nombre"] != null) dgvEspacios.Columns["Nombre"].HeaderText = "LBL_NOMBRE".Translate();
                if (dgvEspacios.Columns["Descripcion"] != null) dgvEspacios.Columns["Descripcion"].HeaderText = "LBL_DESCRIPCION".Translate();
                if (dgvEspacios.Columns["PrecioHora"] != null) dgvEspacios.Columns["PrecioHora"].HeaderText = "LBL_PRECIO_HORA".Translate();
                if (dgvEspacios.Columns["Estado"] != null) dgvEspacios.Columns["Estado"].HeaderText = "LBL_ESTADO".Translate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("MSG_ERR_GENERIC".Translate() + " " + ex.Message, "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvEspacios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvEspacios.SelectedRows.Count > 0)
            {
                _selectedEspacio = (EspacioDTO)dgvEspacios.SelectedRows[0].DataBoundItem;
                PopulateForm(_selectedEspacio);
            }
            else
            {
                _selectedEspacio = null;
                ClearForm();
            }
        }

        private void PopulateForm(EspacioDTO dto)
        {
            txtNombre.Text = dto.Nombre;
            txtDescripcion.Text = dto.Descripcion;
            txtPrecioHora.Text = dto.PrecioHora.ToString("0.00");
        }

        private void ClearForm()
        {
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            txtPrecioHora.Text = "";
            errorProvider.Clear();
        }

        private bool ValidateForm()
        {
            bool isValid = true;
            errorProvider.Clear();

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                errorProvider.SetError(txtNombre, "ERR_REQUIRED_FIELD".Translate());
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtPrecioHora.Text) || !decimal.TryParse(txtPrecioHora.Text, out decimal precio) || precio < 0)
            {
                errorProvider.SetError(txtPrecioHora, "ERR_INVALID_NUMBER".Translate());
                isValid = false;
            }

            return isValid;
        }

        private EspacioDTO GetFormData()
        {
            return new EspacioDTO
            {
                Nombre = txtNombre.Text,
                Descripcion = txtDescripcion.Text,
                PrecioHora = decimal.Parse(txtPrecioHora.Text),
                Estado = _selectedEspacio?.Estado ?? EstadoEspacio.Activo
            };
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                var dto = GetFormData();
                _espacioManager.CrearEspacio(dto);
                MessageBox.Show("MSG_ESPACIO_CREATED".Translate(), "TITLE_INFO".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadEspacios();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("MSG_ERR_GENERIC".Translate() + " " + ex.Message, "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (_selectedEspacio == null) return;
            if (!ValidateForm()) return;

            try
            {
                var dto = GetFormData();
                dto.Id = _selectedEspacio.Id;

                _espacioManager.ActualizarEspacio(dto);
                MessageBox.Show("MSG_ESPACIO_UPDATED".Translate(), "TITLE_INFO".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadEspacios();
            }
            catch (Exception ex)
            {
                MessageBox.Show("MSG_ERR_GENERIC".Translate() + " " + ex.Message, "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_selectedEspacio == null) return;

            if (MessageBox.Show("MSG_CONFIRM_DELETE_ESPACIO".Translate(), "TITLE_CONFIRM".Translate(), MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    _espacioManager.EliminarEspacio(_selectedEspacio.Id);
                    MessageBox.Show("MSG_ESPACIO_DELETED".Translate(), "TITLE_INFO".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadEspacios();
                    ClearForm();
                    _selectedEspacio = null;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("reservas futuras"))
                    {
                        MessageBox.Show("ERR_ESPACIO_CON_RESERVAS".Translate(), "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("MSG_ERR_GENERIC".Translate() + " " + ex.Message, "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            dgvEspacios.ClearSelection();
            ClearForm();
            _selectedEspacio = null;
        }

        private void btnAgenda_Click(object sender, EventArgs e)
        {
            if (_selectedEspacio == null)
            {
                MessageBox.Show("MSG_SELECCIONE_ESPACIO".Translate(), "TITLE_WARNING".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var frm = new FrmAgenda(_selectedEspacio.Id);
            frm.ShowDialog();
        }
    }
}
