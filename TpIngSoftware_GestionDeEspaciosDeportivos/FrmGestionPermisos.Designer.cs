namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    partial class FrmGestionPermisos
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TreeView treeViewPermisos;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.treeViewPermisos = new System.Windows.Forms.TreeView();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // treeViewPermisos
            this.treeViewPermisos.CheckBoxes = true;
            this.treeViewPermisos.Location = new System.Drawing.Point(12, 12);
            this.treeViewPermisos.Name = "treeViewPermisos";
            this.treeViewPermisos.Size = new System.Drawing.Size(360, 280);

            // btnSave
            this.btnSave.Location = new System.Drawing.Point(12, 310);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 30);
            this.btnSave.Text = "Guardar";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(252, 310);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 30);
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // Form
            this.ClientSize = new System.Drawing.Size(384, 361);
            this.Controls.Add(this.treeViewPermisos);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Name = "FrmGestionPermisos";
            this.Text = "Gestión de Permisos";
            this.Load += new System.EventHandler(this.FrmGestionPermisos_Load);
            this.ResumeLayout(false);
        }
    }
}
