using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Service.Logic;
using Service.DTO;
using Service.Facade.Extension;
using System.Threading;
using System.Globalization;
using Service.Factory;
using TpIngSoftware_GestionDeEspaciosDeportivos.Helpers;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmLogin : Form
    {
        private readonly UsuarioService _usuarioService;
        private ComboBox cmbLanguage;

        public FrmLogin()
        {
            InitializeComponent();
            InitializeLanguageSelector();
            _usuarioService = new UsuarioService();
            UpdateLanguage();
        }

        private void InitializeLanguageSelector()
        {
            cmbLanguage = new ComboBox();
            cmbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLanguage.DrawMode = DrawMode.OwnerDrawFixed;
            cmbLanguage.ItemHeight = 20;
            cmbLanguage.Size = new Size(140, 26);
            // Top Right
            cmbLanguage.Location = new Point(this.ClientSize.Width - 150, 10);

            // Events
            cmbLanguage.DrawItem += CmbLanguage_DrawItem;
            cmbLanguage.SelectedIndexChanged += CmbLanguage_SelectedIndexChanged;

            // Load Languages
            var languages = FactoryDao.LanguageRepository.GetLanguages();
            foreach (var lang in languages)
            {
                // We use KeyValuePair for easier access
                cmbLanguage.Items.Add(new KeyValuePair<string, string>(lang.Key, lang.Value));
            }

            this.Controls.Add(cmbLanguage);

            // Set current selection
            string currentCode = FactoryDao.LanguageRepository.LoadUserLanguage();
            // Try to set thread culture if not set
            try {
                if (!string.IsNullOrEmpty(currentCode))
                {
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(currentCode);
                }
            } catch {}

            // Select item
            foreach (object itemObj in cmbLanguage.Items)
            {
                var item = (KeyValuePair<string, string>)itemObj;
                if (item.Key == currentCode)
                {
                    cmbLanguage.SelectedItem = itemObj;
                    break;
                }
            }
        }

        private void CmbLanguage_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            ComboBox combo = sender as ComboBox;
            KeyValuePair<string, string> item = (KeyValuePair<string, string>)combo.Items[e.Index];

            e.DrawBackground();

            // Draw Flag
            using (Bitmap flag = FlagHelper.GetFlag(item.Key))
            {
                if (flag != null)
                {
                    e.Graphics.DrawImage(flag, e.Bounds.Left + 2, e.Bounds.Top + 2, 24, 16);
                }
            }

            // Draw Text
            using (Brush textBrush = new SolidBrush(e.ForeColor))
            {
                // Adjust text position
                e.Graphics.DrawString(item.Value, combo.Font, textBrush, e.Bounds.Left + 30, e.Bounds.Top + 2);
            }

            e.DrawFocusRectangle();
        }

        private void CmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLanguage.SelectedItem == null) return;

            KeyValuePair<string, string> selected = (KeyValuePair<string, string>)cmbLanguage.SelectedItem;
            string code = selected.Key;

            try
            {
                // Set Culture
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(code);
                // Save Preference
                FactoryDao.LanguageRepository.SaveUserLanguage(code);
                // Update UI
                UpdateLanguage();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error changing language: " + ex.Message);
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
