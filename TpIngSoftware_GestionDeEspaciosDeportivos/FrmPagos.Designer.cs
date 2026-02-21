namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    partial class FrmPagos
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvPagos = new System.Windows.Forms.DataGridView();
            this.grpRegistro = new System.Windows.Forms.GroupBox();
            this.btnRegistrar = new System.Windows.Forms.Button();
            this.txtDetalle = new System.Windows.Forms.TextBox();
            this.lblDetalle = new System.Windows.Forms.Label();
            this.cmbMetodo = new System.Windows.Forms.ComboBox();
            this.lblMetodo = new System.Windows.Forms.Label();
            this.txtMonto = new System.Windows.Forms.TextBox();
            this.lblMonto = new System.Windows.Forms.Label();
            this.txtDNICliente = new System.Windows.Forms.TextBox();
            this.lblDNI = new System.Windows.Forms.Label();
            this.grpFiltros = new System.Windows.Forms.GroupBox();
            this.btnFiltrar = new System.Windows.Forms.Button();
            this.txtDNIFiltro = new System.Windows.Forms.TextBox();
            this.lblDNIFiltro = new System.Windows.Forms.Label();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.lblHasta = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.lblDesde = new System.Windows.Forms.Label();
            this.btnReembolsar = new System.Windows.Forms.Button();
            this.btnAdjuntarComprobante = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPagos)).BeginInit();
            this.grpRegistro.SuspendLayout();
            this.grpFiltros.SuspendLayout();
            this.SuspendLayout();
            //
            // dgvPagos
            //
            this.dgvPagos.AllowUserToAddRows = false;
            this.dgvPagos.AllowUserToDeleteRows = false;
            this.dgvPagos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPagos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPagos.Location = new System.Drawing.Point(12, 190);
            this.dgvPagos.MultiSelect = false;
            this.dgvPagos.Name = "dgvPagos";
            this.dgvPagos.ReadOnly = true;
            this.dgvPagos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPagos.Size = new System.Drawing.Size(960, 310);
            this.dgvPagos.TabIndex = 0;
            this.dgvPagos.SelectionChanged += new System.EventHandler(this.dgvPagos_SelectionChanged);
            //
            // grpRegistro
            //
            this.grpRegistro.Controls.Add(this.btnRegistrar);
            this.grpRegistro.Controls.Add(this.txtDetalle);
            this.grpRegistro.Controls.Add(this.lblDetalle);
            this.grpRegistro.Controls.Add(this.cmbMetodo);
            this.grpRegistro.Controls.Add(this.lblMetodo);
            this.grpRegistro.Controls.Add(this.txtMonto);
            this.grpRegistro.Controls.Add(this.lblMonto);
            this.grpRegistro.Controls.Add(this.txtDNICliente);
            this.grpRegistro.Controls.Add(this.lblDNI);
            this.grpRegistro.Controls.Add(this.lblNombreCliente);
            this.grpRegistro.Location = new System.Drawing.Point(12, 12);
            this.grpRegistro.Name = "grpRegistro";
            this.grpRegistro.Size = new System.Drawing.Size(960, 80);
            this.grpRegistro.TabIndex = 1;
            this.grpRegistro.TabStop = false;
            this.grpRegistro.Text = "Registrar Nuevo Pago";
            //
            // btnRegistrar
            //
            this.btnRegistrar.Location = new System.Drawing.Point(850, 30);
            this.btnRegistrar.Name = "btnRegistrar";
            this.btnRegistrar.Size = new System.Drawing.Size(100, 30);
            this.btnRegistrar.TabIndex = 8;
            this.btnRegistrar.Text = "Registrar";
            this.btnRegistrar.UseVisualStyleBackColor = true;
            this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);
            //
            // txtDetalle
            //
            this.txtDetalle.Location = new System.Drawing.Point(620, 35);
            this.txtDetalle.Name = "txtDetalle";
            this.txtDetalle.Size = new System.Drawing.Size(200, 20);
            this.txtDetalle.TabIndex = 7;
            //
            // lblDetalle
            //
            this.lblDetalle.AutoSize = true;
            this.lblDetalle.Location = new System.Drawing.Point(570, 38);
            this.lblDetalle.Name = "lblDetalle";
            this.lblDetalle.Size = new System.Drawing.Size(43, 13);
            this.lblDetalle.TabIndex = 6;
            this.lblDetalle.Text = "Detalle:";
            //
            // cmbMetodo
            //
            this.cmbMetodo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMetodo.FormattingEnabled = true;
            this.cmbMetodo.Items.AddRange(new object[] {
            "Efectivo",
            "Tarjeta",
            "Transferencia"});
            this.cmbMetodo.Location = new System.Drawing.Point(400, 35);
            this.cmbMetodo.Name = "cmbMetodo";
            this.cmbMetodo.Size = new System.Drawing.Size(150, 21);
            this.cmbMetodo.TabIndex = 5;
            //
            // lblMetodo
            //
            this.lblMetodo.AutoSize = true;
            this.lblMetodo.Location = new System.Drawing.Point(350, 38);
            this.lblMetodo.Name = "lblMetodo";
            this.lblMetodo.Size = new System.Drawing.Size(46, 13);
            this.lblMetodo.TabIndex = 4;
            this.lblMetodo.Text = "Método:";
            //
            // txtMonto
            //
            this.txtMonto.Location = new System.Drawing.Point(230, 35);
            this.txtMonto.Name = "txtMonto";
            this.txtMonto.Size = new System.Drawing.Size(100, 20);
            this.txtMonto.TabIndex = 3;
            //
            // lblMonto
            //
            this.lblMonto.AutoSize = true;
            this.lblMonto.Location = new System.Drawing.Point(180, 38);
            this.lblMonto.Name = "lblMonto";
            this.lblMonto.Size = new System.Drawing.Size(40, 13);
            this.lblMonto.TabIndex = 2;
            this.lblMonto.Text = "Monto:";
            //
            // txtDNICliente
            //
            this.txtDNICliente.Location = new System.Drawing.Point(60, 35);
            this.txtDNICliente.Name = "txtDNICliente";
            this.txtDNICliente.Size = new System.Drawing.Size(100, 20);
            this.txtDNICliente.TabIndex = 1;
            this.txtDNICliente.Leave += new System.EventHandler(this.txtDNICliente_Leave);
            //
            // lblDNI
            //
            this.lblDNI.AutoSize = true;
            this.lblDNI.Location = new System.Drawing.Point(20, 38);
            this.lblDNI.Name = "lblDNI";
            this.lblDNI.Size = new System.Drawing.Size(29, 13);
            this.lblDNI.TabIndex = 0;
            this.lblDNI.Text = "DNI:";
            //
            // lblNombreCliente
            //
            this.lblNombreCliente.AutoSize = true;
            this.lblNombreCliente.Location = new System.Drawing.Point(60, 60);
            this.lblNombreCliente.Name = "lblNombreCliente";
            this.lblNombreCliente.Size = new System.Drawing.Size(0, 13);
            this.lblNombreCliente.TabIndex = 9;
            this.lblNombreCliente.ForeColor = System.Drawing.Color.Blue;
            //
            // grpFiltros
            //
            this.grpFiltros.Controls.Add(this.btnFiltrar);
            this.grpFiltros.Controls.Add(this.txtDNIFiltro);
            this.grpFiltros.Controls.Add(this.lblDNIFiltro);
            this.grpFiltros.Controls.Add(this.dtpHasta);
            this.grpFiltros.Controls.Add(this.lblHasta);
            this.grpFiltros.Controls.Add(this.dtpDesde);
            this.grpFiltros.Controls.Add(this.lblDesde);
            this.grpFiltros.Location = new System.Drawing.Point(12, 100);
            this.grpFiltros.Name = "grpFiltros";
            this.grpFiltros.Size = new System.Drawing.Size(960, 70);
            this.grpFiltros.TabIndex = 2;
            this.grpFiltros.TabStop = false;
            this.grpFiltros.Text = "Filtros";
            //
            // btnFiltrar
            //
            this.btnFiltrar.Location = new System.Drawing.Point(850, 25);
            this.btnFiltrar.Name = "btnFiltrar";
            this.btnFiltrar.Size = new System.Drawing.Size(100, 30);
            this.btnFiltrar.TabIndex = 6;
            this.btnFiltrar.Text = "Filtrar";
            this.btnFiltrar.UseVisualStyleBackColor = true;
            this.btnFiltrar.Click += new System.EventHandler(this.btnFiltrar_Click);
            //
            // txtDNIFiltro
            //
            this.txtDNIFiltro.Location = new System.Drawing.Point(500, 30);
            this.txtDNIFiltro.Name = "txtDNIFiltro";
            this.txtDNIFiltro.Size = new System.Drawing.Size(100, 20);
            this.txtDNIFiltro.TabIndex = 5;
            //
            // lblDNIFiltro
            //
            this.lblDNIFiltro.AutoSize = true;
            this.lblDNIFiltro.Location = new System.Drawing.Point(460, 33);
            this.lblDNIFiltro.Name = "lblDNIFiltro";
            this.lblDNIFiltro.Size = new System.Drawing.Size(29, 13);
            this.lblDNIFiltro.TabIndex = 4;
            this.lblDNIFiltro.Text = "DNI:";
            //
            // dtpHasta
            //
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(280, 30);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(100, 20);
            this.dtpHasta.TabIndex = 3;
            //
            // lblHasta
            //
            this.lblHasta.AutoSize = true;
            this.lblHasta.Location = new System.Drawing.Point(230, 33);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Size = new System.Drawing.Size(38, 13);
            this.lblHasta.TabIndex = 2;
            this.lblHasta.Text = "Hasta:";
            //
            // dtpDesde
            //
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(60, 30);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(100, 20);
            this.dtpDesde.TabIndex = 1;
            //
            // lblDesde
            //
            this.lblDesde.AutoSize = true;
            this.lblDesde.Location = new System.Drawing.Point(10, 33);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.Size = new System.Drawing.Size(41, 13);
            this.lblDesde.TabIndex = 0;
            this.lblDesde.Text = "Desde:";
            //
            // btnReembolsar
            //
            this.btnReembolsar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReembolsar.Location = new System.Drawing.Point(822, 510);
            this.btnReembolsar.Name = "btnReembolsar";
            this.btnReembolsar.Size = new System.Drawing.Size(150, 30);
            this.btnReembolsar.TabIndex = 3;
            this.btnReembolsar.Text = "Reembolsar";
            this.btnReembolsar.UseVisualStyleBackColor = true;
            this.btnReembolsar.Click += new System.EventHandler(this.btnReembolsar_Click);
            //
            // btnAdjuntarComprobante
            //
            this.btnAdjuntarComprobante.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdjuntarComprobante.Location = new System.Drawing.Point(660, 510);
            this.btnAdjuntarComprobante.Name = "btnAdjuntarComprobante";
            this.btnAdjuntarComprobante.Size = new System.Drawing.Size(150, 30);
            this.btnAdjuntarComprobante.TabIndex = 4;
            this.btnAdjuntarComprobante.Text = "Adjuntar Comprobante";
            this.btnAdjuntarComprobante.UseVisualStyleBackColor = true;
            this.btnAdjuntarComprobante.Click += new System.EventHandler(this.btnAdjuntarComprobante_Click);
            //
            // FrmPagos
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this.btnAdjuntarComprobante);
            this.Controls.Add(this.btnReembolsar);
            this.Controls.Add(this.grpFiltros);
            this.Controls.Add(this.grpRegistro);
            this.Controls.Add(this.dgvPagos);
            this.Name = "FrmPagos";
            this.Text = "Gestión de Pagos";
            this.Load += new System.EventHandler(this.FrmPagos_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPagos)).EndInit();
            this.grpRegistro.ResumeLayout(false);
            this.grpRegistro.PerformLayout();
            this.grpFiltros.ResumeLayout(false);
            this.grpFiltros.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPagos;
        private System.Windows.Forms.GroupBox grpRegistro;
        private System.Windows.Forms.Label lblDNI;
        private System.Windows.Forms.TextBox txtDNICliente;
        private System.Windows.Forms.Label lblNombreCliente;
        private System.Windows.Forms.Label lblMonto;
        private System.Windows.Forms.TextBox txtMonto;
        private System.Windows.Forms.Label lblMetodo;
        private System.Windows.Forms.ComboBox cmbMetodo;
        private System.Windows.Forms.Label lblDetalle;
        private System.Windows.Forms.TextBox txtDetalle;
        private System.Windows.Forms.Button btnRegistrar;
        private System.Windows.Forms.GroupBox grpFiltros;
        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Label lblDNIFiltro;
        private System.Windows.Forms.TextBox txtDNIFiltro;
        private System.Windows.Forms.Button btnFiltrar;
        private System.Windows.Forms.Button btnReembolsar;
        private System.Windows.Forms.Button btnAdjuntarComprobante;
    }
}
