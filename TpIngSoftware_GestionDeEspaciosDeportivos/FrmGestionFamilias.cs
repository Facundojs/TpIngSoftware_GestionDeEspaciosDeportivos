using Domain.Composite;
using Service.DTO;
using Service.Facade.Extension;
using Service.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Service.Helpers;
using Domain.Enums;

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
            this.Text = Translations.PERMISSIONS_TITLE.Translate();
            lblFamilias.Text = "Familias";
            lblPatentes.Text = "Patentes";
            lblSubFamilias.Text = Translations.LBL_SUBFAMILIAS.Translate();
            lblNombreFamilia.Text = Translations.LBL_NOMBRE.Translate();
            btnCrear.Text = Translations.BTN_CREAR.Translate();
            btnActualizar.Text = Translations.BTN_ACTUALIZAR.Translate();
            btnEliminar.Text = Translations.BTN_ELIMINAR.Translate();
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
                MessageBox.Show(Translations.MSG_NO_PERM_LIST.Translate());
                this.Close();
            }
        }

        private void LoadData()
        {
            LoadPatentes();
            LoadFamilias();
            LoadSubFamilias(null);
        }

        private void LoadFamilias()
        {
            lstFamilias.Items.Clear();
            var familias = _permisosService.GetAllFamilias();
            foreach (var fam in familias)
                lstFamilias.Items.Add(fam);
            lstFamilias.DisplayMember = "Nombre";
        }

        private void LoadPatentes()
        {
            lstPatentes.Items.Clear();
            var patentes = _permisosService.GetAllPatentes();
            foreach (var pat in patentes)
                lstPatentes.Items.Add(pat);
            lstPatentes.DisplayMember = "Nombre";
        }

        private void LoadSubFamilias(Guid? excludeId)
        {
            lstSubFamilias.Items.Clear();
            var familias = _permisosService.GetAllFamilias();
            foreach (var fam in familias)
            {
                if (excludeId.HasValue && fam.Id == excludeId.Value) continue;
                lstSubFamilias.Items.Add(fam);
            }
            lstSubFamilias.DisplayMember = "Nombre";
        }

        private void lstFamilias_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstFamilias.SelectedItem is Familia fam)
            {
                txtNombreFamilia.Text = fam.Nombre;
                LoadSubFamilias(fam.Id);

                for (int i = 0; i < lstPatentes.Items.Count; i++)
                {
                    var pat = lstPatentes.Items[i] as Patente;
                    lstPatentes.SetItemChecked(i, fam.Accesos.Any(x => x.Id == pat.Id));
                }

                for (int i = 0; i < lstSubFamilias.Items.Count; i++)
                {
                    var sub = lstSubFamilias.Items[i] as Familia;
                    lstSubFamilias.SetItemChecked(i, fam.Accesos.Any(x => x.Id == sub.Id));
                }
            }
            else
            {
                txtNombreFamilia.Text = "";
                LoadSubFamilias(null);
                for (int i = 0; i < lstPatentes.Items.Count; i++)
                    lstPatentes.SetItemChecked(i, false);
                for (int i = 0; i < lstSubFamilias.Items.Count; i++)
                    lstSubFamilias.SetItemChecked(i, false);
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreFamilia.Text))
            {
                MessageBox.Show(Translations.ERR_REQUIRED_FIELD.Translate());
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
                        nuevaFamilia.Agregar(pat);
                }

                foreach (var item in lstSubFamilias.CheckedItems)
                {
                    if (item is Familia sub)
                        nuevaFamilia.Agregar(sub);
                }

                _permisosService.CrearFamilia(nuevaFamilia);
                _permisosService.GuardarFamilia(nuevaFamilia);

                MessageBox.Show(Translations.MSG_SUCCESS.Translate());
                LoadFamilias();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + ex.Message);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (lstFamilias.SelectedItem is Familia fam)
            {
                if (fam.Nombre == "Administrador")
                {
                    MessageBox.Show(Translations.ERR_CANNOT_EDIT_ADMIN.Translate());
                    return;
                }

                try
                {
                    fam.Nombre = txtNombreFamilia.Text;

                    var newAccesos = new List<Acceso>();
                    foreach (var item in lstPatentes.CheckedItems)
                    {
                        if (item is Patente pat)
                            newAccesos.Add(pat);
                    }
                    foreach (var item in lstSubFamilias.CheckedItems)
                    {
                        if (item is Familia sub)
                            newAccesos.Add(sub);
                    }

                    fam.ClearAccesos();
                    foreach (var acceso in newAccesos)
                        fam.Agregar(acceso);

                    _permisosService.GuardarFamilia(fam);
                    MessageBox.Show(Translations.MSG_SUCCESS.Translate());
                    LoadFamilias();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + ex.Message);
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (lstFamilias.SelectedItem is Familia fam)
            {
                if (fam.Nombre == "Administrador")
                {
                    MessageBox.Show(Translations.ERR_CANNOT_DELETE_ADMIN.Translate());
                    return;
                }

                try
                {
                    _permisosService.EliminarFamilia(fam.Id);
                    MessageBox.Show(Translations.MSG_SUCCESS.Translate());
                    LoadFamilias();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + ex.Message);
                }
            }
        }
    }
}
