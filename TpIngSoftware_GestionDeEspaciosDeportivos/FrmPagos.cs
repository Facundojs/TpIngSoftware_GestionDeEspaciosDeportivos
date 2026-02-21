using BLL.DTOs;
using Domain.Composite;
using Domain.Enums;
using Service.DTO;
using Service.Helpers;
using Service.Facade.Extension;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TpIngSoftware_GestionDeEspaciosDeportivos.Business;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmPagos : Form
    {
        private readonly UsuarioDTO _usuario;
        private readonly PagoManager _pagoManager;
        private readonly ClienteManager _clienteManager;
        private Dictionary<Guid, string> _clientesCache;

        public FrmPagos(UsuarioDTO usuario)
        {
            InitializeComponent();
            _usuario = usuario;
            _pagoManager = new PagoManager();
            _clienteManager = new ClienteManager();
            _clientesCache = new Dictionary<Guid, string>();
        }

        private void FrmPagos_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            UpdateLanguage();
            ApplyPermissions();
            CargarClientesCache();
            CargarPagos();
        }

        private void ConfigurarGrid()
        {
            dgvPagos.AutoGenerateColumns = false;
            dgvPagos.Columns.Clear();
            dgvPagos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Codigo", HeaderText = "Código" });
            dgvPagos.Columns.Add(new DataGridViewTextBoxColumn { Name = "Cliente", HeaderText = "Cliente" });
            dgvPagos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Monto", HeaderText = "Monto", DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvPagos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Fecha", HeaderText = "Fecha" });
            dgvPagos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Metodo", HeaderText = "Método" });
            dgvPagos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Estado", HeaderText = "Estado" });
            dgvPagos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Detalle", HeaderText = "Detalle" });

            dgvPagos.CellFormatting += DgvPagos_CellFormatting;
        }

        private void DgvPagos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvPagos.Rows[e.RowIndex].DataBoundItem is PagoDTO pago)
            {
                if (dgvPagos.Columns[e.ColumnIndex].Name == "Cliente")
                {
                    if (_clientesCache.ContainsKey(pago.ClienteID))
                    {
                        e.Value = _clientesCache[pago.ClienteID];
                    }
                    else
                    {
                        var cliente = _clienteManager.ObtenerCliente(pago.ClienteID);
                        if (cliente != null)
                        {
                            string nombre = $"{cliente.Nombre} {cliente.Apellido}";
                            _clientesCache[pago.ClienteID] = nombre;
                            e.Value = nombre;
                        }
                        else
                        {
                            e.Value = "Desconocido";
                        }
                    }
                }
            }
        }

        private void UpdateLanguage()
        {
            this.Text = "PAGO_TITLE".Translate();

            grpRegistro.Text = "PAGO_TITLE".Translate(); // Reusing title or similar? Let's keep it simple or add specific key if needed.
            lblDNI.Text = "LBL_DNI_CLIENTE".Translate();
            lblMonto.Text = "LBL_MONTO".Translate();
            lblMetodo.Text = "LBL_METODO".Translate();
            lblDetalle.Text = "LBL_DETALLE".Translate();
            btnRegistrar.Text = "BTN_REGISTRAR_PAGO".Translate();

            grpFiltros.Text = "BTN_FILTER".Translate();
            lblDesde.Text = "LBL_DATE_FROM".Translate();
            lblHasta.Text = "LBL_DATE_TO".Translate();
            lblDNIFiltro.Text = "LBL_DNI_CLIENTE".Translate();
            btnFiltrar.Text = "BTN_FILTER".Translate();

            btnReembolsar.Text = "BTN_REEMBOLSAR".Translate();
            btnAdjuntarComprobante.Text = "BTN_ADJUNTAR_COMPROBANTE".Translate();

            if (dgvPagos.Columns.Count > 0)
            {
                dgvPagos.Columns[0].HeaderText = "LBL_CODIGO".Translate(); // Reusing Membresia Code label
                dgvPagos.Columns[1].HeaderText = "LBL_CLIENTE".Translate();
                dgvPagos.Columns[2].HeaderText = "LBL_MONTO".Translate();
                dgvPagos.Columns[3].HeaderText = "LBL_FECHA".Translate();
                dgvPagos.Columns[4].HeaderText = "LBL_METODO".Translate();
                dgvPagos.Columns[5].HeaderText = "LBL_ESTADO".Translate();
                dgvPagos.Columns[6].HeaderText = "LBL_DETALLE".Translate();
            }
        }

        private void ApplyPermissions()
        {
            if (_usuario == null) return;

            grpRegistro.Enabled = _usuario.TienePermiso(PermisoKeys.PagoRegistrar);
            btnRegistrar.Enabled = _usuario.TienePermiso(PermisoKeys.PagoRegistrar);

            // Refund and Attach depend on selection too
            btnReembolsar.Visible = _usuario.TienePermiso(PermisoKeys.PagoReembolsar);
            btnAdjuntarComprobante.Visible = _usuario.TienePermiso(PermisoKeys.PagoAdjuntarComprobante);
        }

        private void CargarClientesCache()
        {
            try
            {
                var clientes = _clienteManager.ListarClientes();
                _clientesCache = clientes.ToDictionary(c => c.Id, c => $"{c.Nombre} {c.Apellido}");
            }
            catch (Exception)
            {
                // Silent fail, will load on demand
            }
        }

        private void CargarPagos()
        {
            try
            {
                var pagos = _pagoManager.ListarPagos(null, null, null);
                dgvPagos.DataSource = pagos;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtDNICliente.Text) ||
                    string.IsNullOrWhiteSpace(txtMonto.Text) ||
                    cmbMetodo.SelectedIndex == -1)
                {
                    MessageBox.Show("ERR_REQUIRED_FIELD".Translate(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtDNICliente.Text, out int dni))
                {
                    MessageBox.Show("ERR_INVALID_NUMBER".Translate(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtMonto.Text, out decimal monto) || monto <= 0)
                {
                    MessageBox.Show("ERR_MONTO_INVALIDO".Translate(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var cliente = _clienteManager.ObtenerClientePorDNI(dni);
                if (cliente == null)
                {
                    MessageBox.Show("ERR_CLIENTE_NO_ENCONTRADO".Translate(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var dto = new PagoDTO
                {
                    ClienteID = cliente.Id,
                    Monto = monto,
                    Metodo = cmbMetodo.Text,
                    Detalle = txtDetalle.Text,
                    Fecha = DateTime.Now
                    // MembresiaID/ReservaID remain null as per current UI scope
                };

                _pagoManager.RegistrarPago(dto);

                MessageBox.Show("MSG_PAGO_REGISTRADO".Translate(), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LimpiarRegistro();
                CargarPagos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarRegistro()
        {
            txtDNICliente.Text = "";
            txtMonto.Text = "";
            cmbMetodo.SelectedIndex = -1;
            txtDetalle.Text = "";
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            try
            {
                Guid? clienteId = null;
                if (!string.IsNullOrWhiteSpace(txtDNIFiltro.Text))
                {
                    if (int.TryParse(txtDNIFiltro.Text, out int dni))
                    {
                        var cliente = _clienteManager.ObtenerClientePorDNI(dni);
                        if (cliente != null)
                        {
                            clienteId = cliente.Id;
                        }
                        else
                        {
                            // If client not found by DNI, and DNI was entered, maybe we should return empty list?
                            // Or show all? Usually empty list.
                            dgvPagos.DataSource = new List<PagoDTO>();
                            return;
                        }
                    }
                }

                DateTime? desde = dtpDesde.Value.Date;
                DateTime? hasta = dtpHasta.Value.Date.AddDays(1).AddSeconds(-1); // End of day

                var pagos = _pagoManager.ListarPagos(clienteId, desde, hasta);
                dgvPagos.DataSource = pagos;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReembolsar_Click(object sender, EventArgs e)
        {
            if (dgvPagos.SelectedRows.Count == 0) return;

            var pago = (PagoDTO)dgvPagos.SelectedRows[0].DataBoundItem;

            if (pago.Estado != EstadoPago.Abonado)
            {
                MessageBox.Show("ERR_SOLO_ABONADO".Translate(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("MSG_CONFIRM".Translate(), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    _pagoManager.ReembolsarPago(pago.Id);
                    MessageBox.Show("MSG_PAGO_REEMBOLSADO".Translate(), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarPagos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAdjuntarComprobante_Click(object sender, EventArgs e)
        {
            if (dgvPagos.SelectedRows.Count == 0) return;
            var pago = (PagoDTO)dgvPagos.SelectedRows[0].DataBoundItem;

            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Seleccionar Comprobante";
                dialog.Filter = "Archivos de Imagen y PDF|*.jpg;*.jpeg;*.png;*.pdf|Todos los archivos|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        byte[] fileBytes = File.ReadAllBytes(dialog.FileName);
                        var comprobante = new ComprobanteDTO
                        {
                            PagoID = pago.Id,
                            NombreArchivo = Path.GetFileName(dialog.FileName),
                            RutaArchivo = dialog.FileName, // Or relative path handling in backend
                            Contenido = fileBytes,
                            FechaSubida = DateTime.Now
                        };

                        _pagoManager.AdjuntarComprobante(pago.Id, comprobante);
                        MessageBox.Show("MSG_COMPROBANTE_ADJUNTADO".Translate(), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void dgvPagos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPagos.SelectedRows.Count > 0)
            {
                var pago = (PagoDTO)dgvPagos.SelectedRows[0].DataBoundItem;
                bool canRefund = _usuario.TienePermiso(PermisoKeys.PagoReembolsar);
                bool canAttach = _usuario.TienePermiso(PermisoKeys.PagoAdjuntarComprobante);

                btnReembolsar.Enabled = canRefund && pago.Estado == EstadoPago.Abonado;
                btnAdjuntarComprobante.Enabled = canAttach;
            }
            else
            {
                btnReembolsar.Enabled = false;
                btnAdjuntarComprobante.Enabled = false;
            }
        }
    }
}
