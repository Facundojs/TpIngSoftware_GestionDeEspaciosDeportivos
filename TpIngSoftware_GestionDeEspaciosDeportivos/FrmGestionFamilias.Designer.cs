namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    partial class FrmGestionFamilias
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
            this.lstFamilias = new System.Windows.Forms.ListBox();
            this.lstPatentes = new System.Windows.Forms.CheckedListBox();
            this.lstSubFamilias = new System.Windows.Forms.CheckedListBox();
            this.txtNombreFamilia = new System.Windows.Forms.TextBox();
            this.lblNombreFamilia = new System.Windows.Forms.Label();
            this.btnCrear = new System.Windows.Forms.Button();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.lblFamilias = new System.Windows.Forms.Label();
            this.lblPatentes = new System.Windows.Forms.Label();
            this.lblSubFamilias = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // lstFamilias
            //
            this.lstFamilias.FormattingEnabled = true;
            this.lstFamilias.ItemHeight = 16;
            this.lstFamilias.Location = new System.Drawing.Point(12, 33);
            this.lstFamilias.Name = "lstFamilias";
            this.lstFamilias.Size = new System.Drawing.Size(180, 324);
            this.lstFamilias.TabIndex = 0;
            this.lstFamilias.SelectedIndexChanged += new System.EventHandler(this.lstFamilias_SelectedIndexChanged);
            //
            // lstPatentes
            //
            this.lstPatentes.FormattingEnabled = true;
            this.lstPatentes.Location = new System.Drawing.Point(205, 33);
            this.lstPatentes.Name = "lstPatentes";
            this.lstPatentes.Size = new System.Drawing.Size(200, 327);
            this.lstPatentes.TabIndex = 1;
            //
            // lstSubFamilias
            //
            this.lstSubFamilias.FormattingEnabled = true;
            this.lstSubFamilias.Location = new System.Drawing.Point(415, 33);
            this.lstSubFamilias.Name = "lstSubFamilias";
            this.lstSubFamilias.Size = new System.Drawing.Size(200, 327);
            this.lstSubFamilias.TabIndex = 2;
            //
            // txtNombreFamilia
            //
            this.txtNombreFamilia.Location = new System.Drawing.Point(630, 50);
            this.txtNombreFamilia.Name = "txtNombreFamilia";
            this.txtNombreFamilia.Size = new System.Drawing.Size(180, 22);
            this.txtNombreFamilia.TabIndex = 3;
            //
            // lblNombreFamilia
            //
            this.lblNombreFamilia.AutoSize = true;
            this.lblNombreFamilia.Location = new System.Drawing.Point(627, 30);
            this.lblNombreFamilia.Name = "lblNombreFamilia";
            this.lblNombreFamilia.Size = new System.Drawing.Size(61, 16);
            this.lblNombreFamilia.TabIndex = 4;
            this.lblNombreFamilia.Text = "Nombre:";
            //
            // btnCrear
            //
            this.btnCrear.Location = new System.Drawing.Point(630, 90);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.Size = new System.Drawing.Size(100, 30);
            this.btnCrear.TabIndex = 5;
            this.btnCrear.Text = "Crear";
            this.btnCrear.UseVisualStyleBackColor = true;
            this.btnCrear.Click += new System.EventHandler(this.btnCrear_Click);
            //
            // btnActualizar
            //
            this.btnActualizar.Location = new System.Drawing.Point(630, 130);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(100, 30);
            this.btnActualizar.TabIndex = 6;
            this.btnActualizar.Text = "Actualizar";
            this.btnActualizar.UseVisualStyleBackColor = true;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            //
            // btnEliminar
            //
            this.btnEliminar.Location = new System.Drawing.Point(630, 170);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(100, 30);
            this.btnEliminar.TabIndex = 7;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            //
            // lblFamilias
            //
            this.lblFamilias.AutoSize = true;
            this.lblFamilias.Location = new System.Drawing.Point(12, 14);
            this.lblFamilias.Name = "lblFamilias";
            this.lblFamilias.Size = new System.Drawing.Size(61, 16);
            this.lblFamilias.TabIndex = 8;
            this.lblFamilias.Text = "Familias";
            //
            // lblPatentes
            //
            this.lblPatentes.AutoSize = true;
            this.lblPatentes.Location = new System.Drawing.Point(205, 14);
            this.lblPatentes.Name = "lblPatentes";
            this.lblPatentes.Size = new System.Drawing.Size(61, 16);
            this.lblPatentes.TabIndex = 9;
            this.lblPatentes.Text = "Patentes";
            //
            // lblSubFamilias
            //
            this.lblSubFamilias.AutoSize = true;
            this.lblSubFamilias.Location = new System.Drawing.Point(415, 14);
            this.lblSubFamilias.Name = "lblSubFamilias";
            this.lblSubFamilias.Size = new System.Drawing.Size(80, 16);
            this.lblSubFamilias.TabIndex = 10;
            this.lblSubFamilias.Text = "Sub-Familias";
            //
            // FrmGestionFamilias
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 380);
            this.Controls.Add(this.lblSubFamilias);
            this.Controls.Add(this.lblPatentes);
            this.Controls.Add(this.lblFamilias);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnActualizar);
            this.Controls.Add(this.btnCrear);
            this.Controls.Add(this.lblNombreFamilia);
            this.Controls.Add(this.txtNombreFamilia);
            this.Controls.Add(this.lstSubFamilias);
            this.Controls.Add(this.lstPatentes);
            this.Controls.Add(this.lstFamilias);
            this.Name = "FrmGestionFamilias";
            this.Text = "Gestión de Familias";
            this.Load += new System.EventHandler(this.FrmGestionFamilias_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstFamilias;
        private System.Windows.Forms.CheckedListBox lstPatentes;
        private System.Windows.Forms.CheckedListBox lstSubFamilias;
        private System.Windows.Forms.TextBox txtNombreFamilia;
        private System.Windows.Forms.Label lblNombreFamilia;
        private System.Windows.Forms.Button btnCrear;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Label lblFamilias;
        private System.Windows.Forms.Label lblPatentes;
        private System.Windows.Forms.Label lblSubFamilias;
    }
}
