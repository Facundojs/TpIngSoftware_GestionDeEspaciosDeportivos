using Domain.Composite;
using Domain.Enums;
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
    public partial class FrmBackups : Form, IRefreshable
    {
        private readonly UsuarioDTO _usuario;
        private readonly BackupService _backupService;

        public FrmBackups(UsuarioDTO usuario)
        {
            InitializeComponent();
            _usuario = usuario;
            _backupService = new BackupService();
            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            this.Text = Translations.BACKUP_TITLE.Translate();
            btnBackup.Text = Translations.BTN_BACKUP.Translate();
            btnRestore.Text = Translations.BTN_RESTORE.Translate();
            btnDelete.Text = Translations.BTN_DELETE_BACKUP.Translate();
            lblName.Text = Translations.LBL_FILENAME.Translate();
        }

        private void FrmBackups_Load(object sender, EventArgs e)
        {
            ApplyPermissions();
            LoadBackups();
        }

        public void RefreshData() => LoadBackups();

        private void ApplyPermissions()
        {
            if (_usuario == null) return;
            btnBackup.Enabled = _usuario.TienePermiso(PermisoKeys.BackupRealizar);
            btnRestore.Enabled = _usuario.TienePermiso(PermisoKeys.BackupRestore);
            btnDelete.Enabled = _usuario.TienePermiso(PermisoKeys.BackupBorrar);
        }

        private void LoadBackups()
        {
            if (!_usuario.TienePermiso(PermisoKeys.BackupListar))
            {
                 MessageBox.Show(Translations.MSG_NO_PERM_LIST.Translate());
                 this.Close();
                 return;
            }
            try
            {
                dgvBackups.DataSource = null;
                dgvBackups.DataSource = _backupService.ListBackups();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Translations.MSG_ERR_LOAD_BACKUP.Translate() + ex.Message);
            }
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
             string filename = txtBackupName.Text;
             if (string.IsNullOrWhiteSpace(filename))
             {
                 MessageBox.Show(Translations.MSG_ENTER_BACKUP_NAME.Translate());
                 return;
             }

             try
             {
                 _backupService.Backup("IngSoftwareBase", filename);
                 MessageBox.Show(Translations.MSG_BACKUP_SUCCESS.Translate());
                 LoadBackups();
                 txtBackupName.Text = "";
             }
             catch(Exception ex)
             {
                 MessageBox.Show(Translations.MSG_ERR_BACKUP.Translate() + ex.Message);
             }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
             if(dgvBackups.SelectedRows.Count == 0) return;
             var selected = (BackupFile)dgvBackups.SelectedRows[0].DataBoundItem;

             if(MessageBox.Show(Translations.MSG_CONFIRM_RESTORE.Translate(), Translations.TITLE_CONFIRM_RESTORE.Translate(), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
             {
                 try
                 {
                     _backupService.Restore("IngSoftwareBase", selected.Nombre);
                     MessageBox.Show(Translations.MSG_RESTORE_SUCCESS.Translate());
                     Application.Restart();
                 }
                 catch(Exception ex)
                 {
                     MessageBox.Show(Translations.MSG_ERR_RESTORE.Translate() + ex.Message);
                 }
             }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
             if(dgvBackups.SelectedRows.Count == 0) return;
             var selected = (BackupFile)dgvBackups.SelectedRows[0].DataBoundItem;

             if(MessageBox.Show(Translations.MSG_CONFIRM_DELETE_BACKUP.Translate(), Translations.TITLE_CONFIRM_DELETE.Translate(), MessageBoxButtons.YesNo) == DialogResult.Yes)
             {
                 try
                 {
                     _backupService.DeleteBackup(selected.Nombre);
                     LoadBackups();
                 }
                 catch(Exception ex)
                 {
                     MessageBox.Show(Translations.MSG_ERR_DELETE_BACKUP.Translate() + ex.Message);
                 }
             }
        }
    }
}
