namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    partial class FrmReservas
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
            this.cbEspacio = new System.Windows.Forms.ComboBox();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.dtpHora = new System.Windows.Forms.DateTimePicker();
            this.numDuracion = new System.Windows.Forms.NumericUpDown();
            this.txtDniCliente = new System.Windows.Forms.TextBox();
            this.btnBuscarCliente = new System.Windows.Forms.Button();
            this.lblNombreCliente = new System.Windows.Forms.Label();
            this.numAdelanto = new System.Windows.Forms.NumericUpDown();
            this.dgvReservas = new System.Windows.Forms.DataGridView();
            this.btnVerificar = new System.Windows.Forms.Button();
            this.btnGenerar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.lblEspacio = new System.Windows.Forms.Label();
            this.lblFecha = new System.Windows.Forms.Label();
            this.lblHora = new System.Windows.Forms.Label();
            this.lblDuracion = new System.Windows.Forms.Label();
            this.lblAdelanto = new System.Windows.Forms.Label();
            this.lblDni = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numDuracion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAdelanto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReservas)).BeginInit();
            this.SuspendLayout();
            //
            // cbEspacio
            //
            this.cbEspacio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEspacio.FormattingEnabled = true;
            this.cbEspacio.Location = new System.Drawing.Point(12, 29);
            this.cbEspacio.Name = "cbEspacio";
            this.cbEspacio.Size = new System.Drawing.Size(200, 21);
            this.cbEspacio.TabIndex = 0;
            //
            // dtpFecha
            //
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFecha.Location = new System.Drawing.Point(230, 30);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(120, 20);
            this.dtpFecha.TabIndex = 1;
            //
            // dtpHora
            //
            this.dtpHora.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpHora.ShowUpDown = true;
            this.dtpHora.Location = new System.Drawing.Point(370, 30);
            this.dtpHora.Name = "dtpHora";
            this.dtpHora.Size = new System.Drawing.Size(100, 20);
            this.dtpHora.TabIndex = 2;
            //
            // numDuracion
            //
            this.numDuracion.Location = new System.Drawing.Point(490, 30);
            this.numDuracion.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.numDuracion.Minimum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numDuracion.Name = "numDuracion";
            this.numDuracion.Size = new System.Drawing.Size(120, 20);
            this.numDuracion.TabIndex = 3;
            this.numDuracion.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            //
            // btnVerificar
            //
            this.btnVerificar.Location = new System.Drawing.Point(630, 27);
            this.btnVerificar.Name = "btnVerificar";
            this.btnVerificar.Size = new System.Drawing.Size(140, 23);
            this.btnVerificar.TabIndex = 4;
            this.btnVerificar.Text = "Verificar Disponibilidad";
            this.btnVerificar.UseVisualStyleBackColor = true;
            this.btnVerificar.Click += new System.EventHandler(this.btnVerificar_Click);
            //
            // txtDniCliente
            //
            this.txtDniCliente.Location = new System.Drawing.Point(12, 80);
            this.txtDniCliente.Name = "txtDniCliente";
            this.txtDniCliente.Size = new System.Drawing.Size(120, 20);
            this.txtDniCliente.TabIndex = 5;
            //
            // btnBuscarCliente
            //
            this.btnBuscarCliente.Location = new System.Drawing.Point(138, 78);
            this.btnBuscarCliente.Name = "btnBuscarCliente";
            this.btnBuscarCliente.Size = new System.Drawing.Size(75, 23);
            this.btnBuscarCliente.TabIndex = 6;
            this.btnBuscarCliente.Text = "Buscar";
            this.btnBuscarCliente.UseVisualStyleBackColor = true;
            this.btnBuscarCliente.Click += new System.EventHandler(this.btnBuscarCliente_Click);
            //
            // lblNombreCliente
            //
            this.lblNombreCliente.AutoSize = true;
            this.lblNombreCliente.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNombreCliente.Location = new System.Drawing.Point(230, 83);
            this.lblNombreCliente.Name = "lblNombreCliente";
            this.lblNombreCliente.Size = new System.Drawing.Size(11, 13);
            this.lblNombreCliente.TabIndex = 7;
            this.lblNombreCliente.Text = "-";
            //
            // numAdelanto
            //
            this.numAdelanto.DecimalPlaces = 2;
            this.numAdelanto.Location = new System.Drawing.Point(490, 81);
            this.numAdelanto.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numAdelanto.Name = "numAdelanto";
            this.numAdelanto.Size = new System.Drawing.Size(120, 20);
            this.numAdelanto.TabIndex = 8;
            //
            // btnGenerar
            //
            this.btnGenerar.Location = new System.Drawing.Point(630, 78);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(140, 23);
            this.btnGenerar.TabIndex = 9;
            this.btnGenerar.Text = "Generar Reserva";
            this.btnGenerar.UseVisualStyleBackColor = true;
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            //
            // dgvReservas
            //
            this.dgvReservas.AllowUserToAddRows = false;
            this.dgvReservas.AllowUserToDeleteRows = false;
            this.dgvReservas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvReservas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvReservas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReservas.Location = new System.Drawing.Point(12, 120);
            this.dgvReservas.Name = "dgvReservas";
            this.dgvReservas.ReadOnly = true;
            this.dgvReservas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReservas.Size = new System.Drawing.Size(760, 300);
            this.dgvReservas.TabIndex = 10;
            //
            // btnCancelar
            //
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.Location = new System.Drawing.Point(632, 426);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(140, 23);
            this.btnCancelar.TabIndex = 11;
            this.btnCancelar.Text = "Cancelar Reserva";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // lblEspacio
            //
            this.lblEspacio.AutoSize = true;
            this.lblEspacio.Location = new System.Drawing.Point(12, 13);
            this.lblEspacio.Name = "lblEspacio";
            this.lblEspacio.Size = new System.Drawing.Size(45, 13);
            this.lblEspacio.TabIndex = 12;
            this.lblEspacio.Text = "Espacio";
            //
            // lblFecha
            //
            this.lblFecha.AutoSize = true;
            this.lblFecha.Location = new System.Drawing.Point(230, 13);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.Size = new System.Drawing.Size(37, 13);
            this.lblFecha.TabIndex = 13;
            this.lblFecha.Text = "Fecha";
            //
            // lblHora
            //
            this.lblHora.AutoSize = true;
            this.lblHora.Location = new System.Drawing.Point(370, 13);
            this.lblHora.Name = "lblHora";
            this.lblHora.Size = new System.Drawing.Size(30, 13);
            this.lblHora.TabIndex = 14;
            this.lblHora.Text = "Hora";
            //
            // lblDuracion
            //
            this.lblDuracion.AutoSize = true;
            this.lblDuracion.Location = new System.Drawing.Point(490, 13);
            this.lblDuracion.Name = "lblDuracion";
            this.lblDuracion.Size = new System.Drawing.Size(50, 13);
            this.lblDuracion.TabIndex = 15;
            this.lblDuracion.Text = "Duración";
            //
            // lblAdelanto
            //
            this.lblAdelanto.AutoSize = true;
            this.lblAdelanto.Location = new System.Drawing.Point(490, 65);
            this.lblAdelanto.Name = "lblAdelanto";
            this.lblAdelanto.Size = new System.Drawing.Size(49, 13);
            this.lblAdelanto.TabIndex = 16;
            this.lblAdelanto.Text = "Adelanto";
            //
            // lblDni
            //
            this.lblDni.AutoSize = true;
            this.lblDni.Location = new System.Drawing.Point(12, 64);
            this.lblDni.Name = "lblDni";
            this.lblDni.Size = new System.Drawing.Size(61, 13);
            this.lblDni.TabIndex = 17;
            this.lblDni.Text = "DNI Cliente";
            //
            // FrmReservas
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.lblDni);
            this.Controls.Add(this.lblAdelanto);
            this.Controls.Add(this.lblDuracion);
            this.Controls.Add(this.lblHora);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.lblEspacio);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.dgvReservas);
            this.Controls.Add(this.btnGenerar);
            this.Controls.Add(this.numAdelanto);
            this.Controls.Add(this.lblNombreCliente);
            this.Controls.Add(this.btnBuscarCliente);
            this.Controls.Add(this.txtDniCliente);
            this.Controls.Add(this.btnVerificar);
            this.Controls.Add(this.numDuracion);
            this.Controls.Add(this.dtpHora);
            this.Controls.Add(this.dtpFecha);
            this.Controls.Add(this.cbEspacio);
            this.Name = "FrmReservas";
            this.Text = "Gestión de Reservas";
            ((System.ComponentModel.ISupportInitialize)(this.numDuracion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAdelanto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReservas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.ComboBox cbEspacio;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.DateTimePicker dtpHora;
        private System.Windows.Forms.NumericUpDown numDuracion;
        private System.Windows.Forms.TextBox txtDniCliente;
        private System.Windows.Forms.Button btnBuscarCliente;
        private System.Windows.Forms.Label lblNombreCliente;
        private System.Windows.Forms.NumericUpDown numAdelanto;
        private System.Windows.Forms.DataGridView dgvReservas;
        private System.Windows.Forms.Button btnVerificar;
        private System.Windows.Forms.Button btnGenerar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Label lblEspacio;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.Label lblHora;
        private System.Windows.Forms.Label lblDuracion;
        private System.Windows.Forms.Label lblAdelanto;
        private System.Windows.Forms.Label lblDni;
    }
}
