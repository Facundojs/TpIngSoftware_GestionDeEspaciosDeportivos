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
using Service.Helpers;
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
            this.AcceptButton = btnLogin;
        }

        private void SetupLanguageSelector()
        {
            cmbLanguage = new ComboBox();
            cmbLanguage.Width = 150;
            cmbLanguage.Location = new Point(this.ClientSize.Width - cmbLanguage.Width - 10, 10);

            LanguageSelectorHelper.SetupComboBox(cmbLanguage, _languageService, UpdateLanguage);

            this.Controls.Add(cmbLanguage);
        }

        private void UpdateLanguage()
        {
            this.Text = Domain.Enums.Translations.LOGIN_TITLE.Translate();
            this.lblUsername.Text = Domain.Enums.Translations.LBL_USERNAME.Translate();
            this.lblPassword.Text = Domain.Enums.Translations.LBL_PASSWORD.Translate();
            this.btnLogin.Text = Domain.Enums.Translations.BTN_LOGIN.Translate();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text;
                string password = txtPassword.Text;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show(Domain.Enums.Translations.MSG_ENTER_CREDENTIALS.Translate(), Domain.Enums.Translations.TITLE_WARNING.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                UsuarioDTO usuario = _usuarioService.Login(username, password);

                if (usuario != null)
                {
                    SessionContext.CurrentUser = usuario;
                    MessageBox.Show(string.Format(Domain.Enums.Translations.MSG_WELCOME.Translate() + ", {0}!", usuario.Username), Domain.Enums.Translations.TITLE_SUCCESS.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Hide();
                    Form1 mainForm = new Form1(usuario);
                    mainForm.FormClosed += (s, args) => this.Close();
                    mainForm.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Domain.Enums.Translations.MSG_ERR_LOGIN.Translate() + ex.Message, Domain.Enums.Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
