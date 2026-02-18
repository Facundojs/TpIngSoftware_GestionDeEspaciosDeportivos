namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    partial class FrmRutina
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblCliente = new System.Windows.Forms.Label();
            this.lblDesde = new System.Windows.Forms.Label();
            this.lblHasta = new System.Windows.Forms.Label();
            this.pnlEdicion = new System.Windows.Forms.Panel();
            this.btnBorrarRutina = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnEliminarEjercicio = new System.Windows.Forms.Button();
            this.btnAgregarEjercicio = new System.Windows.Forms.Button();
            this.txtOrden = new System.Windows.Forms.TextBox();
            this.lblOrden = new System.Windows.Forms.Label();
            this.txtRepeticiones = new System.Windows.Forms.TextBox();
            this.lblRepeticiones = new System.Windows.Forms.Label();
            this.cmbDiaSemana = new System.Windows.Forms.ComboBox();
            this.lblDiaSemana = new System.Windows.Forms.Label();
            this.txtNombreEjercicio = new System.Windows.Forms.TextBox();
            this.lblNombreEjercicio = new System.Windows.Forms.Label();
            this.dgvEjercicios = new System.Windows.Forms.DataGridView();
            this.pnlHeader.SuspendLayout();
            this.pnlEdicion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEjercicios)).BeginInit();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.Controls.Add(this.lblHasta);
            this.pnlHeader.Controls.Add(this.lblDesde);
            this.pnlHeader.Controls.Add(this.lblCliente);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(800, 60);
            this.pnlHeader.TabIndex = 0;
            //
            // lblCliente
            //
            this.lblCliente.AutoSize = true;
            this.lblCliente.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCliente.Location = new System.Drawing.Point(12, 9);
            this.lblCliente.Name = "lblCliente";
            this.lblCliente.Size = new System.Drawing.Size(110, 20);
            this.lblCliente.TabIndex = 0;
            this.lblCliente.Text = "LBL_CLIENTE";
            //
            // lblDesde
            //
            this.lblDesde.AutoSize = true;
            this.lblDesde.Location = new System.Drawing.Point(13, 35);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.Size = new System.Drawing.Size(121, 13);
            this.lblDesde.TabIndex = 1;
            this.lblDesde.Text = "LBL_RUTINA_DESDE";
            //
            // lblHasta
            //
            this.lblHasta.AutoSize = true;
            this.lblHasta.Location = new System.Drawing.Point(300, 35);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Size = new System.Drawing.Size(120, 13);
            this.lblHasta.TabIndex = 2;
            this.lblHasta.Text = "LBL_RUTINA_HASTA";
            //
            // pnlEdicion
            //
            this.pnlEdicion.Controls.Add(this.btnBorrarRutina);
            this.pnlEdicion.Controls.Add(this.btnGuardar);
            this.pnlEdicion.Controls.Add(this.btnEliminarEjercicio);
            this.pnlEdicion.Controls.Add(this.btnAgregarEjercicio);
            this.pnlEdicion.Controls.Add(this.txtOrden);
            this.pnlEdicion.Controls.Add(this.lblOrden);
            this.pnlEdicion.Controls.Add(this.txtRepeticiones);
            this.pnlEdicion.Controls.Add(this.lblRepeticiones);
            this.pnlEdicion.Controls.Add(this.cmbDiaSemana);
            this.pnlEdicion.Controls.Add(this.lblDiaSemana);
            this.pnlEdicion.Controls.Add(this.txtNombreEjercicio);
            this.pnlEdicion.Controls.Add(this.lblNombreEjercicio);
            this.pnlEdicion.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlEdicion.Location = new System.Drawing.Point(550, 60);
            this.pnlEdicion.Name = "pnlEdicion";
            this.pnlEdicion.Size = new System.Drawing.Size(250, 390);
            this.pnlEdicion.TabIndex = 1;
            //
            // btnBorrarRutina
            //
            this.btnBorrarRutina.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBorrarRutina.BackColor = System.Drawing.Color.IndianRed;
            this.btnBorrarRutina.ForeColor = System.Drawing.Color.White;
            this.btnBorrarRutina.Location = new System.Drawing.Point(20, 345);
            this.btnBorrarRutina.Name = "btnBorrarRutina";
            this.btnBorrarRutina.Size = new System.Drawing.Size(210, 30);
            this.btnBorrarRutina.TabIndex = 11;
            this.btnBorrarRutina.Text = "BTN_BORRAR_RUTINA";
            this.btnBorrarRutina.UseVisualStyleBackColor = false;
            this.btnBorrarRutina.Click += new System.EventHandler(this.btnBorrarRutina_Click);
            //
            // btnGuardar
            //
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.Location = new System.Drawing.Point(20, 300);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(210, 30);
            this.btnGuardar.TabIndex = 10;
            this.btnGuardar.Text = "BTN_GUARDAR_RUTINA";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // btnEliminarEjercicio
            //
            this.btnEliminarEjercicio.Location = new System.Drawing.Point(130, 240);
            this.btnEliminarEjercicio.Name = "btnEliminarEjercicio";
            this.btnEliminarEjercicio.Size = new System.Drawing.Size(100, 23);
            this.btnEliminarEjercicio.TabIndex = 9;
            this.btnEliminarEjercicio.Text = "BTN_ELIMINAR_EJERCICIO";
            this.btnEliminarEjercicio.UseVisualStyleBackColor = true;
            this.btnEliminarEjercicio.Click += new System.EventHandler(this.btnEliminarEjercicio_Click);
            //
            // btnAgregarEjercicio
            //
            this.btnAgregarEjercicio.Location = new System.Drawing.Point(20, 240);
            this.btnAgregarEjercicio.Name = "btnAgregarEjercicio";
            this.btnAgregarEjercicio.Size = new System.Drawing.Size(100, 23);
            this.btnAgregarEjercicio.TabIndex = 8;
            this.btnAgregarEjercicio.Text = "BTN_AGREGAR_EJERCICIO";
            this.btnAgregarEjercicio.UseVisualStyleBackColor = true;
            this.btnAgregarEjercicio.Click += new System.EventHandler(this.btnAgregarEjercicio_Click);
            //
            // txtOrden
            //
            this.txtOrden.Location = new System.Drawing.Point(20, 200);
            this.txtOrden.Name = "txtOrden";
            this.txtOrden.Size = new System.Drawing.Size(210, 20);
            this.txtOrden.TabIndex = 7;
            //
            // lblOrden
            //
            this.lblOrden.AutoSize = true;
            this.lblOrden.Location = new System.Drawing.Point(20, 180);
            this.lblOrden.Name = "lblOrden";
            this.lblOrden.Size = new System.Drawing.Size(130, 13);
            this.lblOrden.TabIndex = 6;
            this.lblOrden.Text = "LBL_EJERCICIO_ORDEN";
            //
            // txtRepeticiones
            //
            this.txtRepeticiones.Location = new System.Drawing.Point(20, 150);
            this.txtRepeticiones.Name = "txtRepeticiones";
            this.txtRepeticiones.Size = new System.Drawing.Size(210, 20);
            this.txtRepeticiones.TabIndex = 5;
            //
            // lblRepeticiones
            //
            this.lblRepeticiones.AutoSize = true;
            this.lblRepeticiones.Location = new System.Drawing.Point(20, 130);
            this.lblRepeticiones.Name = "lblRepeticiones";
            this.lblRepeticiones.Size = new System.Drawing.Size(116, 13);
            this.lblRepeticiones.TabIndex = 4;
            this.lblRepeticiones.Text = "LBL_EJERCICIO_REP";
            //
            // cmbDiaSemana
            //
            this.cmbDiaSemana.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDiaSemana.FormattingEnabled = true;
            this.cmbDiaSemana.Location = new System.Drawing.Point(20, 100);
            this.cmbDiaSemana.Name = "cmbDiaSemana";
            this.cmbDiaSemana.Size = new System.Drawing.Size(210, 21);
            this.cmbDiaSemana.TabIndex = 3;
            //
            // lblDiaSemana
            //
            this.lblDiaSemana.AutoSize = true;
            this.lblDiaSemana.Location = new System.Drawing.Point(20, 80);
            this.lblDiaSemana.Name = "lblDiaSemana";
            this.lblDiaSemana.Size = new System.Drawing.Size(113, 13);
            this.lblDiaSemana.TabIndex = 2;
            this.lblDiaSemana.Text = "LBL_EJERCICIO_DIA";
            //
            // txtNombreEjercicio
            //
            this.txtNombreEjercicio.Location = new System.Drawing.Point(20, 50);
            this.txtNombreEjercicio.Name = "txtNombreEjercicio";
            this.txtNombreEjercicio.Size = new System.Drawing.Size(210, 20);
            this.txtNombreEjercicio.TabIndex = 1;
            //
            // lblNombreEjercicio
            //
            this.lblNombreEjercicio.AutoSize = true;
            this.lblNombreEjercicio.Location = new System.Drawing.Point(20, 30);
            this.lblNombreEjercicio.Name = "lblNombreEjercicio";
            this.lblNombreEjercicio.Size = new System.Drawing.Size(139, 13);
            this.lblNombreEjercicio.TabIndex = 0;
            this.lblNombreEjercicio.Text = "LBL_EJERCICIO_NOMBRE";
            //
            // dgvEjercicios
            //
            this.dgvEjercicios.AllowUserToAddRows = false;
            this.dgvEjercicios.AllowUserToDeleteRows = false;
            this.dgvEjercicios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEjercicios.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEjercicios.Location = new System.Drawing.Point(0, 60);
            this.dgvEjercicios.MultiSelect = false;
            this.dgvEjercicios.Name = "dgvEjercicios";
            this.dgvEjercicios.ReadOnly = true;
            this.dgvEjercicios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEjercicios.Size = new System.Drawing.Size(550, 390);
            this.dgvEjercicios.TabIndex = 2;
            this.dgvEjercicios.SelectionChanged += new System.EventHandler(this.dgvEjercicios_SelectionChanged);
            //
            // FrmRutina
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dgvEjercicios);
            this.Controls.Add(this.pnlEdicion);
            this.Controls.Add(this.pnlHeader);
            this.Name = "FrmRutina";
            this.Text = "RUTINA_TITLE";
            this.Load += new System.EventHandler(this.FrmRutina_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlEdicion.ResumeLayout(false);
            this.pnlEdicion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEjercicios)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.Label lblCliente;
        private System.Windows.Forms.Panel pnlEdicion;
        private System.Windows.Forms.DataGridView dgvEjercicios;
        private System.Windows.Forms.TextBox txtOrden;
        private System.Windows.Forms.Label lblOrden;
        private System.Windows.Forms.TextBox txtRepeticiones;
        private System.Windows.Forms.Label lblRepeticiones;
        private System.Windows.Forms.ComboBox cmbDiaSemana;
        private System.Windows.Forms.Label lblDiaSemana;
        private System.Windows.Forms.TextBox txtNombreEjercicio;
        private System.Windows.Forms.Label lblNombreEjercicio;
        private System.Windows.Forms.Button btnBorrarRutina;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnEliminarEjercicio;
        private System.Windows.Forms.Button btnAgregarEjercicio;
    }
}
