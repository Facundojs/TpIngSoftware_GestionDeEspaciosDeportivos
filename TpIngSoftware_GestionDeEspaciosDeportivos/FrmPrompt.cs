using System;
using System.Windows.Forms;
using Domain.Enums;
using Service.Facade.Extension;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmPrompt : Form
    {
        public string InputText { get; private set; }

        public FrmPrompt(string promptText, string titleText)
        {
            InitializeComponent();

            this.Text = titleText;
            lblPrompt.Text = promptText;

            btnOk.Text = "BTN_OK".Translate();
            btnCancel.Text = "BTN_CANCEL".Translate();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            InputText = txtInput.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
