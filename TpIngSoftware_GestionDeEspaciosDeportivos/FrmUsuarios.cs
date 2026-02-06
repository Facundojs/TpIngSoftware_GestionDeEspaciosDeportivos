using Domain.Composite;
using Service.DTO;
using Service.Facade.Extension;
using Service.Helpers;
using Service.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmUsuarios : Form
    {
        private readonly UsuarioDTO _currentUser;
        private readonly UsuarioService _usuarioService;
        private UsuarioDTO _selectedUser;

        public FrmUsuarios(UsuarioDTO currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _usuarioService = new UsuarioService();
            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            this.Text = "USERS_TITLE".Translate();
            lblUser.Text = "LBL_USER".Translate();
            lblPass.Text = "LBL_PASS".Translate();
            chkActive.Text = "CHK_ACTIVE".Translate();
            btnAdd.Text = "BTN_ADD".Translate();
            btnUpdate.Text = "BTN_UPDATE".Translate();
            btnDelete.Text = "BTN_DELETE".Translate();
            btnPermisos.Text = "BTN_PERMISSIONS".Translate();
            btnClear.Text = "BTN_CLEAR".Translate();
        }

        private void FrmUsuarios_Load(object sender, EventArgs e)
        {
            ApplyPermissions();
            LoadUsuarios();
        }

        private void ApplyPermissions()
        {
            if (_currentUser == null) return;
            btnAdd.Enabled = _currentUser.TienePermiso(PermisoKeys.UsuarioCrear);
            btnUpdate.Enabled = _currentUser.TienePermiso(PermisoKeys.UsuarioModificar);
            btnDelete.Enabled = _currentUser.TienePermiso(PermisoKeys.UsuarioEliminar);
            btnPermisos.Enabled = _currentUser.TienePermiso(PermisoKeys.PermisoAsignar);

            if (!_currentUser.TienePermiso(PermisoKeys.UsuarioListar))
            {
                MessageBox.Show("MSG_NO_PERM_USERS".Translate());
                this.Close();
            }
        }

        private void LoadUsuarios()
        {
            try
            {
                dgvUsuarios.DataSource = null;
                dgvUsuarios.DataSource = _usuarioService.GetUsuarios();
            }
            catch(Exception ex)
            {
                MessageBox.Show("MSG_ERR_LOAD_USERS".Translate() + ex.Message);
            }
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count > 0)
            {
                _selectedUser = (UsuarioDTO)dgvUsuarios.SelectedRows[0].DataBoundItem;
                txtUsername.Text = _selectedUser.Username;
                chkActive.Checked = _selectedUser.Estado == "Activo";
                txtPassword.Text = "";
                txtPassword.Enabled = false;
            }
            else
            {
                _selectedUser = null;
                ClearForm();
            }
        }

        private void ClearForm()
        {
            txtUsername.Text = "";
            txtPassword.Text = "";
            chkActive.Checked = true;
            txtPassword.Enabled = true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || (string.IsNullOrWhiteSpace(txtPassword.Text) && txtPassword.Enabled))
            {
                MessageBox.Show("MSG_COMPLETE_FIELDS".Translate());
                return;
            }

            try
            {
                var newUser = new UsuarioDTO
                {
                    Username = txtUsername.Text,
                    Permisos = new List<Acceso>()
                };

                _usuarioService.Register(newUser, txtPassword.Text);
                MessageBox.Show("MSG_USER_CREATED".Translate());
                LoadUsuarios();
                ClearForm();
            }
            catch(Exception ex)
            {
                MessageBox.Show("MSG_ERR_GENERIC".Translate() + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedUser == null) return;

            try
            {
                _selectedUser.Username = txtUsername.Text;
                _selectedUser.Estado = chkActive.Checked ? "Activo" : "Bloqueado";

                _usuarioService.Update(_selectedUser);

                MessageBox.Show("MSG_USER_UPDATED".Translate());
                LoadUsuarios();
            }
            catch(Exception ex)
            {
                MessageBox.Show("MSG_ERR_GENERIC".Translate() + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedUser == null) return;

            if (MessageBox.Show("MSG_CONFIRM_DELETE_USER".Translate(), "TITLE_CONFIRM".Translate(), MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    _usuarioService.Delete(_selectedUser.Id);
                    LoadUsuarios();
                    ClearForm();
                }
                catch(Exception ex)
                {
                     MessageBox.Show("MSG_ERR_GENERIC".Translate() + ex.Message);
                }
            }
        }

        private void btnPermisos_Click(object sender, EventArgs e)
        {
            if (_selectedUser == null) return;

            var frm = new FrmGestionPermisos(_selectedUser);
            frm.ShowDialog();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dgvUsuarios.ClearSelection();
            ClearForm();
            _selectedUser = null;
        }
    }
}
