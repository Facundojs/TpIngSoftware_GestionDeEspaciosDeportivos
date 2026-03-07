using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using BLL.DTOs;
using Domain;
using Service.DTO;
using Service.Helpers;
using Service.Facade.Extension;
using TpIngSoftware_GestionDeEspaciosDeportivos.Business;
using Domain.Enums;
using Domain;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmRutina : Form
    {
        private readonly Guid _clienteId;
        private readonly UsuarioDTO _usuario;
        private readonly RutinaManager _rutinaManager;
        private readonly ClienteManager _clienteManager;
        private readonly Guid? _rutinaId;
        private RutinaDTO _rutinaActual;
        private BindingList<EjercicioDTO> _ejerciciosBinding;

        public FrmRutina(Guid clienteId, UsuarioDTO usuario, Guid? rutinaId = null)
        {
            InitializeComponent();
            _clienteId = clienteId;
            _usuario = usuario;
            _rutinaId = rutinaId;
            _rutinaManager = new RutinaManager();
            _clienteManager = new ClienteManager();
            _ejerciciosBinding = new BindingList<EjercicioDTO>();
        }

        private void FrmRutina_Load(object sender, EventArgs e)
        {
            ConfigurarUI();
            CargarDatosCliente();
            CargarRutina();
            ApplyPermissions();
        }

        private void ConfigurarUI()
        {
            this.Text = "RUTINA_TITLE".Translate();
            lblCliente.Text = "LBL_CLIENTE".Translate();
            lblDesde.Text = "LBL_RUTINA_DESDE".Translate();
            lblHasta.Text = "LBL_RUTINA_HASTA".Translate();

            lblNombreEjercicio.Text = "LBL_EJERCICIO_NOMBRE".Translate();
            lblDiaSemana.Text = "LBL_EJERCICIO_DIA".Translate();
            lblRepeticiones.Text = "LBL_EJERCICIO_REP".Translate();
            lblOrden.Text = "LBL_EJERCICIO_ORDEN".Translate();

            btnAgregarEjercicio.Text = "BTN_AGREGAR_EJERCICIO".Translate();
            btnEliminarEjercicio.Text = "BTN_ELIMINAR_EJERCICIO".Translate();
            btnGuardar.Text = "BTN_GUARDAR_RUTINA".Translate();
            btnBorrarRutina.Text = "BTN_BORRAR_RUTINA".Translate();

            cmbDiaSemana.Items.Clear();
            for (int i = 1; i <= 7; i++)
            {
                string translationKey = $"DAY_{i % 7}";
                cmbDiaSemana.Items.Add(new KeyValuePair<int, string>(i, translationKey.Translate()));
            }

            cmbDiaSemana.DisplayMember = "Value";
            cmbDiaSemana.ValueMember = "Key";
            cmbDiaSemana.SelectedIndex = 0;

            dgvEjercicios.AutoGenerateColumns = false;
            dgvEjercicios.Columns.Clear();
            dgvEjercicios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "LBL_EJERCICIO_NOMBRE".Translate(), Width = 150 });
            dgvEjercicios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Repeticiones", HeaderText = "LBL_EJERCICIO_REP".Translate() });
            dgvEjercicios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Orden", HeaderText = "LBL_EJERCICIO_ORDEN".Translate() });
            dgvEjercicios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DiaSemana", HeaderText = "LBL_EJERCICIO_DIA".Translate() });

            dgvEjercicios.DataSource = _ejerciciosBinding;
            dgvEjercicios.CellFormatting += DgvEjercicios_CellFormatting;
        }

        private void DgvEjercicios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvEjercicios.Columns[e.ColumnIndex].DataPropertyName == "DiaSemana" && e.Value != null)
            {
                int day = (int)e.Value;
                string translationKey = $"DAY_{day % 7}";
                e.Value = translationKey.Translate();
                e.FormattingApplied = true;
            }
        }

        private void CargarDatosCliente()
        {
            try
            {
                var cliente = _clienteManager.ObtenerCliente(_clienteId);
                if (cliente != null)
                {
                    lblCliente.Text = $"{cliente.Nombre} {cliente.Apellido}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("MSG_ERR_GENERIC".Translate() + " " + ex.Message, "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarRutina()
        {
            try
            {
                if (_rutinaId.HasValue)
                {
                    _rutinaActual = _rutinaManager.ObtenerRutina(_rutinaId.Value);
                }
                else
                {
                    _rutinaActual = _rutinaManager.ObtenerRutinaActiva(_clienteId);
                }

                _ejerciciosBinding.Clear();

                if (_rutinaActual != null)
                {
                    lblDesde.Text = $"{"LBL_RUTINA_DESDE".Translate()} {_rutinaActual.Desde.ToShortDateString()}";
                    lblHasta.Text = _rutinaActual.Hasta.HasValue ? $"{"LBL_RUTINA_HASTA".Translate()} {_rutinaActual.Hasta.Value.ToShortDateString()}" : "";

                    if (_rutinaActual.Ejercicios != null)
                    {
                        foreach (var ex in _rutinaActual.Ejercicios.OrderBy(x => x.DiaSemana).ThenBy(x => x.Orden))
                        {
                            _ejerciciosBinding.Add(ex);
                        }
                    }

                    btnGuardar.Text = "BTN_GUARDAR_RUTINA".Translate();
                    btnBorrarRutina.Enabled = _usuario.TienePermiso(PermisoKeys.RutinaEliminar);
                }
                else
                {
                    lblDesde.Text = "LBL_RUTINA_VACIA".Translate();
                    lblHasta.Text = "";
                    btnBorrarRutina.Enabled = false;
                    _rutinaActual = new RutinaDTO { ClienteID = _clienteId };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("MSG_ERR_GENERIC".Translate() + " " + ex.Message, "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyPermissions()
        {
            bool canCreate = _usuario.TienePermiso(PermisoKeys.RutinaCrear);
            bool canModify = _usuario.TienePermiso(PermisoKeys.RutinaModificar);

            pnlEdicion.Enabled = canCreate || canModify;
            btnGuardar.Enabled = canCreate || canModify;
        }

        private void btnAgregarEjercicio_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNombreEjercicio.Text))
                {
                    MessageBox.Show("ERR_REQUIRED_FIELD".Translate(), "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtRepeticiones.Text, out int rep) || rep <= 0)
                {
                    MessageBox.Show("ERR_EJERCICIO_REP_ZERO".Translate(), "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtOrden.Text, out int orden))
                {
                    orden = _ejerciciosBinding.Count + 1;
                }

                var selectedDay = (KeyValuePair<int, string>)cmbDiaSemana.SelectedItem;

                var ejercicio = new EjercicioDTO
                {
                    Id = Guid.Empty,
                    Nombre = txtNombreEjercicio.Text.Trim(),
                    Repeticiones = rep,
                    DiaSemana = selectedDay.Key,
                    Orden = orden
                };

                _ejerciciosBinding.Add(ejercicio);

                txtNombreEjercicio.Text = "";
                txtRepeticiones.Text = "";
                txtOrden.Text = "";
                txtNombreEjercicio.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminarEjercicio_Click(object sender, EventArgs e)
        {
            if (dgvEjercicios.CurrentRow?.DataBoundItem is EjercicioDTO selected)
            {
                _ejerciciosBinding.Remove(selected);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_ejerciciosBinding.Count == 0)
                {
                    MessageBox.Show("ERR_RUTINA_SIN_EJERCICIOS".Translate(), "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _rutinaActual.Ejercicios = _ejerciciosBinding.ToList();

                bool isNew = _rutinaActual.Id == Guid.Empty || _rutinaManager.ObtenerRutinaActiva(_clienteId) == null;

                if (isNew)
                {
                    if (!_usuario.TienePermiso(PermisoKeys.RutinaCrear))
                    {
                        MessageBox.Show("MSG_NO_PERM_USERS".Translate(), "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error); // Generic no perm
                        return;
                    }
                    _rutinaManager.CrearRutina(_rutinaActual);
                }
                else
                {
                    if (!_usuario.TienePermiso(PermisoKeys.RutinaModificar))
                    {
                         MessageBox.Show("MSG_NO_PERM_USERS".Translate(), "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                         return;
                    }
                    _rutinaManager.ModificarRutina(_rutinaActual.Id, _rutinaActual.Ejercicios);
                }

                MessageBox.Show("MSG_RUTINA_GUARDADA".Translate(), "TITLE_INFO".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarRutina();
            }
            catch (Exception ex)
            {
                MessageBox.Show("MSG_ERR_GENERIC".Translate() + " " + ex.Message, "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBorrarRutina_Click(object sender, EventArgs e)
        {
            if (_rutinaActual == null || _rutinaActual.Id == Guid.Empty) return;

            if (MessageBox.Show("MSG_CONFIRM_BORRAR_RUTINA".Translate(), "TITLE_CONFIRM".Translate(), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    _rutinaManager.BorrarRutina(_rutinaActual.Id);
                    MessageBox.Show("MSG_RUTINA_ELIMINADA".Translate(), "TITLE_INFO".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarRutina();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("MSG_ERR_GENERIC".Translate() + " " + ex.Message, "TITLE_ERROR".Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvEjercicios_SelectionChanged(object sender, EventArgs e)
        {
             if (dgvEjercicios.CurrentRow?.DataBoundItem is EjercicioDTO selected)
             {
                 txtNombreEjercicio.Text = selected.Nombre;
                 txtRepeticiones.Text = selected.Repeticiones.ToString();
                 txtOrden.Text = selected.Orden.ToString();

                 foreach (var item in cmbDiaSemana.Items)
                 {
                     if (((KeyValuePair<int, string>)item).Key == selected.DiaSemana)
                     {
                         cmbDiaSemana.SelectedItem = item;
                         break;
                     }
                 }
             }
        }
    }
}
