using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Service;
using Domain;
using Service.Impl;
using Service.Impl.SqlServer;
using Service.Facade.Extension;
using Service.DTO;
using Domain.Composite;
using Service.Helpers;
using Service.Logic;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class Form1: Form
    {
        private UsuarioDTO _usuario;
        private MenuStrip _menuStrip;
        private ToolStripMenuItem _menuAdmin;
        private ToolStripMenuItem _menuBackups;
        private ToolStripMenuItem _menuUsuarios;
        private ToolStripMenuItem _menuBitacora;
        private ToolStripMenuItem _menuMembresias;
        private ToolStripMenuItem _menuClientes;

        public Form1(UsuarioDTO usuario)
        {
            InitializeComponent();
            _usuario = usuario;
            SetupMenu();
            UpdateLanguage();
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void SetupMenu()
        {
            _menuStrip = new MenuStrip();
            _menuAdmin = new ToolStripMenuItem("MENU_ADMIN".Translate());

            _menuBackups = new ToolStripMenuItem("MENU_BACKUPS".Translate());
            _menuBackups.Click += (s, e) => OpenBackups();

            _menuUsuarios = new ToolStripMenuItem("MENU_USERS".Translate());
            _menuUsuarios.Click += (s, e) => OpenUsuarios();

            _menuBitacora = new ToolStripMenuItem("MENU_BITACORA".Translate());
            _menuBitacora.Click += (s, e) => OpenBitacora();

            _menuMembresias = new ToolStripMenuItem("MENU_MEMBRESIA".Translate());
            _menuMembresias.Click += (s, e) => OpenMembresias();

            _menuClientes = new ToolStripMenuItem("CLIENTE_TITLE".Translate());
            _menuClientes.Click += (s, e) => OpenClientes();

            _menuAdmin.DropDownItems.Add(_menuBackups);
            _menuAdmin.DropDownItems.Add(_menuUsuarios);
            _menuAdmin.DropDownItems.Add(_menuBitacora);
            _menuAdmin.DropDownItems.Add(_menuMembresias);
            _menuAdmin.DropDownItems.Add(_menuClientes);

            _menuStrip.Items.Add(_menuAdmin);

            this.Controls.Add(_menuStrip);
            this.MainMenuStrip = _menuStrip;
        }

        private void UpdateLanguage()
        {
            this.Text = "MAIN_TITLE".Translate();
            if(_menuAdmin != null) _menuAdmin.Text = "MENU_ADMIN".Translate();
            if(_menuBackups != null) _menuBackups.Text = "MENU_BACKUPS".Translate();
            if(_menuUsuarios != null) _menuUsuarios.Text = "MENU_USERS".Translate();
            if(_menuBitacora != null) _menuBitacora.Text = "MENU_BITACORA".Translate();
            if(_menuMembresias != null) _menuMembresias.Text = "MENU_MEMBRESIA".Translate();
            if(_menuClientes != null) _menuClientes.Text = "CLIENTE_TITLE".Translate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                new PermisosService().EnsurePermissions();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error seeding permissions: " + ex.Message);
            }
            ApplyPermissions();
        }

        private void ApplyPermissions()
        {
            if (_usuario == null)
            {
                if(_menuAdmin != null) _menuAdmin.Visible = false;
                return;
            }

            bool canBackup = _usuario.TienePermiso(PermisoKeys.BackupListar);
            bool canManageUsers = _usuario.TienePermiso(PermisoKeys.UsuarioListar);
            bool canViewLogs = _usuario.TienePermiso(PermisoKeys.BitacoraVer);
            bool canManageMembresias = _usuario.TienePermiso(PermisoKeys.MembresiaListar);
            bool canManageClientes = _usuario.TienePermiso(PermisoKeys.ClienteListar);

            if(_menuBackups != null) _menuBackups.Visible = canBackup;
            if(_menuUsuarios != null) _menuUsuarios.Visible = canManageUsers;
            if(_menuBitacora != null) _menuBitacora.Visible = canViewLogs;
            if(_menuMembresias != null) _menuMembresias.Visible = canManageMembresias;
            if(_menuClientes != null) _menuClientes.Visible = canManageClientes;

            if(_menuAdmin != null) _menuAdmin.Visible = canBackup || canManageUsers || canViewLogs || canManageMembresias || canManageClientes;
        }

        private void OpenClientes()
        {
            var frm = new FrmClientes(_usuario);
            frm.ShowDialog();
        }

        private void OpenMembresias()
        {
            var frm = new FrmMembresias(_usuario);
            frm.ShowDialog();
        }

        private void OpenBitacora()
        {
            var frm = new FrmBitacora(_usuario);
            frm.ShowDialog();
        }

        private void OpenBackups()
        {
             var frm = new FrmBackups(_usuario);
             frm.ShowDialog();
        }

        private void OpenUsuarios()
        {
             var frm = new FrmUsuarios(_usuario);
             frm.ShowDialog();
        }
    }
}
