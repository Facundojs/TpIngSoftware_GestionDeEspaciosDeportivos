using System;
using System.Drawing;
using System.Windows.Forms;
using Service.DTO;
using TpIngSoftware_GestionDeEspaciosDeportivos.Business;
using Service.Facade.Extension;
using Domain.Enums;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmIngresos : Form
    {
        private readonly UsuarioDTO _usuario;
        private readonly IngresoManager _manager;
        private readonly BLL.DTOs.ClienteDTO _clienteFiltro;

        public FrmIngresos(UsuarioDTO usuario, BLL.DTOs.ClienteDTO cliente = null)
        {
            InitializeComponent();
            _usuario = usuario;
            _manager = new IngresoManager();
            _clienteFiltro = cliente;
            ApplyLanguage();
        }

        private void FrmIngresos_Load(object sender, EventArgs e)
        {
            ApplyPermissions();
            CargarIngresos();
        }

        private void ApplyLanguage()
        {
            if (_clienteFiltro != null)
            {
                this.Text = $"{Translations.FRM_INGRESOS_TITLE.Translate()} - {_clienteFiltro.Nombre} {_clienteFiltro.Apellido}";
            }
            else
            {
                this.Text = Translations.FRM_INGRESOS_TITLE.Translate();
            }
            btnActualizar.Text = Translations.BTN_ACTUALIZAR.Translate();

            if (dgvIngresos.Columns.Count > 0)
            {
                dgvIngresos.Columns["ClienteNombre"].HeaderText = Translations.LBL_CLIENTE_NOMBRE.Translate();
                dgvIngresos.Columns["FechaHora"].HeaderText = Translations.LBL_FECHA.Translate();
            }
        }

        private void ApplyPermissions()
        {
            // Just viewing
        }

        private void CargarIngresos()
        {
            try
            {
                var ingresos = _clienteFiltro != null
                    ? _manager.ListarIngresosPorCliente(_clienteFiltro.Id)
                    : _manager.ListarIngresos();

                dgvIngresos.DataSource = null;
                dgvIngresos.DataSource = ingresos;
                ConfigurarColumnas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + " " + ex.Message, Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarColumnas()
        {
            if (dgvIngresos.Columns.Count > 0)
            {
                dgvIngresos.Columns["Id"].Visible = false;
                dgvIngresos.Columns["ClienteID"].Visible = false;
                dgvIngresos.Columns["ClienteNombre"].HeaderText = Translations.LBL_CLIENTE_NOMBRE.Translate();
                dgvIngresos.Columns["ClienteNombre"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvIngresos.Columns["FechaHora"].HeaderText = Translations.LBL_FECHA.Translate();
                dgvIngresos.Columns["FechaHora"].DefaultCellStyle.Format = "g";
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarIngresos();
        }
    }
}
