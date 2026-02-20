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
        private ToolStripMenuItem _menuAdmin;
        private ToolStripMenuItem _menuBackups;
        private ToolStripMenuItem _menuUsuarios;
        private ToolStripMenuItem _menuBitacora;
        private ToolStripMenuItem _menuMembresias;
        private ToolStripMenuItem _menuClientes;
        private ToolStripMenuItem _menuRutinas;
        private ToolStripMenuItem _menuEspacios;
        private ToolStripComboBox _cmbLanguage;
        private LanguageService _languageService;

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

            _menuStrip.Items.Add(_menuAdmin);

            // Language Selector
            _languageService = new LanguageService();
            _cmbLanguage = new ToolStripComboBox();
            _cmbLanguage.Alignment = ToolStripItemAlignment.Right;
            _cmbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbLanguage.ComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            _cmbLanguage.ComboBox.DrawItem += CmbLanguage_DrawItem;
            _cmbLanguage.SelectedIndexChanged += CmbLanguage_SelectedIndexChanged;

            // Load Data
            var languages = _languageService.GetLanguages();
            _cmbLanguage.ComboBox.DataSource = new BindingSource(languages, null);
            _cmbLanguage.ComboBox.DisplayMember = "Value";
            _cmbLanguage.ComboBox.ValueMember = "Key";

            // Set Selection
            string currentCulture = Thread.CurrentThread.CurrentUICulture.Name;
            if (languages.ContainsKey(currentCulture))
                _cmbLanguage.ComboBox.SelectedValue = currentCulture;
            else if (_cmbLanguage.Items.Count > 0)
                _cmbLanguage.SelectedIndex = 0;

            _menuStrip.Items.Add(_cmbLanguage);

            this.Controls.Add(_menuStrip);
            this.MainMenuStrip = _menuStrip;
        }

        private void CmbLanguage_DrawItem(object sender, DrawItemEventArgs e)
        {
             if (e.Index < 0) return;
             e.DrawBackground();

             var item = (KeyValuePair<string, string>)((ComboBox)sender).Items[e.Index];
             string code = item.Key;
             string name = item.Value;

             Image flag = FlagHelper.DrawFlag(code, 25, 15);
             e.Graphics.DrawImage(flag, e.Bounds.Left + 2, e.Bounds.Top + 2);

             using (Brush textBrush = new SolidBrush(e.ForeColor))
             {
                 e.Graphics.DrawString(name, e.Font, textBrush, e.Bounds.Left + 30, e.Bounds.Top + 2);
             }

             e.DrawFocusRectangle();
        }

        private void CmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
             if (_cmbLanguage.ComboBox.SelectedValue is string code)
             {
                 Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(code);
                 _languageService.SaveUserLanguage(code);
                 UpdateLanguage();
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
