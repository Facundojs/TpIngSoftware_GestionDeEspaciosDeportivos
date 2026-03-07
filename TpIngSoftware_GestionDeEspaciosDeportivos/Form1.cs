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
using Domain;
using Service.Helpers;
using Service.Logic;
using TpIngSoftware_GestionDeEspaciosDeportivos.Helpers;
using Domain.Enums;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class Form1: Form
    {
        private UsuarioDTO _usuario;
        private TabControl _tabControlMain;
        private TabControl _tabControlNegocio;
        private TabControl _tabControlAdmin;
        
        private Dictionary<string, TabPage> _allTabs;
        private TabPage _tabPageNegocio;
        private TabPage _tabPageAdmin;

        private ToolStripComboBox _cmbLanguage;
        private LanguageService _languageService;

        public Form1(UsuarioDTO usuario)
        {
            InitializeComponent();
            _usuario = usuario;
            _allTabs = new Dictionary<string, TabPage>();
            SetupUIStructure();
            InitializeAllTabs();
            UpdateLanguage();
            ApplyPermissions();
        }

        public Form1()
        {
            InitializeComponent();
            _allTabs = new Dictionary<string, TabPage>();
            SetupUIStructure();
        }

        private void SetupUIStructure()
        {
            var menuStrip = new MenuStrip { Dock = DockStyle.Top };
            
            _languageService = new LanguageService();
            _cmbLanguage = new ToolStripComboBox { Alignment = ToolStripItemAlignment.Right, Width = 200 };
            LanguageSelectorHelper.SetupToolStripComboBox(_cmbLanguage, _languageService, UpdateLanguage);
            
            if (_usuario != null)
            {
                var lblUser = new ToolStripMenuItem($"[{_usuario.RolNegocio}] {_usuario.Username}") { Enabled = false, Alignment = ToolStripItemAlignment.Left };
                menuStrip.Items.Add(lblUser);
            }

            menuStrip.Items.Add(_cmbLanguage);
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            _tabControlMain = new TabControl { Dock = DockStyle.Fill };
            _tabPageNegocio = new TabPage("MAIN_NEGOCIO".Translate()) { Tag = "MAIN_NEGOCIO "};
            _tabPageAdmin = new TabPage("Administración") { Tag = "MENU_ADMIN "};
            
            _tabControlMain.TabPages.Add(_tabPageNegocio);
            _tabControlMain.TabPages.Add(_tabPageAdmin);
            
            this.Controls.Add(_tabControlMain);
            _tabControlMain.BringToFront();

            _tabControlNegocio = new TabControl { Dock = DockStyle.Fill };
            _tabPageNegocio.Controls.Add(_tabControlNegocio);

            _tabControlAdmin = new TabControl { Dock = DockStyle.Fill };
            _tabPageAdmin.Controls.Add(_tabControlAdmin);

            _tabControlMain.SelectedIndexChanged += TabMain_SelectedIndexChanged;
            _tabControlNegocio.SelectedIndexChanged += TabInner_SelectedIndexChanged;
            _tabControlAdmin.SelectedIndexChanged += TabInner_SelectedIndexChanged;
        }

        private void InitializeAllTabs()
        {
            CreateFixedTab(_tabControlNegocio, "MENU_INGRESOS", () => new FrmIngresos(_usuario));
            CreateFixedTab(_tabControlNegocio, "CLIENTE_TITLE", () => new FrmClientes(_usuario));
            CreateFixedTab(_tabControlNegocio, "MENU_MEMBRESIA", () => new FrmMembresias(_usuario));
            CreateFixedTab(_tabControlNegocio, "MENU_RESERVAS", () => new FrmReservas(_usuario));
            CreateFixedTab(_tabControlNegocio, "MENU_ESPACIOS", () => new FrmEspacios(_usuario));
            //CreateFixedTab(_tabControlNegocio, "MENU_RUTINAS", () => new FrmGestionRutinas(_usuario));
            CreateFixedTab(_tabControlNegocio, "MENU_PAGOS", () => new FrmPagos(_usuario));

            CreateFixedTab(_tabControlAdmin, "MENU_USERS", () => new FrmUsuarios(_usuario));
            CreateFixedTab(_tabControlAdmin, "MENU_PERMISOS", () => new FrmGestionFamilias(_usuario));
            CreateFixedTab(_tabControlAdmin, "MENU_BITACORA", () => new FrmBitacora(_usuario));
            CreateFixedTab(_tabControlAdmin, "MENU_BACKUPS", () => new FrmBackups(_usuario));
        }

        private void CreateFixedTab(TabControl parent, string transKey, Func<Form> formFactory)
        {
            var tab = new TabPage(transKey.Translate()) { Tag = transKey };
            var form = formFactory();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            tab.Controls.Add(form);
            form.Show();
            _allTabs.Add(transKey, tab);
        }

        private void ApplyPermissions()
        {
            _tabControlNegocio.TabPages.Clear();
            _tabControlAdmin.TabPages.Clear();
            _tabControlMain.TabPages.Clear();

            if (_usuario == null) return;

            CheckAndAddSubTab(_tabControlNegocio, "MENU_INGRESOS", PermisoKeys.IngresoListar);
            CheckAndAddSubTab(_tabControlNegocio, "CLIENTE_TITLE", PermisoKeys.ClienteListar);
            CheckAndAddSubTab(_tabControlNegocio, "MENU_MEMBRESIA", PermisoKeys.MembresiaListar);
            CheckAndAddSubTab(_tabControlNegocio, "MENU_RESERVAS", PermisoKeys.ReservaListar);
            CheckAndAddSubTab(_tabControlNegocio, "MENU_ESPACIOS", PermisoKeys.EspacioListar);
            //CheckAndAddSubTab(_tabControlNegocio, "MENU_RUTINAS", PermisoKeys.RutinaVer);
            CheckAndAddSubTab(_tabControlNegocio, "MENU_PAGOS", PermisoKeys.PagoListar);

            CheckAndAddSubTab(_tabControlAdmin, "MENU_USERS", PermisoKeys.UsuarioListar);
            CheckAndAddSubTab(_tabControlAdmin, "MENU_PERMISOS", PermisoKeys.PermisoAsignar);
            CheckAndAddSubTab(_tabControlAdmin, "MENU_BITACORA", PermisoKeys.BitacoraVer);
            CheckAndAddSubTab(_tabControlAdmin, "MENU_BACKUPS", PermisoKeys.BackupListar);

            if (_tabControlNegocio.TabPages.Count > 0) _tabControlMain.TabPages.Add(_tabPageNegocio);
            if (_tabControlAdmin.TabPages.Count > 0) _tabControlMain.TabPages.Add(_tabPageAdmin);
        }

        private void CheckAndAddSubTab(TabControl parent, string transKey, string permission)
        {
            if (_usuario.TienePermiso(permission) && _allTabs.ContainsKey(transKey))
            {
                parent.TabPages.Add(_allTabs[transKey]);
            }
        }

        private void UpdateLanguage()
        {
            this.Text = "MAIN_TITLE".Translate();

            if (_tabPageNegocio.Tag is string negocioKey)
                _tabPageNegocio.Text = negocioKey.Translate();
            if (_tabPageAdmin.Tag is string adminKey)
                _tabPageAdmin.Text = adminKey.Translate();

            foreach (var tab in _allTabs.Values)
            {
                if (tab.Tag is string transKey)
                    tab.Text = transKey.Translate();
                tab.Controls.OfType<ITranslatable>().FirstOrDefault()?.UpdateLanguage();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            new PermisosService().EnsurePermissions();
        }

        private void TabMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            var inner = _tabControlMain.SelectedTab == _tabPageNegocio ? _tabControlNegocio : _tabControlAdmin;
            RefreshActiveForm(inner);
        }

        private void TabInner_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshActiveForm((TabControl)sender);
        }

        private void RefreshActiveForm(TabControl tabControl)
        {
            var form = tabControl.SelectedTab?.Controls.OfType<IRefreshable>().FirstOrDefault();
            form?.RefreshData();
        }
    }
}
