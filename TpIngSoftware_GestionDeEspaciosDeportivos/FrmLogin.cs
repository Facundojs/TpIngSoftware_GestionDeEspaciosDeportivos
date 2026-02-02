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

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmLogin : Form
    {
        private readonly UsuarioService _usuarioService;

        public FrmLogin()
        {
            InitializeComponent();
            _usuarioService = new UsuarioService();
            UpdateLanguage();
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
