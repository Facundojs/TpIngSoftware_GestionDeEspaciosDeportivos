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
        private Familia _familiaSeleccionada;

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
            lblPreview.Text = "Vista Previa";
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
            LoadSubFamilias(null, new HashSet<Guid>());
            RefreshHierarchy();
        }

        // Rebuilds tvFamilias showing only root Familias and their full subtrees.
        private void RefreshHierarchy()
        {
            tvFamilias.BeginUpdate();
            tvFamilias.Nodes.Clear();
            var todas = _permisosService.GetAllFamilias();
            foreach (var fam in GetRootFamilias(todas))
            {
                var node = new TreeNode(fam.Nombre) { Tag = fam };
                AddNodosRecursivos(node, fam.Accesos);
                tvFamilias.Nodes.Add(node);
            }
            tvFamilias.ExpandAll();
            tvFamilias.EndUpdate();
        }

        private void AddNodosRecursivos(TreeNode parent, IList<Acceso> accesos)
        {
            foreach (var acceso in accesos)
            {
                var node = new TreeNode(acceso.Nombre) { Tag = acceso };
                if (acceso is Familia sub)
                    AddNodosRecursivos(node, sub.Accesos);
                parent.Nodes.Add(node);
            }
        }

        // Returns Familias that do not appear as a child inside any other Familia's subtree.
        private List<Familia> GetRootFamilias(List<Familia> todas)
        {
            var childIds = new HashSet<Guid>();
            foreach (var f in todas)
                CollectChildFamiliaIds(f.Accesos, childIds);
            return todas.Where(f => !childIds.Contains(f.Id)).ToList();
        }

        private void CollectChildFamiliaIds(IList<Acceso> accesos, HashSet<Guid> childIds)
        {
            foreach (var acceso in accesos)
            {
                if (acceso is Familia f)
                {
                    childIds.Add(f.Id);
                    CollectChildFamiliaIds(f.Accesos, childIds);
                }
            }
        }

        // Returns IDs of Familias that have familiaId somewhere in their descendant tree,
        // i.e. adding them as children of familiaId would create a cycle.
        private HashSet<Guid> GetAncestorIds(Guid familiaId, List<Familia> todas)
        {
            var ancestors = new HashSet<Guid>();
            foreach (var f in todas)
            {
                if (f.Id != familiaId && ContainsDescendant(f, familiaId, new HashSet<Guid>()))
                    ancestors.Add(f.Id);
            }
            return ancestors;
        }

        private bool ContainsDescendant(Familia familia, Guid targetId, HashSet<Guid> visited)
        {
            if (!visited.Add(familia.Id)) return false;
            foreach (var acceso in familia.Accesos)
            {
                if (acceso.Id == targetId) return true;
                if (acceso is Familia sub && ContainsDescendant(sub, targetId, visited)) return true;
            }
            return false;
        }

        private void LoadPatentes()
        {
            lstPatentes.Items.Clear();
            foreach (var pat in _permisosService.GetAllPatentes())
                lstPatentes.Items.Add(pat);
            lstPatentes.DisplayMember = "Nombre";
        }

        private void LoadSubFamilias(Guid? excludeId, HashSet<Guid> ancestorIds, List<Familia> allFamilias = null)
        {
            lstSubFamilias.Items.Clear();
            var familias = allFamilias ?? _permisosService.GetAllFamilias();
            foreach (var fam in familias)
            {
                if (excludeId.HasValue && fam.Id == excludeId.Value) continue;
                if (ancestorIds.Contains(fam.Id)) continue;
                lstSubFamilias.Items.Add(fam);
            }
            lstSubFamilias.DisplayMember = "Nombre";
        }

        private void RefreshPreview()
        {
            tvPreview.BeginUpdate();
            tvPreview.Nodes.Clear();
            string nombre = string.IsNullOrWhiteSpace(txtNombreFamilia.Text) ? "..." : txtNombreFamilia.Text;
            var root = new TreeNode(nombre);
            foreach (var item in lstPatentes.CheckedItems)
            {
                if (item is Patente pat)
                    root.Nodes.Add(new TreeNode(pat.Nombre));
            }
            foreach (var item in lstSubFamilias.CheckedItems)
            {
                if (item is Familia sub)
                {
                    var subNode = new TreeNode(sub.Nombre) { Tag = sub };
                    AddNodosRecursivos(subNode, sub.Accesos);
                    root.Nodes.Add(subNode);
                }
            }
            tvPreview.Nodes.Add(root);
            tvPreview.ExpandAll();
            tvPreview.EndUpdate();
        }

        private void ClearEditor()
        {
            _familiaSeleccionada = null;
            txtNombreFamilia.Text = "";
            for (int i = 0; i < lstPatentes.Items.Count; i++)
                lstPatentes.SetItemChecked(i, false);
            LoadSubFamilias(null, new HashSet<Guid>());
            tvPreview.Nodes.Clear();
            tvFamilias.SelectedNode = null;
        }

        private void tvFamilias_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!(e.Node?.Tag is Familia fam)) return;

            _familiaSeleccionada = fam;
            txtNombreFamilia.Text = fam.Nombre;

            var todas = _permisosService.GetAllFamilias();
            var ancestorIds = GetAncestorIds(fam.Id, todas);
            LoadSubFamilias(fam.Id, ancestorIds, todas);

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

            RefreshPreview();
        }

        // ItemCheck fires before the state updates; BeginInvoke defers RefreshPreview until after.
        private void lstPatentes_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!IsDisposed) this.BeginInvoke(new Action(RefreshPreview));
        }

        private void lstSubFamilias_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!IsDisposed) this.BeginInvoke(new Action(RefreshPreview));
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
                var nuevaFamilia = new Familia { Id = Guid.NewGuid(), Nombre = txtNombreFamilia.Text };
                foreach (var item in lstPatentes.CheckedItems)
                {
                    if (item is Patente pat) nuevaFamilia.Agregar(pat);
                }
                foreach (var item in lstSubFamilias.CheckedItems)
                {
                    if (item is Familia sub) nuevaFamilia.Agregar(sub);
                }

                _permisosService.CrearFamilia(nuevaFamilia);
                _permisosService.GuardarFamilia(nuevaFamilia);

                MessageBox.Show(Translations.MSG_SUCCESS.Translate());
                RefreshHierarchy();
                ClearEditor();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + ex.Message);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (_familiaSeleccionada == null) return;
            if (_familiaSeleccionada.Nombre == "Administrador")
            {
                MessageBox.Show(Translations.ERR_CANNOT_EDIT_ADMIN.Translate());
                return;
            }

            try
            {
                _familiaSeleccionada.Nombre = txtNombreFamilia.Text;

                var newAccesos = new List<Acceso>();
                foreach (var item in lstPatentes.CheckedItems)
                {
                    if (item is Patente pat) newAccesos.Add(pat);
                }
                foreach (var item in lstSubFamilias.CheckedItems)
                {
                    if (item is Familia sub) newAccesos.Add(sub);
                }

                _familiaSeleccionada.ClearAccesos();
                foreach (var acceso in newAccesos)
                    _familiaSeleccionada.Agregar(acceso);

                _permisosService.GuardarFamilia(_familiaSeleccionada);
                MessageBox.Show(Translations.MSG_SUCCESS.Translate());
                RefreshHierarchy();
                ClearEditor();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_familiaSeleccionada == null) return;
            if (_familiaSeleccionada.Nombre == "Administrador")
            {
                MessageBox.Show(Translations.ERR_CANNOT_DELETE_ADMIN.Translate());
                return;
            }

            try
            {
                _permisosService.EliminarFamilia(_familiaSeleccionada.Id);
                MessageBox.Show(Translations.MSG_SUCCESS.Translate());
                RefreshHierarchy();
                ClearEditor();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + ex.Message);
            }
        }
    }
}
