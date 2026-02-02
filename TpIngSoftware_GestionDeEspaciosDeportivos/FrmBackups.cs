using Service.Domain.Composite;
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
    public partial class FrmBackups : Form
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
            this.Text = "Gestión de Backups".Translate();
            btnBackup.Text = "Realizar Backup".Translate();
            btnRestore.Text = "Realizar Restore".Translate();
            btnDelete.Text = "Borrar Backup".Translate();
            lblName.Text = "Nombre Archivo:".Translate();
        }

        private void FrmBackups_Load(object sender, EventArgs e)
        {
            ApplyPermissions();
            LoadBackups();
        }

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
                 MessageBox.Show("No tiene permiso para listar backups.".Translate());
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
                MessageBox.Show("Error cargando backups: " + ex.Message);
            }
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
             string filename = txtBackupName.Text;
             if (string.IsNullOrWhiteSpace(filename))
             {
                 MessageBox.Show("Ingrese un nombre para el backup.".Translate());
                 return;
             }

             try
             {
                 _backupService.Backup("IngSoftwareBase", filename);
                 MessageBox.Show("Backup realizado con éxito.".Translate());
                 LoadBackups();
                 txtBackupName.Text = "";
             }
             catch(Exception ex)
             {
                 MessageBox.Show("Error al realizar backup: " + ex.Message);
             }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
             if(dgvBackups.SelectedRows.Count == 0) return;
             var selected = (BackupFile)dgvBackups.SelectedRows[0].DataBoundItem;

             if(MessageBox.Show("¿Está seguro de restaurar la base de datos? Esto sobreescribirá los datos actuales.".Translate(), "Confirmar Restore".Translate(), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
             {
                 try
                 {
                     _backupService.Restore("IngSoftwareBase", selected.Nombre);
                     MessageBox.Show("Restore realizado con éxito. La aplicación se reiniciará.".Translate());
                     Application.Restart();
                 }
                 catch(Exception ex)
                 {
                     MessageBox.Show("Error al realizar restore: " + ex.Message);
                 }
             }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
             if(dgvBackups.SelectedRows.Count == 0) return;
             var selected = (BackupFile)dgvBackups.SelectedRows[0].DataBoundItem;

             if(MessageBox.Show("¿Está seguro de eliminar este backup?".Translate(), "Confirmar Eliminación".Translate(), MessageBoxButtons.YesNo) == DialogResult.Yes)
             {
                 try
                 {
                     _backupService.DeleteBackup(selected.Nombre);
                     LoadBackups();
                 }
                 catch(Exception ex)
                 {
                     MessageBox.Show("Error al eliminar backup: " + ex.Message);
                 }
             }
        }
    }
}
