namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    partial class FrmAgenda
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
            this.dgvAgenda = new System.Windows.Forms.DataGridView();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.lblDesde = new System.Windows.Forms.Label();
            this.lblHasta = new System.Windows.Forms.Label();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAgenda)).BeginInit();
            this.SuspendLayout();
            //
            // dgvAgenda
            //
            this.dgvAgenda.AllowUserToAddRows = false;
            this.dgvAgenda.AllowUserToDeleteRows = false;
            this.dgvAgenda.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAgenda.Location = new System.Drawing.Point(12, 12);
            this.dgvAgenda.Name = "dgvAgenda";
            this.dgvAgenda.ReadOnly = true;
            this.dgvAgenda.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAgenda.Size = new System.Drawing.Size(300, 200);
            this.dgvAgenda.TabIndex = 0;
            //
            // dtpDesde
            //
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpDesde.Location = new System.Drawing.Point(340, 30);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.ShowUpDown = true;
            this.dtpDesde.Size = new System.Drawing.Size(100, 20);
            this.dtpDesde.TabIndex = 1;
            //
            // dtpHasta
            //
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpHasta.Location = new System.Drawing.Point(340, 80);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.ShowUpDown = true;
            this.dtpHasta.Size = new System.Drawing.Size(100, 20);
            this.dtpHasta.TabIndex = 2;
            //
            // lblDesde
            //
            this.lblDesde.AutoSize = true;
            this.lblDesde.Location = new System.Drawing.Point(337, 14);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.Size = new System.Drawing.Size(65, 13);
            this.lblDesde.TabIndex = 3;
            this.lblDesde.Text = "Hora Desde";
            //
            // lblHasta
            //
            this.lblHasta.AutoSize = true;
            this.lblHasta.Location = new System.Drawing.Point(337, 64);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Size = new System.Drawing.Size(62, 13);
            this.lblHasta.TabIndex = 4;
            this.lblHasta.Text = "Hora Hasta";
            //
            // btnAgregar
            //
            this.btnAgregar.Location = new System.Drawing.Point(340, 115);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(100, 23);
            this.btnAgregar.TabIndex = 5;
            this.btnAgregar.Text = "Agregar";
            this.btnAgregar.UseVisualStyleBackColor = true;
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            //
            // btnEliminar
            //
            this.btnEliminar.Location = new System.Drawing.Point(340, 144);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(100, 23);
            this.btnEliminar.TabIndex = 6;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            //
            // btnGuardar
            //
            this.btnGuardar.Location = new System.Drawing.Point(340, 189);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(100, 23);
            this.btnGuardar.TabIndex = 7;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // FrmAgenda
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 230);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnAgregar);
            this.Controls.Add(this.lblHasta);
            this.Controls.Add(this.lblDesde);
            this.Controls.Add(this.dtpHasta);
            this.Controls.Add(this.dtpDesde);
            this.Controls.Add(this.dgvAgenda);
            this.Name = "FrmAgenda";
            this.Text = "FrmAgenda";
            this.Load += new System.EventHandler(this.FrmAgenda_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAgenda)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.DataGridView dgvAgenda;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnGuardar;
    }
}
