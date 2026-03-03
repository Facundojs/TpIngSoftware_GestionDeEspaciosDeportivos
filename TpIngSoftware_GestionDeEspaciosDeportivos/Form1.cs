using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using TpIngSoftware_GestionDeEspaciosDeportivos.Helpers;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class Form1: Form
    {
        private UsuarioDTO _usuario;
        private MenuStrip _menuStrip;
        private TabControl _tabControl;
        private ToolStripMenuItem _menuAdmin;
        private ToolStripMenuItem _menuBackups;
        private ToolStripMenuItem _menuUsuarios;
        private ToolStripMenuItem _menuBitacora;
        private ToolStripMenuItem _menuMembresias;
        private ToolStripMenuItem _menuClientes;
        private ToolStripMenuItem _menuFamilias;
        private ToolStripMenuItem _menuRutinas;
        private ToolStripMenuItem _menuEspacios;
        private ToolStripMenuItem _menuPagos;
        private ToolStripMenuItem _menuReservas;
        private ToolStripMenuItem _menuIngresos;
        private ToolStripComboBox _cmbLanguage;
        private LanguageService _languageService;

        public Form1(UsuarioDTO usuario)
        {
            InitializeComponent();
            _usuario = usuario;
            SetupTabControl();
            SetupMenu();
            UpdateLanguage();
        }

        public Form1()
        {
            InitializeComponent();
            SetupTabControl();
        }

        private void SetupTabControl()
        {
            _tabControl = new TabControl();
            _tabControl.Dock = DockStyle.Fill;
            this.Controls.Add(_tabControl);
        }

        private void SetupMenu()
        {
            _menuStrip = new MenuStrip();
            _menuAdmin = new ToolStripMenuItem(Domain.Enums.Translations.MENU_ADMIN.Translate());

            _menuBackups = new ToolStripMenuItem(Domain.Enums.Translations.MENU_BACKUPS.Translate());
            _menuBackups.Click += (s, e) => OpenBackups();

            _menuUsuarios = new ToolStripMenuItem(Domain.Enums.Translations.MENU_USERS.Translate());
            _menuUsuarios.Click += (s, e) => OpenUsuarios();

            _menuBitacora = new ToolStripMenuItem(Domain.Enums.Translations.MENU_BITACORA.Translate());
            _menuBitacora.Click += (s, e) => OpenBitacora();

            _menuMembresias = new ToolStripMenuItem(Domain.Enums.Translations.MENU_MEMBRESIA.Translate());
            _menuMembresias.Click += (s, e) => OpenMembresias();

            _menuClientes = new ToolStripMenuItem(Domain.Enums.Translations.CLIENTE_TITLE.Translate());
            _menuClientes.Click += (s, e) => OpenClientes();

            _menuFamilias = new ToolStripMenuItem(Domain.Enums.Translations.PERMISSIONS_TITLE.Translate());
            _menuFamilias.Click += (s, e) => OpenFamilias();

            _menuRutinas = new ToolStripMenuItem(Domain.Enums.Translations.MENU_RUTINAS.Translate());
            _menuRutinas.Click += (s, e) => OpenRutinas();

            _menuEspacios = new ToolStripMenuItem(Domain.Enums.Translations.MENU_ESPACIOS.Translate());
            _menuEspacios.Click += (s, e) => OpenEspacios();

            _menuPagos = new ToolStripMenuItem(Domain.Enums.Translations.MENU_PAGOS.Translate());
            _menuPagos.Click += (s, e) => OpenPagos();

            _menuReservas = new ToolStripMenuItem(Domain.Enums.Translations.MENU_RESERVAS.Translate());
            _menuReservas.Click += (s, e) => OpenReservas();

            _menuIngresos = new ToolStripMenuItem(Domain.Enums.Translations.MENU_INGRESOS.Translate());
            _menuIngresos.Click += (s, e) => OpenIngresos();

            _menuAdmin.DropDownItems.Add(_menuBackups);
            _menuAdmin.DropDownItems.Add(_menuUsuarios);
            _menuAdmin.DropDownItems.Add(_menuBitacora);
            _menuAdmin.DropDownItems.Add(_menuMembresias);
            _menuAdmin.DropDownItems.Add(_menuClientes);
            _menuAdmin.DropDownItems.Add(_menuFamilias);
            _menuAdmin.DropDownItems.Add(_menuRutinas);
            _menuAdmin.DropDownItems.Add(_menuEspacios);
            _menuAdmin.DropDownItems.Add(_menuPagos);
            _menuAdmin.DropDownItems.Add(_menuReservas);
            _menuAdmin.DropDownItems.Add(_menuIngresos);

            _menuStrip.Items.Add(_menuAdmin);

            // Language Selector
            _languageService = new LanguageService();
            _cmbLanguage = new ToolStripComboBox();
            _cmbLanguage.Alignment = ToolStripItemAlignment.Right;
            _cmbLanguage.Width = 200;

            LanguageSelectorHelper.SetupToolStripComboBox(_cmbLanguage, _languageService, UpdateLanguage);

            _menuStrip.Items.Add(_cmbLanguage);

            this.Controls.Add(_menuStrip);
            this.MainMenuStrip = _menuStrip;
        }

        private void UpdateLanguage()
        {
            this.Text = Domain.Enums.Translations.MAIN_TITLE.Translate();
            if(_menuAdmin != null) _menuAdmin.Text = Domain.Enums.Translations.MENU_ADMIN.Translate();
            if(_menuBackups != null) _menuBackups.Text = Domain.Enums.Translations.MENU_BACKUPS.Translate();
            if(_menuUsuarios != null) _menuUsuarios.Text = Domain.Enums.Translations.MENU_USERS.Translate();
            if(_menuBitacora != null) _menuBitacora.Text = Domain.Enums.Translations.MENU_BITACORA.Translate();
            if(_menuMembresias != null) _menuMembresias.Text = Domain.Enums.Translations.MENU_MEMBRESIA.Translate();
            if(_menuClientes != null) _menuClientes.Text = Domain.Enums.Translations.CLIENTE_TITLE.Translate();
            if(_menuFamilias != null) _menuFamilias.Text = Domain.Enums.Translations.PERMISSIONS_TITLE.Translate();
            if(_menuRutinas != null) _menuRutinas.Text = Domain.Enums.Translations.MENU_RUTINAS.Translate();
            if(_menuEspacios != null) _menuEspacios.Text = Domain.Enums.Translations.MENU_ESPACIOS.Translate();
            if(_menuPagos != null) _menuPagos.Text = Domain.Enums.Translations.MENU_PAGOS.Translate();
            if(_menuReservas != null) _menuReservas.Text = Domain.Enums.Translations.MENU_RESERVAS.Translate();
            if(_menuIngresos != null) _menuIngresos.Text = Domain.Enums.Translations.MENU_INGRESOS.Translate();

            // Update Tab Titles
            if (_tabControl != null)
            {
                foreach (TabPage tab in _tabControl.TabPages)
                {
                    if (tab.Tag is Domain.Enums.Translations transKey)
                    {
                        tab.Text = transKey.Translate();
                    }
                }
            }
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
            bool canManagePermissions = _usuario.TienePermiso(PermisoKeys.PermisoAsignar);
            bool canViewLogs = _usuario.TienePermiso(PermisoKeys.BitacoraVer);
            bool canManageMembresias = _usuario.TienePermiso(PermisoKeys.MembresiaListar);
            bool canManageClientes = _usuario.TienePermiso(PermisoKeys.ClienteListar);
            bool canManageRutinas = _usuario.TienePermiso(PermisoKeys.RutinaVer);
            bool canManageEspacios = _usuario.TienePermiso(PermisoKeys.EspacioListar);
            bool canManagePagos = _usuario.TienePermiso(PermisoKeys.PagoListar);
            bool canManageReservas = _usuario.TienePermiso(PermisoKeys.ReservaListar);
            bool canManageIngresos = _usuario.TienePermiso(PermisoKeys.IngresoListar);

            if(_menuBackups != null) _menuBackups.Visible = canBackup;
            if(_menuUsuarios != null) _menuUsuarios.Visible = canManageUsers;
            if(_menuBitacora != null) _menuBitacora.Visible = canViewLogs;
            if(_menuMembresias != null) _menuMembresias.Visible = canManageMembresias;
            if(_menuClientes != null) _menuClientes.Visible = canManageClientes;
            if(_menuFamilias != null) _menuFamilias.Visible = canManagePermissions;
            if(_menuRutinas != null) _menuRutinas.Visible = canManageRutinas;
            if(_menuEspacios != null) _menuEspacios.Visible = canManageEspacios;
            if(_menuPagos != null) _menuPagos.Visible = canManagePagos;
            if(_menuReservas != null) _menuReservas.Visible = canManageReservas;
            if(_menuIngresos != null) _menuIngresos.Visible = canManageIngresos;

            if(_menuAdmin != null) _menuAdmin.Visible = canBackup || canManageUsers || canManagePermissions || canViewLogs || canManageMembresias || canManageClientes || canManageRutinas || canManageEspacios || canManagePagos || canManageReservas || canManageIngresos;
        }

        private void OpenFormInTab<T>(Domain.Enums.Translations transKey, Func<T> formFactory) where T : Form
        {
            // Check if tab already exists
            foreach (TabPage tab in _tabControl.TabPages)
            {
                if (tab.Tag is Domain.Enums.Translations tagKey && tagKey == transKey)
                {
                    _tabControl.SelectedTab = tab;
                    return;
                }
            }

            // Create new tab
            TabPage newTab = new TabPage(transKey.Translate());
            newTab.Tag = transKey;
            T form = formFactory();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            
            newTab.Controls.Add(form);
            _tabControl.TabPages.Add(newTab);
            _tabControl.SelectedTab = newTab;
            
            form.Show();
        }

        private void OpenIngresos()
        {
            OpenFormInTab(Domain.Enums.Translations.MENU_INGRESOS, () => new FrmIngresos(_usuario));
        }

        private void OpenPagos()
        {
            OpenFormInTab(Domain.Enums.Translations.MENU_PAGOS, () => new FrmPagos(_usuario));
        }

        private void OpenReservas()
        {
            OpenFormInTab(Domain.Enums.Translations.MENU_RESERVAS, () => new FrmReservas(_usuario));
        }

        private void OpenRutinas()
        {
            OpenFormInTab(Domain.Enums.Translations.MENU_RUTINAS, () => new FrmGestionRutinas(_usuario));
        }

        private void OpenEspacios()
        {
            OpenFormInTab(Domain.Enums.Translations.MENU_ESPACIOS, () => new FrmEspacios(_usuario));
        }

        private void OpenClientes()
        {
            OpenFormInTab(Domain.Enums.Translations.CLIENTE_TITLE, () => new FrmClientes(_usuario));
        }

        private void OpenMembresias()
        {
            OpenFormInTab(Domain.Enums.Translations.MENU_MEMBRESIA, () => new FrmMembresias(_usuario));
        }

        private void OpenBitacora()
        {
            OpenFormInTab(Domain.Enums.Translations.MENU_BITACORA, () => new FrmBitacora(_usuario));
        }

        private void OpenBackups()
        {
             OpenFormInTab(Domain.Enums.Translations.MENU_BACKUPS, () => new FrmBackups(_usuario));
        }

        private void OpenUsuarios()
        {
             OpenFormInTab(Domain.Enums.Translations.MENU_USERS, () => new FrmUsuarios(_usuario));
        }

        private void OpenFamilias()
        {
             OpenFormInTab(Domain.Enums.Translations.PERMISSIONS_TITLE, () => new FrmGestionFamilias(_usuario));
        }
    }
}
