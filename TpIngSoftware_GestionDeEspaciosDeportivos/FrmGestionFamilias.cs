using Domain.Composite;
using Service.DTO;
using Service.Facade.Extension;
using Service.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Service.Helpers;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmGestionFamilias : Form
    {
        private UsuarioDTO _usuario;
        private PermisosService _permisosService;

        public FrmGestionFamilias(UsuarioDTO usuario)
        {
            InitializeComponent();
            _usuario = usuario;
            _permisosService = new PermisosService();
            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            this.Text = Domain.Enums.Translations.PERMISSIONS_TITLE.Translate();
            lblFamilias.Text = "Familias";
            lblPatentes.Text = "Patentes";
            lblNombreFamilia.Text = Domain.Enums.Translations.LBL_NOMBRE.Translate();
            btnCrear.Text = Domain.Enums.Translations.BTN_CREAR.Translate();
            btnActualizar.Text = Domain.Enums.Translations.BTN_ACTUALIZAR.Translate();
            btnEliminar.Text = Domain.Enums.Translations.BTN_ELIMINAR.Translate();
        }

        private void FrmGestionFamilias_Load(object sender, EventArgs e)
        {
            ApplyPermissions();
            LoadData();
        }

        private void ApplyPermissions()
        {
            if (_usuario == null) return;

            if (!_usuario.TienePermiso(PermisoKeys.PermisoAsignar))
            {
                MessageBox.Show(Domain.Enums.Translations.MSG_NO_PERM_LIST.Translate());
                this.Close();
            }
        }

        private void LoadData()
        {
            LoadPatentes();
            LoadFamilias();
        }

        private void LoadFamilias()
        {
            lstFamilias.Items.Clear();
            var familias = _permisosService.GetAllFamilias();
            foreach (var fam in familias)
            {
                lstFamilias.Items.Add(fam);
            }
            lstFamilias.DisplayMember = "Nombre";
        }

        private void LoadPatentes()
        {
            lstPatentes.Items.Clear();
            var patentes = _permisosService.GetAllPatentes();
            foreach (var pat in patentes)
            {
                lstPatentes.Items.Add(pat);
            }
            lstPatentes.DisplayMember = "Nombre";
        }

        private void lstFamilias_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstFamilias.SelectedItem is Familia fam)
            {
                txtNombreFamilia.Text = fam.Nombre;

                for (int i = 0; i < lstPatentes.Items.Count; i++)
                {
                    var pat = lstPatentes.Items[i] as Patente;
                    lstPatentes.SetItemChecked(i, fam.Accesos.Any(x => x.Id == pat.Id));
                }
            }
            else
            {
                txtNombreFamilia.Text = "";
                for (int i = 0; i < lstPatentes.Items.Count; i++)
                {
                    lstPatentes.SetItemChecked(i, false);
                }
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreFamilia.Text))
            {
                MessageBox.Show(Domain.Enums.Translations.ERR_REQUIRED_FIELD.Translate());
                return;
            }

            try
            {
                var nuevaFamilia = new Familia
                {
                    Id = Guid.NewGuid(),
                    Nombre = txtNombreFamilia.Text
                };

                foreach (var item in lstPatentes.CheckedItems)
                {
                    if (item is Patente pat)
                    {
                        nuevaFamilia.Agregar(pat);
                    }
                }

                _permisosService.CrearFamilia(nuevaFamilia);
                _permisosService.GuardarFamilia(nuevaFamilia);

                MessageBox.Show(Domain.Enums.Translations.MSG_SUCCESS.Translate());
                LoadFamilias();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Domain.Enums.Translations.MSG_ERR_GENERIC.Translate() + ex.Message);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (lstFamilias.SelectedItem is Familia fam)
            {
                if (fam.Nombre == "Administrador")
                {
                    MessageBox.Show("Cannot edit Administrador family.");
                    return;
                }

                try
                {
                    fam.Nombre = txtNombreFamilia.Text;

                    var newAccesos = new List<Acceso>();
                    foreach (var item in lstPatentes.CheckedItems)
                    {
                        if (item is Patente pat)
                        {
                            newAccesos.Add(pat);
                        }
                    }

                    fam.ClearAccesos();

                    foreach (var pat in newAccesos)
                    {
                        fam.Agregar(pat);
                    }

                    _permisosService.GuardarFamilia(fam);
                    MessageBox.Show(Domain.Enums.Translations.MSG_SUCCESS.Translate());
                    LoadFamilias();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Domain.Enums.Translations.MSG_ERR_GENERIC.Translate() + ex.Message);
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (lstFamilias.SelectedItem is Familia fam)
            {
                if (fam.Nombre == "Administrador")
                {
                    MessageBox.Show("Cannot delete Administrador family.");
                    return;
                }

                try
                {
                    _permisosService.EliminarFamilia(fam.Id);
                    MessageBox.Show(Domain.Enums.Translations.MSG_SUCCESS.Translate());
                    LoadFamilias();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Domain.Enums.Translations.MSG_ERR_GENERIC.Translate() + ex.Message);
                }
            }
        }
    }
}
