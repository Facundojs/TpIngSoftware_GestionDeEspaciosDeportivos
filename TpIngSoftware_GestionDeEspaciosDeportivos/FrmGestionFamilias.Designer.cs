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
            this.tvFamilias = new System.Windows.Forms.TreeView();
            this.lstPatentes = new System.Windows.Forms.CheckedListBox();
            this.lstSubFamilias = new System.Windows.Forms.CheckedListBox();
            this.tvPreview = new System.Windows.Forms.TreeView();
            this.txtNombreFamilia = new System.Windows.Forms.TextBox();
            this.lblNombreFamilia = new System.Windows.Forms.Label();
            this.btnCrear = new System.Windows.Forms.Button();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.lblFamilias = new System.Windows.Forms.Label();
            this.lblPatentes = new System.Windows.Forms.Label();
            this.lblSubFamilias = new System.Windows.Forms.Label();
            this.lblPreview = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // tvFamilias
            //
            this.tvFamilias.Location = new System.Drawing.Point(12, 33);
            this.tvFamilias.Name = "tvFamilias";
            this.tvFamilias.Size = new System.Drawing.Size(190, 265);
            this.tvFamilias.TabIndex = 0;
            this.tvFamilias.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFamilias_AfterSelect);
            //
            // lstPatentes
            //
            this.lstPatentes.FormattingEnabled = true;
            this.lstPatentes.Location = new System.Drawing.Point(207, 33);
            this.lstPatentes.Name = "lstPatentes";
            this.lstPatentes.Size = new System.Drawing.Size(185, 265);
            this.lstPatentes.TabIndex = 1;
            this.lstPatentes.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lstPatentes_ItemCheck);
            //
            // lstSubFamilias
            //
            this.lstSubFamilias.FormattingEnabled = true;
            this.lstSubFamilias.Location = new System.Drawing.Point(397, 33);
            this.lstSubFamilias.Name = "lstSubFamilias";
            this.lstSubFamilias.Size = new System.Drawing.Size(185, 265);
            this.lstSubFamilias.TabIndex = 2;
            this.lstSubFamilias.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lstSubFamilias_ItemCheck);
            //
            // tvPreview
            //
            this.tvPreview.Location = new System.Drawing.Point(587, 33);
            this.tvPreview.Name = "tvPreview";
            this.tvPreview.Size = new System.Drawing.Size(231, 265);
            this.tvPreview.TabIndex = 3;
            this.tvPreview.LabelEdit = false;
            //
            // lblNombreFamilia
            //
            this.lblNombreFamilia.AutoSize = true;
            this.lblNombreFamilia.Location = new System.Drawing.Point(12, 314);
            this.lblNombreFamilia.Name = "lblNombreFamilia";
            this.lblNombreFamilia.Size = new System.Drawing.Size(61, 16);
            this.lblNombreFamilia.TabIndex = 4;
            this.lblNombreFamilia.Text = "Nombre:";
            //
            // txtNombreFamilia
            //
            this.txtNombreFamilia.Location = new System.Drawing.Point(75, 311);
            this.txtNombreFamilia.Name = "txtNombreFamilia";
            this.txtNombreFamilia.Size = new System.Drawing.Size(160, 22);
            this.txtNombreFamilia.TabIndex = 5;
            //
            // btnCrear
            //
            this.btnCrear.Location = new System.Drawing.Point(245, 309);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.Size = new System.Drawing.Size(85, 28);
            this.btnCrear.TabIndex = 6;
            this.btnCrear.Text = "Crear";
            this.btnCrear.UseVisualStyleBackColor = true;
            this.btnCrear.Click += new System.EventHandler(this.btnCrear_Click);
            //
            // btnActualizar
            //
            this.btnActualizar.Location = new System.Drawing.Point(338, 309);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(85, 28);
            this.btnActualizar.TabIndex = 7;
            this.btnActualizar.Text = "Actualizar";
            this.btnActualizar.UseVisualStyleBackColor = true;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            //
            // btnEliminar
            //
            this.btnEliminar.Location = new System.Drawing.Point(431, 309);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(85, 28);
            this.btnEliminar.TabIndex = 8;
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
            this.lblFamilias.TabIndex = 9;
            this.lblFamilias.Text = "Familias";
            //
            // lblPatentes
            //
            this.lblPatentes.AutoSize = true;
            this.lblPatentes.Location = new System.Drawing.Point(207, 14);
            this.lblPatentes.Name = "lblPatentes";
            this.lblPatentes.Size = new System.Drawing.Size(61, 16);
            this.lblPatentes.TabIndex = 10;
            this.lblPatentes.Text = "Patentes";
            //
            // lblSubFamilias
            //
            this.lblSubFamilias.AutoSize = true;
            this.lblSubFamilias.Location = new System.Drawing.Point(397, 14);
            this.lblSubFamilias.Name = "lblSubFamilias";
            this.lblSubFamilias.Size = new System.Drawing.Size(80, 16);
            this.lblSubFamilias.TabIndex = 11;
            this.lblSubFamilias.Text = "Sub-Familias";
            //
            // lblPreview
            //
            this.lblPreview.AutoSize = true;
            this.lblPreview.Location = new System.Drawing.Point(587, 14);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(70, 16);
            this.lblPreview.TabIndex = 12;
            this.lblPreview.Text = "Vista Previa";
            //
            // FrmGestionFamilias
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 350);
            this.Controls.Add(this.lblPreview);
            this.Controls.Add(this.lblSubFamilias);
            this.Controls.Add(this.lblPatentes);
            this.Controls.Add(this.lblFamilias);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnActualizar);
            this.Controls.Add(this.btnCrear);
            this.Controls.Add(this.txtNombreFamilia);
            this.Controls.Add(this.lblNombreFamilia);
            this.Controls.Add(this.tvPreview);
            this.Controls.Add(this.lstSubFamilias);
            this.Controls.Add(this.lstPatentes);
            this.Controls.Add(this.tvFamilias);
            this.Name = "FrmGestionFamilias";
            this.Text = "Gestión de Familias";
            this.Load += new System.EventHandler(this.FrmGestionFamilias_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvFamilias;
        private System.Windows.Forms.CheckedListBox lstPatentes;
        private System.Windows.Forms.CheckedListBox lstSubFamilias;
        private System.Windows.Forms.TreeView tvPreview;
        private System.Windows.Forms.TextBox txtNombreFamilia;
        private System.Windows.Forms.Label lblNombreFamilia;
        private System.Windows.Forms.Button btnCrear;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Label lblFamilias;
        private System.Windows.Forms.Label lblPatentes;
        private System.Windows.Forms.Label lblSubFamilias;
        private System.Windows.Forms.Label lblPreview;
    }
}
