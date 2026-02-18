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
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.lblBuscar = new System.Windows.Forms.Label();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnGestionarRutina = new System.Windows.Forms.Button();
            this.dgvClientes = new System.Windows.Forms.DataGridView();
            this.pnlSearch.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).BeginInit();
            this.SuspendLayout();
            //
            // pnlSearch
            //
            this.pnlSearch.Controls.Add(this.txtBuscar);
            this.pnlSearch.Controls.Add(this.lblBuscar);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearch.Location = new System.Drawing.Point(0, 0);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size(600, 50);
            this.pnlSearch.TabIndex = 0;
            //
            // txtBuscar
            //
            this.txtBuscar.Location = new System.Drawing.Point(100, 15);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(200, 20);
            this.txtBuscar.TabIndex = 1;
            this.txtBuscar.TextChanged += new System.EventHandler(this.txtBuscar_TextChanged);
            //
            // lblBuscar
            //
            this.lblBuscar.AutoSize = true;
            this.lblBuscar.Location = new System.Drawing.Point(12, 18);
            this.lblBuscar.Name = "lblBuscar";
            this.lblBuscar.Size = new System.Drawing.Size(70, 13);
            this.lblBuscar.TabIndex = 0;
            this.lblBuscar.Text = "LBL_DNI"; // Reusing "LBL_DNI" as per previous translations
            //
            // pnlBottom
            //
            this.pnlBottom.Controls.Add(this.btnGestionarRutina);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 350);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(600, 50);
            this.pnlBottom.TabIndex = 1;
            //
            // btnGestionarRutina
            //
            this.btnGestionarRutina.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGestionarRutina.Location = new System.Drawing.Point(450, 10);
            this.btnGestionarRutina.Name = "btnGestionarRutina";
            this.btnGestionarRutina.Size = new System.Drawing.Size(130, 30);
            this.btnGestionarRutina.TabIndex = 0;
            this.btnGestionarRutina.Text = "BTN_GESTIONAR_RUTINA";
            this.btnGestionarRutina.UseVisualStyleBackColor = true;
            this.btnGestionarRutina.Click += new System.EventHandler(this.btnGestionarRutina_Click);
            //
            // dgvClientes
            //
            this.dgvClientes.AllowUserToAddRows = false;
            this.dgvClientes.AllowUserToDeleteRows = false;
            this.dgvClientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClientes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvClientes.Location = new System.Drawing.Point(0, 50);
            this.dgvClientes.MultiSelect = false;
            this.dgvClientes.Name = "dgvClientes";
            this.dgvClientes.ReadOnly = true;
            this.dgvClientes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvClientes.Size = new System.Drawing.Size(600, 300);
            this.dgvClientes.TabIndex = 2;
            this.dgvClientes.SelectionChanged += new System.EventHandler(this.dgvClientes_SelectionChanged);
            this.dgvClientes.DoubleClick += new System.EventHandler(this.dgvClientes_DoubleClick);
            //
            // FrmGestionRutinas
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.Controls.Add(this.dgvClientes);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlSearch);
            this.Name = "FrmGestionRutinas";
            this.Text = "FRM_GESTION_RUTINAS_TITLE";
            this.Load += new System.EventHandler(this.FrmGestionRutinas_Load);
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.pnlBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Label lblBuscar;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnGestionarRutina;
        private System.Windows.Forms.DataGridView dgvClientes;
    }
}
