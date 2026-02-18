namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    partial class FrmEjercicios
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlEditor = new System.Windows.Forms.Panel();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.dgvEjercicios = new System.Windows.Forms.DataGridView();
            this.pnlEditor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEjercicios)).BeginInit();
            this.SuspendLayout();
            //
            // pnlEditor
            //
            this.pnlEditor.Controls.Add(this.btnLimpiar);
            this.pnlEditor.Controls.Add(this.btnEliminar);
            this.pnlEditor.Controls.Add(this.btnActualizar);
            this.pnlEditor.Controls.Add(this.btnAgregar);
            this.pnlEditor.Controls.Add(this.txtNombre);
            this.pnlEditor.Controls.Add(this.lblNombre);
            this.pnlEditor.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlEditor.Location = new System.Drawing.Point(400, 0);
            this.pnlEditor.Name = "pnlEditor";
            this.pnlEditor.Size = new System.Drawing.Size(250, 450);
            this.pnlEditor.TabIndex = 0;
            //
            // btnLimpiar
            //
            this.btnLimpiar.Location = new System.Drawing.Point(20, 200);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(210, 23);
            this.btnLimpiar.TabIndex = 5;
            this.btnLimpiar.Text = "BTN_LIMPIAR";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            //
            // btnEliminar
            //
            this.btnEliminar.Location = new System.Drawing.Point(20, 160);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(210, 23);
            this.btnEliminar.TabIndex = 4;
            this.btnEliminar.Text = "BTN_ELIMINAR";
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            //
            // btnActualizar
            //
            this.btnActualizar.Location = new System.Drawing.Point(130, 120);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(100, 23);
            this.btnActualizar.TabIndex = 3;
            this.btnActualizar.Text = "BTN_ACTUALIZAR";
            this.btnActualizar.UseVisualStyleBackColor = true;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            //
            // btnAgregar
            //
            this.btnAgregar.Location = new System.Drawing.Point(20, 120);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(100, 23);
            this.btnAgregar.TabIndex = 2;
            this.btnAgregar.Text = "BTN_AGREGAR";
            this.btnAgregar.UseVisualStyleBackColor = true;
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            //
            // txtNombre
            //
            this.txtNombre.Location = new System.Drawing.Point(20, 50);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(210, 20);
            this.txtNombre.TabIndex = 1;
            //
            // lblNombre
            //
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(20, 30);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(139, 13);
            this.lblNombre.TabIndex = 0;
            this.lblNombre.Text = "LBL_NOMBRE_EJERCICIO";
            //
            // dgvEjercicios
            //
            this.dgvEjercicios.AllowUserToAddRows = false;
            this.dgvEjercicios.AllowUserToDeleteRows = false;
            this.dgvEjercicios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEjercicios.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEjercicios.Location = new System.Drawing.Point(0, 0);
            this.dgvEjercicios.MultiSelect = false;
            this.dgvEjercicios.Name = "dgvEjercicios";
            this.dgvEjercicios.ReadOnly = true;
            this.dgvEjercicios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEjercicios.Size = new System.Drawing.Size(400, 450);
            this.dgvEjercicios.TabIndex = 1;
            this.dgvEjercicios.SelectionChanged += new System.EventHandler(this.dgvEjercicios_SelectionChanged);
            //
            // FrmEjercicios
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 450);
            this.Controls.Add(this.dgvEjercicios);
            this.Controls.Add(this.pnlEditor);
            this.Name = "FrmEjercicios";
            this.Text = "FRM_EJERCICIOS_TITLE";
            this.Load += new System.EventHandler(this.FrmEjercicios_Load);
            this.pnlEditor.ResumeLayout(false);
            this.pnlEditor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEjercicios)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlEditor;
        private System.Windows.Forms.DataGridView dgvEjercicios;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnLimpiar;
    }
}
