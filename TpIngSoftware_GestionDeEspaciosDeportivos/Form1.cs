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
using Service.Factory;
using System.Threading;
using System.Globalization;
using TpIngSoftware_GestionDeEspaciosDeportivos.Helpers;

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
        private ToolStripMenuItem _menuRutinas;
        private ToolStripMenuItem _menuEspacios;
        private ToolStripMenuItem _menuIdioma;

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

            _menuRutinas = new ToolStripMenuItem("MENU_RUTINAS".Translate());
            _menuRutinas.Click += (s, e) => OpenRutinas();

            _menuEspacios = new ToolStripMenuItem("MENU_ESPACIOS".Translate());
            _menuEspacios.Click += (s, e) => OpenEspacios();

            _menuAdmin.DropDownItems.Add(_menuBackups);
            _menuAdmin.DropDownItems.Add(_menuUsuarios);
            _menuAdmin.DropDownItems.Add(_menuBitacora);
            _menuAdmin.DropDownItems.Add(_menuMembresias);
            _menuAdmin.DropDownItems.Add(_menuClientes);
            _menuAdmin.DropDownItems.Add(_menuRutinas);
            _menuAdmin.DropDownItems.Add(_menuEspacios);

            // Menu Idioma
            _menuIdioma = new ToolStripMenuItem("Idioma");
            var languages = FactoryDao.LanguageRepository.GetLanguages();
            foreach (var lang in languages)
            {
                var item = new ToolStripMenuItem(lang.Value);
                item.Tag = lang.Key;
                item.Image = FlagHelper.GetFlag(lang.Key);
                item.Click += (s, e) => ChangeLanguage(lang.Key);
                _menuIdioma.DropDownItems.Add(item);
            }

            _menuStrip.Items.Add(_menuAdmin);
            _menuStrip.Items.Add(_menuIdioma);

            this.Controls.Add(_menuStrip);
            this.MainMenuStrip = _menuStrip;
        }

        private void ChangeLanguage(string code)
        {
            try
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(code);
                FactoryDao.LanguageRepository.SaveUserLanguage(code);
                UpdateLanguage();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error changing language: " + ex.Message);
            }
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
            if(_menuRutinas != null) _menuRutinas.Text = "MENU_RUTINAS".Translate();
            if(_menuEspacios != null) _menuEspacios.Text = "MENU_ESPACIOS".Translate();
            if (_menuIdioma != null) _menuIdioma.Text = Thread.CurrentThread.CurrentUICulture.NativeName;
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
            bool canManageRutinas = _usuario.TienePermiso(PermisoKeys.RutinaVer);
            bool canManageEspacios = _usuario.TienePermiso(PermisoKeys.EspacioListar);

            if(_menuBackups != null) _menuBackups.Visible = canBackup;
            if(_menuUsuarios != null) _menuUsuarios.Visible = canManageUsers;
            if(_menuBitacora != null) _menuBitacora.Visible = canViewLogs;
            if(_menuMembresias != null) _menuMembresias.Visible = canManageMembresias;
            if(_menuClientes != null) _menuClientes.Visible = canManageClientes;
            if(_menuRutinas != null) _menuRutinas.Visible = canManageRutinas;
            if(_menuEspacios != null) _menuEspacios.Visible = canManageEspacios;

            if(_menuAdmin != null) _menuAdmin.Visible = canBackup || canManageUsers || canViewLogs || canManageMembresias || canManageClientes || canManageRutinas || canManageEspacios;
        }

        private void OpenRutinas()
        {
            var frm = new FrmGestionRutinas(_usuario);
            frm.ShowDialog();
        }

        private void OpenEspacios()
        {
            var frm = new FrmEspacios(_usuario);
            frm.ShowDialog();
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
