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

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmLogin : Form
    {
        private readonly UsuarioService _usuarioService;

        public FrmLogin()
        {
            InitializeComponent();
            _usuarioService = new UsuarioService();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text;
                string password = txtPassword.Text;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Por favor, ingrese usuario y contraseña.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                UsuarioDTO usuario = _usuarioService.Login(username, password);

                if (usuario != null)
                {
                    MessageBox.Show($"Bienvenido, {usuario.Username}!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Hide login and show main form
                    this.Hide();
                    Form1 mainForm = new Form1();
                    mainForm.FormClosed += (s, args) => this.Close();
                    mainForm.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar sesión: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
