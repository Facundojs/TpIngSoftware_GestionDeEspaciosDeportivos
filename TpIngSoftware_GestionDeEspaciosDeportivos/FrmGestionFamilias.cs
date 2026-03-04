using Domain.Composite;
using Domain.Enums;
using Service.DTO;
using Service.Facade.Extension;
using Service.Helpers;
using Service.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmGestionFamilias : Form, IRefreshable, ITranslatable
    {
        private readonly UsuarioDTO _usuario;
        private readonly PermisosService _permisosService;

        private List<Familia> _allFamilias = new List<Familia>();
        private Familia _selectedFamilia;

        public FrmGestionFamilias(UsuarioDTO usuario)
        {
            InitializeComponent();
            _usuario = usuario;
            _permisosService = new PermisosService();
            UpdateLanguage();
        }

        public void UpdateLanguage()
        {
            this.Text = Translations.PERMISSIONS_TITLE.Translate();
            lblNombre.Text = Translations.LBL_NOMBRE.Translate();
            btnCrear.Text = Translations.BTN_CREAR.Translate();
            btnGuardar.Text = Translations.BTN_SAVE.Translate();
            btnEliminar.Text = Translations.BTN_ELIMINAR.Translate();
            btnLimpiar.Text = Translations.BTN_LIMPIAR.Translate();
            lblPatentes.Text = Translations.LBL_PATENTES.Translate();
            lblSubFamilias.Text = Translations.LBL_SUBFAMILIAS.Translate();
            lblPreview.Text = Translations.LBL_VISTA_PREVIA.Translate();
        }

        private void FrmGestionFamilias_Load(object sender, EventArgs e)
        {
            ApplyPermissions();
            LoadData();
        }

        public void RefreshData() => LoadData();

        private void ApplyPermissions()
        {
            if (_usuario == null) return;
            if (!_usuario.TienePermiso(PermisoKeys.PermisoAsignar))
            {
                MessageBox.Show(Translations.MSG_NO_PERM_LIST.Translate(), Translations.TITLE_WARNING.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }

        // ── Data loading ─────────────────────────────────────────────────────────

        private void LoadData()
        {
            LoadPatentes();
            RefreshAll();
        }

        // Fetches fresh Familia tree and rebuilds the hierarchy TreeView.
        private void RefreshAll()
        {
            _allFamilias = _permisosService.GetAllFamilias();
            RefreshHierarchy();
            LoadSubFamilias(null, new HashSet<Guid>());
        }

        private void LoadPatentes()
        {
            clbPatentes.Items.Clear();
            foreach (var pat in _permisosService.GetAllPatentes())
                clbPatentes.Items.Add(pat);
            clbPatentes.DisplayMember = "Nombre";
        }

        // ── Hierarchy TreeView ────────────────────────────────────────────────────

        private void RefreshHierarchy()
        {
            tvHierarchy.BeginUpdate();
            tvHierarchy.Nodes.Clear();
            foreach (var fam in GetRootFamilias())
                tvHierarchy.Nodes.Add(BuildFamiliaNode(fam, new HashSet<Guid>()));
            tvHierarchy.ExpandAll();
            tvHierarchy.EndUpdate();
        }

        // Recursively builds a TreeNode for a Familia and its children.
        // visited guards against cycles in persisted data (safety net; backend prevents them).
        private TreeNode BuildFamiliaNode(Familia fam, HashSet<Guid> visited)
        {
            var node = new TreeNode("[F] " + fam.Nombre) { Tag = fam };
            if (!visited.Add(fam.Id)) return node;

            foreach (var acceso in fam.Accesos)
            {
                if (acceso is Familia subFam)
                    node.Nodes.Add(BuildFamiliaNode(subFam, new HashSet<Guid>(visited)));
                else if (acceso is Patente pat)
                    node.Nodes.Add(new TreeNode("[P] " + pat.Nombre) { Tag = pat });
            }
            return node;
        }

        // A root Familia is one whose Id does not appear as a child in any other Familia's subtree.
        private List<Familia> GetRootFamilias()
        {
            var childIds = new HashSet<Guid>();
            foreach (var f in _allFamilias)
                CollectChildIds(f.Accesos, childIds);
            return _allFamilias.Where(f => !childIds.Contains(f.Id)).ToList();
        }

        private void CollectChildIds(IList<Acceso> accesos, HashSet<Guid> childIds)
        {
            foreach (var acceso in accesos)
            {
                if (acceso is Familia f)
                {
                    childIds.Add(f.Id);
                    CollectChildIds(f.Accesos, childIds);
                }
            }
        }

        // ── Sub-Familias list ─────────────────────────────────────────────────────

        // excludeId: the Familia being edited (self-reference not allowed).
        // ancestorIds: Familias that have the edited one in their subtree (cycle prevention).
        private void LoadSubFamilias(Guid? excludeId, HashSet<Guid> ancestorIds)
        {
            clbSubFamilias.Items.Clear();
            foreach (var fam in _allFamilias)
            {
                if (excludeId.HasValue && fam.Id == excludeId.Value) continue;
                if (ancestorIds.Contains(fam.Id)) continue;
                clbSubFamilias.Items.Add(fam);
            }
            clbSubFamilias.DisplayMember = "Nombre";
        }

        // Returns IDs of Familias whose descendant tree contains familiaId —
        // adding any of them as children would create a cycle.
        private HashSet<Guid> GetAncestorIds(Guid familiaId)
        {
            var ancestors = new HashSet<Guid>();
            foreach (var f in _allFamilias)
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

        // ── Preview TreeView ──────────────────────────────────────────────────────

        private void RefreshPreview()
        {
            tvPreview.BeginUpdate();
            tvPreview.Nodes.Clear();

            string nombre = string.IsNullOrWhiteSpace(txtNombre.Text) ? "..." : txtNombre.Text;
            var root = new TreeNode("[F] " + nombre);

            foreach (var item in clbPatentes.CheckedItems)
            {
                if (item is Patente pat)
                    root.Nodes.Add(new TreeNode("[P] " + pat.Nombre));
            }
            foreach (var item in clbSubFamilias.CheckedItems)
            {
                if (item is Familia sub)
                    root.Nodes.Add(BuildFamiliaNode(sub, new HashSet<Guid>()));
            }

            tvPreview.Nodes.Add(root);
            tvPreview.ExpandAll();
            tvPreview.EndUpdate();
        }

        // ItemCheck fires before the check state updates; BeginInvoke defers the refresh.
        private void clbPatentes_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!IsDisposed) this.BeginInvoke(new Action(RefreshPreview));
        }

        private void clbSubFamilias_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!IsDisposed) this.BeginInvoke(new Action(RefreshPreview));
        }

        // ── Editor state ──────────────────────────────────────────────────────────

        private void SetEditorState(bool hasSelection)
        {
            btnGuardar.Enabled = hasSelection;
            btnEliminar.Enabled = hasSelection;
            btnCrear.Enabled = !hasSelection;
        }

        private void Limpiar()
        {
            _selectedFamilia = null;
            txtNombre.Text = "";
            for (int i = 0; i < clbPatentes.Items.Count; i++)
                clbPatentes.SetItemChecked(i, false);
            LoadSubFamilias(null, new HashSet<Guid>());
            tvPreview.Nodes.Clear();
            tvHierarchy.SelectedNode = null;
            SetEditorState(false);
        }

        // ── Events ────────────────────────────────────────────────────────────────

        private void tvHierarchy_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!(e.Node?.Tag is Familia fam)) return;

            _selectedFamilia = fam;
            txtNombre.Text = fam.Nombre;

            var ancestorIds = GetAncestorIds(fam.Id);
            LoadSubFamilias(fam.Id, ancestorIds);

            for (int i = 0; i < clbPatentes.Items.Count; i++)
            {
                var pat = clbPatentes.Items[i] as Patente;
                clbPatentes.SetItemChecked(i, fam.Accesos.Any(x => x.Id == pat.Id));
            }
            for (int i = 0; i < clbSubFamilias.Items.Count; i++)
            {
                var sub = clbSubFamilias.Items[i] as Familia;
                clbSubFamilias.SetItemChecked(i, fam.Accesos.Any(x => x.Id == sub.Id));
            }

            RefreshPreview();
            SetEditorState(true);
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show(Translations.ERR_REQUIRED_FIELD.Translate(), Translations.TITLE_WARNING.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var nuevaFamilia = new Familia { Id = Guid.NewGuid(), Nombre = txtNombre.Text };
                foreach (var item in clbPatentes.CheckedItems)
                    if (item is Patente pat) nuevaFamilia.Agregar(pat);
                foreach (var item in clbSubFamilias.CheckedItems)
                    if (item is Familia sub) nuevaFamilia.Agregar(sub);

                _permisosService.CrearFamilia(nuevaFamilia);
                _permisosService.GuardarFamilia(nuevaFamilia);

                MessageBox.Show(Translations.MSG_SUCCESS.Translate(), Translations.TITLE_INFO.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshAll();
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + ex.Message, Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (_selectedFamilia == null) return;
            if (_selectedFamilia.Nombre == "Administrador")
            {
                MessageBox.Show(Translations.ERR_CANNOT_EDIT_ADMIN.Translate(), Translations.TITLE_WARNING.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _selectedFamilia.Nombre = txtNombre.Text;

                var newAccesos = new List<Acceso>();
                foreach (var item in clbPatentes.CheckedItems)
                    if (item is Patente pat) newAccesos.Add(pat);
                foreach (var item in clbSubFamilias.CheckedItems)
                    if (item is Familia sub) newAccesos.Add(sub);

                _selectedFamilia.ClearAccesos();
                foreach (var acceso in newAccesos)
                    _selectedFamilia.Agregar(acceso);

                _permisosService.GuardarFamilia(_selectedFamilia);

                MessageBox.Show(Translations.MSG_SUCCESS.Translate(), Translations.TITLE_INFO.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshAll();
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + ex.Message, Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_selectedFamilia == null) return;
            if (_selectedFamilia.Nombre == "Administrador")
            {
                MessageBox.Show(Translations.ERR_CANNOT_DELETE_ADMIN.Translate(), Translations.TITLE_WARNING.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                Translations.MSG_CONFIRM.Translate(),
                Translations.TITLE_CONFIRM_DELETE.Translate(),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            try
            {
                _permisosService.EliminarFamilia(_selectedFamilia.Id);
                MessageBox.Show(Translations.MSG_SUCCESS.Translate(), Translations.TITLE_INFO.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshAll();
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Translations.MSG_ERR_GENERIC.Translate() + ex.Message, Translations.TITLE_ERROR.Translate(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
