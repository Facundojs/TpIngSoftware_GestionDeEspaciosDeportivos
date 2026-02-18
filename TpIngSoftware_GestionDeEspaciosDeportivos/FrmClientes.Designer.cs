namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    partial class FrmClientes
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
            this.pnlCheckIn = new System.Windows.Forms.Panel();
            this.lblResultado = new System.Windows.Forms.Label();
            this.btnCheckIn = new System.Windows.Forms.Button();
            this.txtDNICheckIn = new System.Windows.Forms.TextBox();
            this.lblDNICheckIn = new System.Windows.Forms.Label();
            this.pnlEdicion = new System.Windows.Forms.Panel();
            this.btnVerRutina = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnHabilitar = new System.Windows.Forms.Button();
            this.btnDeshabilitar = new System.Windows.Forms.Button();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.btnCrear = new System.Windows.Forms.Button();
            this.lblMembresia = new System.Windows.Forms.Label();
            this.cmbMembresia = new System.Windows.Forms.ComboBox();
            this.dtpFechaNacimiento = new System.Windows.Forms.DateTimePicker();
            this.lblFechaNacimiento = new System.Windows.Forms.Label();
            this.txtApellido = new System.Windows.Forms.TextBox();
            this.lblApellido = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtDNI = new System.Windows.Forms.TextBox();
            this.lblDNI = new System.Windows.Forms.Label();
            this.dgvClientes = new System.Windows.Forms.DataGridView();
            this.pnlCheckIn.SuspendLayout();
            this.pnlEdicion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).BeginInit();
            this.SuspendLayout();
            //
            // pnlCheckIn
            //
            this.pnlCheckIn.Controls.Add(this.lblResultado);
            this.pnlCheckIn.Controls.Add(this.btnCheckIn);
            this.pnlCheckIn.Controls.Add(this.txtDNICheckIn);
            this.pnlCheckIn.Controls.Add(this.lblDNICheckIn);
            this.pnlCheckIn.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlCheckIn.Location = new System.Drawing.Point(0, 450);
            this.pnlCheckIn.Name = "pnlCheckIn";
            this.pnlCheckIn.Size = new System.Drawing.Size(900, 100);
            this.pnlCheckIn.TabIndex = 0;
            //
            // lblResultado
            //
            this.lblResultado.AutoSize = true;
            this.lblResultado.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResultado.Location = new System.Drawing.Point(29, 60);
            this.lblResultado.Name = "lblResultado";
            this.lblResultado.Size = new System.Drawing.Size(0, 20);
            this.lblResultado.TabIndex = 3;
            //
            // btnCheckIn
            //
            this.btnCheckIn.Location = new System.Drawing.Point(200, 23);
            this.btnCheckIn.Name = "btnCheckIn";
            this.btnCheckIn.Size = new System.Drawing.Size(100, 23);
            this.btnCheckIn.TabIndex = 2;
            this.btnCheckIn.Text = "BTN_CHECK_IN";
            this.btnCheckIn.UseVisualStyleBackColor = true;
            this.btnCheckIn.Click += new System.EventHandler(this.btnCheckIn_Click);
            //
            // txtDNICheckIn
            //
            this.txtDNICheckIn.Location = new System.Drawing.Point(70, 25);
            this.txtDNICheckIn.Name = "txtDNICheckIn";
            this.txtDNICheckIn.Size = new System.Drawing.Size(120, 20);
            this.txtDNICheckIn.TabIndex = 1;
            //
            // lblDNICheckIn
            //
            this.lblDNICheckIn.AutoSize = true;
            this.lblDNICheckIn.Location = new System.Drawing.Point(26, 28);
            this.lblDNICheckIn.Name = "lblDNICheckIn";
            this.lblDNICheckIn.Size = new System.Drawing.Size(46, 13);
            this.lblDNICheckIn.TabIndex = 0;
            this.lblDNICheckIn.Text = "LBL_DNI";
            //
            // pnlEdicion
            //
            this.pnlEdicion.Controls.Add(this.btnVerRutina);
            this.pnlEdicion.Controls.Add(this.btnLimpiar);
            this.pnlEdicion.Controls.Add(this.btnHabilitar);
            this.pnlEdicion.Controls.Add(this.btnDeshabilitar);
            this.pnlEdicion.Controls.Add(this.btnActualizar);
            this.pnlEdicion.Controls.Add(this.btnCrear);
            this.pnlEdicion.Controls.Add(this.lblMembresia);
            this.pnlEdicion.Controls.Add(this.cmbMembresia);
            this.pnlEdicion.Controls.Add(this.dtpFechaNacimiento);
            this.pnlEdicion.Controls.Add(this.lblFechaNacimiento);
            this.pnlEdicion.Controls.Add(this.txtApellido);
            this.pnlEdicion.Controls.Add(this.lblApellido);
            this.pnlEdicion.Controls.Add(this.txtNombre);
            this.pnlEdicion.Controls.Add(this.lblNombre);
            this.pnlEdicion.Controls.Add(this.txtDNI);
            this.pnlEdicion.Controls.Add(this.lblDNI);
            this.pnlEdicion.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlEdicion.Location = new System.Drawing.Point(600, 0);
            this.pnlEdicion.Name = "pnlEdicion";
            this.pnlEdicion.Size = new System.Drawing.Size(300, 450);
            this.pnlEdicion.TabIndex = 1;
            //
            // btnLimpiar
            //
            this.btnLimpiar.Location = new System.Drawing.Point(20, 360);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(260, 23);
            this.btnLimpiar.TabIndex = 14;
            this.btnLimpiar.Text = "BTN_LIMPIAR";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            //
            // btnVerRutina
            //
            this.btnVerRutina.Location = new System.Drawing.Point(20, 400);
            this.btnVerRutina.Name = "btnVerRutina";
            this.btnVerRutina.Size = new System.Drawing.Size(260, 23);
            this.btnVerRutina.TabIndex = 15;
            this.btnVerRutina.Text = "BTN_VER_RUTINA";
            this.btnVerRutina.UseVisualStyleBackColor = true;
            this.btnVerRutina.Click += new System.EventHandler(this.btnVerRutina_Click);
            //
            // btnHabilitar
            //
            this.btnHabilitar.Location = new System.Drawing.Point(160, 320);
            this.btnHabilitar.Name = "btnHabilitar";
            this.btnHabilitar.Size = new System.Drawing.Size(120, 23);
            this.btnHabilitar.TabIndex = 13;
            this.btnHabilitar.Text = "BTN_HABILITAR";
            this.btnHabilitar.UseVisualStyleBackColor = true;
            this.btnHabilitar.Click += new System.EventHandler(this.btnHabilitar_Click);
            //
            // btnDeshabilitar
            //
            this.btnDeshabilitar.Location = new System.Drawing.Point(20, 320);
            this.btnDeshabilitar.Name = "btnDeshabilitar";
            this.btnDeshabilitar.Size = new System.Drawing.Size(120, 23);
            this.btnDeshabilitar.TabIndex = 12;
            this.btnDeshabilitar.Text = "BTN_DESHABILITAR";
            this.btnDeshabilitar.UseVisualStyleBackColor = true;
            this.btnDeshabilitar.Click += new System.EventHandler(this.btnDeshabilitar_Click);
            //
            // btnActualizar
            //
            this.btnActualizar.Location = new System.Drawing.Point(160, 280);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(120, 23);
            this.btnActualizar.TabIndex = 11;
            this.btnActualizar.Text = "BTN_ACTUALIZAR";
            this.btnActualizar.UseVisualStyleBackColor = true;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            //
            // btnCrear
            //
            this.btnCrear.Location = new System.Drawing.Point(20, 280);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.Size = new System.Drawing.Size(120, 23);
            this.btnCrear.TabIndex = 10;
            this.btnCrear.Text = "BTN_CREAR";
            this.btnCrear.UseVisualStyleBackColor = true;
            this.btnCrear.Click += new System.EventHandler(this.btnCrear_Click);
            //
            // lblMembresia
            //
            this.lblMembresia.AutoSize = true;
            this.lblMembresia.Location = new System.Drawing.Point(20, 220);
            this.lblMembresia.Name = "lblMembresia";
            this.lblMembresia.Size = new System.Drawing.Size(91, 13);
            this.lblMembresia.TabIndex = 9;
            this.lblMembresia.Text = "LBL_MEMBRESIA";
            //
            // cmbMembresia
            //
            this.cmbMembresia.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMembresia.FormattingEnabled = true;
            this.cmbMembresia.Location = new System.Drawing.Point(20, 240);
            this.cmbMembresia.Name = "cmbMembresia";
            this.cmbMembresia.Size = new System.Drawing.Size(260, 21);
            this.cmbMembresia.TabIndex = 8;
            //
            // dtpFechaNacimiento
            //
            this.dtpFechaNacimiento.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaNacimiento.Location = new System.Drawing.Point(20, 190);
            this.dtpFechaNacimiento.Name = "dtpFechaNacimiento";
            this.dtpFechaNacimiento.Size = new System.Drawing.Size(260, 20);
            this.dtpFechaNacimiento.TabIndex = 7;
            //
            // lblFechaNacimiento
            //
            this.lblFechaNacimiento.AutoSize = true;
            this.lblFechaNacimiento.Location = new System.Drawing.Point(20, 170);
            this.lblFechaNacimiento.Name = "lblFechaNacimiento";
            this.lblFechaNacimiento.Size = new System.Drawing.Size(94, 13);
            this.lblFechaNacimiento.TabIndex = 6;
            this.lblFechaNacimiento.Text = "LBL_FECHA_NAC";
            //
            // txtApellido
            //
            this.txtApellido.Location = new System.Drawing.Point(20, 140);
            this.txtApellido.Name = "txtApellido";
            this.txtApellido.Size = new System.Drawing.Size(260, 20);
            this.txtApellido.TabIndex = 5;
            //
            // lblApellido
            //
            this.lblApellido.AutoSize = true;
            this.lblApellido.Location = new System.Drawing.Point(20, 120);
            this.lblApellido.Name = "lblApellido";
            this.lblApellido.Size = new System.Drawing.Size(80, 13);
            this.lblApellido.TabIndex = 4;
            this.lblApellido.Text = "LBL_APELLIDO";
            //
            // txtNombre
            //
            this.txtNombre.Location = new System.Drawing.Point(20, 90);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(260, 20);
            this.txtNombre.TabIndex = 3;
            //
            // lblNombre
            //
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(20, 70);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(75, 13);
            this.lblNombre.TabIndex = 2;
            this.lblNombre.Text = "LBL_NOMBRE";
            //
            // txtDNI
            //
            this.txtDNI.Location = new System.Drawing.Point(20, 40);
            this.txtDNI.Name = "txtDNI";
            this.txtDNI.Size = new System.Drawing.Size(260, 20);
            this.txtDNI.TabIndex = 1;
            //
            // lblDNI
            //
            this.lblDNI.AutoSize = true;
            this.lblDNI.Location = new System.Drawing.Point(20, 20);
            this.lblDNI.Name = "lblDNI";
            this.lblDNI.Size = new System.Drawing.Size(46, 13);
            this.lblDNI.TabIndex = 0;
            this.lblDNI.Text = "LBL_DNI";
            //
            // dgvClientes
            //
            this.dgvClientes.AllowUserToAddRows = false;
            this.dgvClientes.AllowUserToDeleteRows = false;
            this.dgvClientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClientes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvClientes.Location = new System.Drawing.Point(0, 0);
            this.dgvClientes.MultiSelect = false;
            this.dgvClientes.Name = "dgvClientes";
            this.dgvClientes.ReadOnly = true;
            this.dgvClientes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvClientes.Size = new System.Drawing.Size(600, 450);
            this.dgvClientes.TabIndex = 2;
            this.dgvClientes.SelectionChanged += new System.EventHandler(this.dgvClientes_SelectionChanged);
            //
            // FrmClientes
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 550);
            this.Controls.Add(this.dgvClientes);
            this.Controls.Add(this.pnlEdicion);
            this.Controls.Add(this.pnlCheckIn);
            this.Name = "FrmClientes";
            this.Text = "CLIENTE_TITLE";
            this.Load += new System.EventHandler(this.FrmClientes_Load);
            this.pnlCheckIn.ResumeLayout(false);
            this.pnlCheckIn.PerformLayout();
            this.pnlEdicion.ResumeLayout(false);
            this.pnlEdicion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlCheckIn;
        private System.Windows.Forms.Panel pnlEdicion;
        private System.Windows.Forms.DataGridView dgvClientes;
        private System.Windows.Forms.Label lblResultado;
        private System.Windows.Forms.Button btnCheckIn;
        private System.Windows.Forms.TextBox txtDNICheckIn;
        private System.Windows.Forms.Label lblDNICheckIn;
        private System.Windows.Forms.Button btnVerRutina;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnHabilitar;
        private System.Windows.Forms.Button btnDeshabilitar;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Button btnCrear;
        private System.Windows.Forms.Label lblMembresia;
        private System.Windows.Forms.ComboBox cmbMembresia;
        private System.Windows.Forms.DateTimePicker dtpFechaNacimiento;
        private System.Windows.Forms.Label lblFechaNacimiento;
        private System.Windows.Forms.TextBox txtApellido;
        private System.Windows.Forms.Label lblApellido;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtDNI;
        private System.Windows.Forms.Label lblDNI;
    }
}
