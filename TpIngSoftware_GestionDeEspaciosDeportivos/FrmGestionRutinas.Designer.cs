namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    partial class FrmGestionRutinas
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
            this.pnlTop = new System.Windows.Forms.Panel();
            this.chkVerHistorial = new System.Windows.Forms.CheckBox();
            this.btnGestionarEjercicios = new System.Windows.Forms.Button();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnModificar = new System.Windows.Forms.Button();
            this.btnNueva = new System.Windows.Forms.Button();
            this.dgvRutinas = new System.Windows.Forms.DataGridView();
            this.pnlTop.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRutinas)).BeginInit();
            this.SuspendLayout();
            //
            // pnlTop
            //
            this.pnlTop.Controls.Add(this.chkVerHistorial);
            this.pnlTop.Controls.Add(this.btnGestionarEjercicios);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(800, 60);
            this.pnlTop.TabIndex = 0;
            //
            // chkVerHistorial
            //
            this.chkVerHistorial.AutoSize = true;
            this.chkVerHistorial.Location = new System.Drawing.Point(200, 25);
            this.chkVerHistorial.Name = "chkVerHistorial";
            this.chkVerHistorial.Size = new System.Drawing.Size(135, 17);
            this.chkVerHistorial.TabIndex = 1;
            this.chkVerHistorial.Text = "CHK_VER_HISTORIAL";
            this.chkVerHistorial.UseVisualStyleBackColor = true;
            this.chkVerHistorial.CheckedChanged += new System.EventHandler(this.chkVerHistorial_CheckedChanged);
            //
            // btnGestionarEjercicios
            //
            this.btnGestionarEjercicios.Location = new System.Drawing.Point(20, 20);
            this.btnGestionarEjercicios.Name = "btnGestionarEjercicios";
            this.btnGestionarEjercicios.Size = new System.Drawing.Size(160, 25);
            this.btnGestionarEjercicios.TabIndex = 0;
            this.btnGestionarEjercicios.Text = "BTN_GESTIONAR_EJERCICIOS";
            this.btnGestionarEjercicios.UseVisualStyleBackColor = true;
            this.btnGestionarEjercicios.Click += new System.EventHandler(this.btnGestionarEjercicios_Click);
            //
            // pnlBottom
            //
            this.pnlBottom.Controls.Add(this.btnEliminar);
            this.pnlBottom.Controls.Add(this.btnModificar);
            this.pnlBottom.Controls.Add(this.btnNueva);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 390);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(800, 60);
            this.pnlBottom.TabIndex = 1;
            //
            // btnEliminar
            //
            this.btnEliminar.Location = new System.Drawing.Point(300, 15);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(120, 30);
            this.btnEliminar.TabIndex = 2;
            this.btnEliminar.Text = "BTN_ELIMINAR";
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            //
            // btnModificar
            //
            this.btnModificar.Location = new System.Drawing.Point(160, 15);
            this.btnModificar.Name = "btnModificar";
            this.btnModificar.Size = new System.Drawing.Size(120, 30);
            this.btnModificar.TabIndex = 1;
            this.btnModificar.Text = "BTN_MODIFICAR";
            this.btnModificar.UseVisualStyleBackColor = true;
            this.btnModificar.Click += new System.EventHandler(this.btnModificar_Click);
            //
            // btnNueva
            //
            this.btnNueva.Location = new System.Drawing.Point(20, 15);
            this.btnNueva.Name = "btnNueva";
            this.btnNueva.Size = new System.Drawing.Size(120, 30);
            this.btnNueva.TabIndex = 0;
            this.btnNueva.Text = "BTN_NUEVA_RUTINA";
            this.btnNueva.UseVisualStyleBackColor = true;
            this.btnNueva.Click += new System.EventHandler(this.btnNueva_Click);
            //
            // dgvRutinas
            //
            this.dgvRutinas.AllowUserToAddRows = false;
            this.dgvRutinas.AllowUserToDeleteRows = false;
            this.dgvRutinas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRutinas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRutinas.Location = new System.Drawing.Point(0, 60);
            this.dgvRutinas.MultiSelect = false;
            this.dgvRutinas.Name = "dgvRutinas";
            this.dgvRutinas.ReadOnly = true;
            this.dgvRutinas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRutinas.Size = new System.Drawing.Size(800, 330);
            this.dgvRutinas.TabIndex = 2;
            this.dgvRutinas.SelectionChanged += new System.EventHandler(this.dgvRutinas_SelectionChanged);
            //
            // FrmGestionRutinas
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dgvRutinas);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.Name = "FrmGestionRutinas";
            this.Text = "FRM_GESTION_RUTINAS_TITLE";
            this.Load += new System.EventHandler(this.FrmGestionRutinas_Load);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRutinas)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.DataGridView dgvRutinas;
        private System.Windows.Forms.Button btnGestionarEjercicios;
        private System.Windows.Forms.CheckBox chkVerHistorial;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnModificar;
        private System.Windows.Forms.Button btnNueva;
    }
}
