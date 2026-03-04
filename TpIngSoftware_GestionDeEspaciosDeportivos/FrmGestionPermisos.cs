using Domain.Composite;
using Domain.Enums;
using Service.DTO;
using Service.Facade.Extension;
using Service.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class FrmGestionPermisos : Form
    {
        private UsuarioDTO _usuario;
        private UsuarioService _usuarioService;
        private PermisosService _permisosService;

        public FrmGestionPermisos(UsuarioDTO usuario)
        {
            InitializeComponent();
            _usuario = usuario;
            _usuarioService = new UsuarioService();
            _permisosService = new PermisosService();
            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            this.Text = Translations.PERMISSIONS_TITLE.Translate();
            btnSave.Text = Translations.BTN_SAVE.Translate();
            btnCancel.Text = Translations.BTN_CANCEL.Translate();
        }

        private void FrmGestionPermisos_Load(object sender, EventArgs e)
        {
            LoadTreeView();
        }

        private void LoadTreeView()
        {
            treeViewPermisos.Nodes.Clear();
            var familias = _permisosService.GetAllFamilias();
            var userFamilyIds = _usuario.Permisos.Select(p => p.Id).ToList();

            foreach (var familia in familias)
            {
                var node = new TreeNode(familia.Nombre);
                node.Tag = familia;
                node.Checked = userFamilyIds.Contains(familia.Id);
                AddChildNodes(node, familia.Accesos);
                treeViewPermisos.Nodes.Add(node);
            }

            treeViewPermisos.ExpandAll();
        }

        private void AddChildNodes(TreeNode parentNode, System.Collections.Generic.IList<Acceso> accesos)
        {
            foreach (var acceso in accesos)
            {
                var childNode = new TreeNode(acceso.Nombre);
                childNode.Tag = acceso;

                if (acceso is Familia subFamilia)
                    AddChildNodes(childNode, subFamilia.Accesos);

                parentNode.Nodes.Add(childNode);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
             var nuevosPermisos = new List<Acceso>();

             foreach(TreeNode node in treeViewPermisos.Nodes)
             {
                 if(node.Checked && node.Tag is Familia f)
                 {
                     nuevosPermisos.Add(f);
                 }
             }

             try
             {
                 _usuario.Permisos = nuevosPermisos;
                 _usuarioService.Update(_usuario);
                 MessageBox.Show(Translations.MSG_PERMISSIONS_UPDATED.Translate());
                 this.Close();
             }
             catch(Exception ex)
             {
                 MessageBox.Show(Translations.MSG_ERR_SAVE.Translate() + ex.Message);
             }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
