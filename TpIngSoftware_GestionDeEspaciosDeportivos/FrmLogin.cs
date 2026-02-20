using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Service.Logic;
using Service.DTO;
using Service.Facade.Extension;
using TpIngSoftware_GestionDeEspaciosDeportivos.Helpers;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmLogin : Form
    {
        private readonly UsuarioService _usuarioService;
        private readonly LanguageService _languageService;
        private ComboBox cmbLanguage;

        public FrmLogin()
        {
            InitializeComponent();
            _usuarioService = new UsuarioService();
            _languageService = new LanguageService();
            SetupLanguageSelector();
            UpdateLanguage();
        }

        private void SetupLanguageSelector()
        {
            cmbLanguage = new ComboBox();
            cmbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLanguage.DrawMode = DrawMode.OwnerDrawFixed;
            cmbLanguage.ItemHeight = 20;
            cmbLanguage.Width = 150;
            cmbLanguage.Location = new Point(this.ClientSize.Width - cmbLanguage.Width - 10, 10);
            cmbLanguage.DrawItem += CmbLanguage_DrawItem;
            cmbLanguage.SelectedIndexChanged += CmbLanguage_SelectedIndexChanged;

            // Load languages
            var languages = _languageService.GetLanguages();
            cmbLanguage.DataSource = new BindingSource(languages, null);
            cmbLanguage.DisplayMember = "Value";
            cmbLanguage.ValueMember = "Key";

            // Set current selection
            string currentCulture = Thread.CurrentThread.CurrentUICulture.Name;
            // Handle cases where current culture might be more specific or not in list
            if (languages.ContainsKey(currentCulture))
            {
                 cmbLanguage.SelectedValue = currentCulture;
            }
            else
            {
                 // Default to first if not found
                 if (cmbLanguage.Items.Count > 0) cmbLanguage.SelectedIndex = 0;
            }

            this.Controls.Add(cmbLanguage);
        }

        private void CmbLanguage_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            e.DrawBackground();

            var item = (KeyValuePair<string, string>)cmbLanguage.Items[e.Index];
            string code = item.Key;
            string name = item.Value;

            // Draw Flag
            Image flag = FlagHelper.DrawFlag(code, 25, 15);
            e.Graphics.DrawImage(flag, e.Bounds.Left + 2, e.Bounds.Top + 2);

            // Draw Text
            using (Brush textBrush = new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(name, e.Font, textBrush, e.Bounds.Left + 30, e.Bounds.Top + 2);
            }

            e.DrawFocusRectangle();
        }

        private void CmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLanguage.SelectedValue is string code)
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(code);
                _languageService.SaveUserLanguage(code);
                UpdateLanguage();
            }
        }

        private void UpdateLanguage()
        {
            this.Text = "LOGIN_TITLE".Translate();
            this.lblUsername.Text = "LBL_USERNAME".Translate();
            this.lblPassword.Text = "LBL_PASSWORD".Translate();
            this.btnLogin.Text = "BTN_LOGIN".Translate();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text;
                string password = txtPassword.Text;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("MSG_ENTER_CREDENTIALS".Translate(), "TITLE_WARNING".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                UsuarioDTO usuario = _usuarioService.Login(username, password);

                if (usuario != null)
                {
                    MessageBox.Show(string.Format("MSG_WELCOME".Translate() + ", {0}!", usuario.Username), "TITLE_SUCCESS".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Hide();
                    Form1 mainForm = new Form1(usuario);
                    mainForm.FormClosed += (s, args) => this.Close();
                    mainForm.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("MSG_ERR_LOGIN".Translate() + ex.Message, "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
