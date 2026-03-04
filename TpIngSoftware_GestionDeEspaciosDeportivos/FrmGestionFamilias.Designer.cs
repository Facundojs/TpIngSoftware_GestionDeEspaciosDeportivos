namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    partial class FrmGestionFamilias
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.tvHierarchy = new System.Windows.Forms.TreeView();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.btnCrear = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.splitLists = new System.Windows.Forms.SplitContainer();
            this.lblPatentes = new System.Windows.Forms.Label();
            this.clbPatentes = new System.Windows.Forms.CheckedListBox();
            this.lblSubFamilias = new System.Windows.Forms.Label();
            this.clbSubFamilias = new System.Windows.Forms.CheckedListBox();
            this.pnlPreview = new System.Windows.Forms.Panel();
            this.lblPreview = new System.Windows.Forms.Label();
            this.tvPreview = new System.Windows.Forms.TreeView();

            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitLists)).BeginInit();
            this.splitLists.Panel1.SuspendLayout();
            this.splitLists.Panel2.SuspendLayout();
            this.splitLists.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.pnlPreview.SuspendLayout();
            this.SuspendLayout();

            // splitMain — main left/right divider
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 0);
            this.splitMain.Name = "splitMain";
            this.splitMain.Size = new System.Drawing.Size(960, 620);
            this.splitMain.SplitterDistance = 280;
            this.splitMain.TabIndex = 0;

            // splitMain.Panel1 — hierarchy tree
            this.splitMain.Panel1.Controls.Add(this.tvHierarchy);

            // splitMain.Panel2 — editor area
            this.splitMain.Panel2.Controls.Add(this.splitLists);
            this.splitMain.Panel2.Controls.Add(this.pnlPreview);
            this.splitMain.Panel2.Controls.Add(this.pnlTop);

            // tvHierarchy
            this.tvHierarchy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvHierarchy.Name = "tvHierarchy";
            this.tvHierarchy.TabIndex = 0;
            this.tvHierarchy.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvHierarchy_AfterSelect);

            // pnlTop — name field + action buttons
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Height = 45;
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.TabIndex = 1;
            this.pnlTop.Controls.Add(this.lblNombre);
            this.pnlTop.Controls.Add(this.txtNombre);
            this.pnlTop.Controls.Add(this.btnCrear);
            this.pnlTop.Controls.Add(this.btnGuardar);
            this.pnlTop.Controls.Add(this.btnEliminar);
            this.pnlTop.Controls.Add(this.btnLimpiar);

            // lblNombre
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(6, 14);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.TabIndex = 0;
            this.lblNombre.Text = "Nombre:";

            // txtNombre
            this.txtNombre.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.txtNombre.Location = new System.Drawing.Point(68, 11);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(160, 22);
            this.txtNombre.TabIndex = 1;

            // btnCrear
            this.btnCrear.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.btnCrear.Location = new System.Drawing.Point(236, 9);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.Size = new System.Drawing.Size(75, 26);
            this.btnCrear.TabIndex = 2;
            this.btnCrear.Text = "Crear";
            this.btnCrear.UseVisualStyleBackColor = true;
            this.btnCrear.Click += new System.EventHandler(this.btnCrear_Click);

            // btnGuardar
            this.btnGuardar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.btnGuardar.Enabled = false;
            this.btnGuardar.Location = new System.Drawing.Point(317, 9);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(75, 26);
            this.btnGuardar.TabIndex = 3;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);

            // btnEliminar
            this.btnEliminar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.btnEliminar.Enabled = false;
            this.btnEliminar.Location = new System.Drawing.Point(398, 9);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(75, 26);
            this.btnEliminar.TabIndex = 4;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);

            // btnLimpiar
            this.btnLimpiar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.btnLimpiar.Location = new System.Drawing.Point(479, 9);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(75, 26);
            this.btnLimpiar.TabIndex = 5;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);

            // splitLists — Patentes (left) | Sub-Familias (right)
            this.splitLists.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitLists.Name = "splitLists";
            this.splitLists.TabIndex = 2;

            // splitLists.Panel1 — Patentes
            this.splitLists.Panel1.Controls.Add(this.clbPatentes);
            this.splitLists.Panel1.Controls.Add(this.lblPatentes);

            // splitLists.Panel2 — Sub-Familias
            this.splitLists.Panel2.Controls.Add(this.clbSubFamilias);
            this.splitLists.Panel2.Controls.Add(this.lblSubFamilias);

            // lblPatentes
            this.lblPatentes.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPatentes.Height = 20;
            this.lblPatentes.AutoSize = false;
            this.lblPatentes.Name = "lblPatentes";
            this.lblPatentes.TabIndex = 0;
            this.lblPatentes.Text = "Patentes";
            this.lblPatentes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // clbPatentes
            this.clbPatentes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbPatentes.FormattingEnabled = true;
            this.clbPatentes.Name = "clbPatentes";
            this.clbPatentes.TabIndex = 1;
            this.clbPatentes.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbPatentes_ItemCheck);

            // lblSubFamilias
            this.lblSubFamilias.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSubFamilias.Height = 20;
            this.lblSubFamilias.AutoSize = false;
            this.lblSubFamilias.Name = "lblSubFamilias";
            this.lblSubFamilias.TabIndex = 0;
            this.lblSubFamilias.Text = "Sub-Familias";
            this.lblSubFamilias.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // clbSubFamilias
            this.clbSubFamilias.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbSubFamilias.FormattingEnabled = true;
            this.clbSubFamilias.Name = "clbSubFamilias";
            this.clbSubFamilias.TabIndex = 1;
            this.clbSubFamilias.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbSubFamilias_ItemCheck);

            // pnlPreview — read-only composite preview
            this.pnlPreview.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlPreview.Height = 200;
            this.pnlPreview.Name = "pnlPreview";
            this.pnlPreview.TabIndex = 3;
            this.pnlPreview.Controls.Add(this.tvPreview);
            this.pnlPreview.Controls.Add(this.lblPreview);

            // lblPreview
            this.lblPreview.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPreview.Height = 20;
            this.lblPreview.AutoSize = false;
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.TabIndex = 0;
            this.lblPreview.Text = "Vista Previa";
            this.lblPreview.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // tvPreview
            this.tvPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvPreview.LabelEdit = false;
            this.tvPreview.Name = "tvPreview";
            this.tvPreview.TabIndex = 1;

            // FrmGestionFamilias
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 620);
            this.Controls.Add(this.splitMain);
            this.Name = "FrmGestionFamilias";
            this.Text = "Gestión de Familias";
            this.Load += new System.EventHandler(this.FrmGestionFamilias_Load);

            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);

            this.splitLists.Panel1.ResumeLayout(false);
            this.splitLists.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitLists)).EndInit();
            this.splitLists.ResumeLayout(false);

            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlPreview.ResumeLayout(false);

            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.TreeView tvHierarchy;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Button btnCrear;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.SplitContainer splitLists;
        private System.Windows.Forms.Label lblPatentes;
        private System.Windows.Forms.CheckedListBox clbPatentes;
        private System.Windows.Forms.Label lblSubFamilias;
        private System.Windows.Forms.CheckedListBox clbSubFamilias;
        private System.Windows.Forms.Panel pnlPreview;
        private System.Windows.Forms.Label lblPreview;
        private System.Windows.Forms.TreeView tvPreview;
    }
}
