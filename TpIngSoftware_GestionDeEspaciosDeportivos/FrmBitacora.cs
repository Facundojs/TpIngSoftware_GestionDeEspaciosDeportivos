using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Service.DTO;
using Service.Logic;
using Service.Facade.Extension;
using Service.Domain.Composite;
using Service.Helpers;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmBitacora : Form
    {
        private readonly UsuarioDTO _usuario;
        private readonly BitacoraService _bitacoraService;
        private int _page = 1;
        private const int PageSize = 20;

        public FrmBitacora(UsuarioDTO usuario)
        {
            InitializeComponent();
            _usuario = usuario;
            _bitacoraService = new BitacoraService();
            UpdateLanguage();
            SetupControls();
        }

        // Default constructor for designer
        public FrmBitacora()
        {
            InitializeComponent();
        }

        private void SetupControls()
        {
            cmbLevel.Items.Add("Todos");
            cmbLevel.Items.Add("INFO");
            cmbLevel.Items.Add("ERROR");
            cmbLevel.SelectedIndex = 0;
            dtpFrom.Value = DateTime.Today.AddDays(-7);
            dtpTo.Value = DateTime.Today;
        }

        private void UpdateLanguage()
        {
            this.Text = "BITACORA_TITLE".Translate();
            lblFrom.Text = "LBL_DATE_FROM".Translate();
            lblTo.Text = "LBL_DATE_TO".Translate();
            lblLevel.Text = "LBL_LOG_LEVEL".Translate();
            lblMessage.Text = "LBL_MESSAGE".Translate();
            btnFilter.Text = "BTN_FILTER".Translate();
        }

        private void FrmBitacora_Load(object sender, EventArgs e)
        {
            if (_usuario != null && !_usuario.TienePermiso(PermisoKeys.BitacoraVer))
            {
                MessageBox.Show("MSG_NO_PERM_LOGS".Translate());
                this.Close();
                return;
            }
            LoadLogs();
        }

        private void LoadLogs()
        {
            try
            {
                string logLevel = cmbLevel.SelectedItem?.ToString();
                if (logLevel == "Todos") logLevel = null;

                // Adjust To Date to include the end of the day
                DateTime toDate = dtpTo.Value.Date.AddDays(1).AddTicks(-1);

                var logs = _bitacoraService.GetLogs(_page, PageSize, dtpFrom.Value.Date, toDate, logLevel, txtMessage.Text);
                dgvLogs.DataSource = logs;
                dgvLogs.AutoResizeColumns();

                lblPage.Text = "LBL_PAGE".Translate() + " " + _page;
                btnPrev.Enabled = _page > 1;
                btnNext.Enabled = logs.Count == PageSize;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            _page = 1;
            LoadLogs();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (_page > 1)
            {
                _page--;
                LoadLogs();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            _page++;
            LoadLogs();
        }
    }
}
