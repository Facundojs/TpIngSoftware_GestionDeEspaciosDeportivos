namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    partial class FrmBackups
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvBackups;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.TextBox txtBackupName;
        private System.Windows.Forms.Label lblName;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvBackups = new System.Windows.Forms.DataGridView();
            this.btnBackup = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.txtBackupName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBackups)).BeginInit();
            this.SuspendLayout();

            // dgvBackups
            this.dgvBackups.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBackups.Location = new System.Drawing.Point(12, 12);
            this.dgvBackups.Name = "dgvBackups";
            this.dgvBackups.Size = new System.Drawing.Size(500, 300);
            this.dgvBackups.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBackups.MultiSelect = false;

            // lblName
            this.lblName.Location = new System.Drawing.Point(530, 15);
            this.lblName.Text = "Nombre Backup:";
            this.lblName.Size = new System.Drawing.Size(100, 20);

            // txtBackupName
            this.txtBackupName.Location = new System.Drawing.Point(530, 40);
            this.txtBackupName.Size = new System.Drawing.Size(150, 20);

            // btnBackup
            this.btnBackup.Location = new System.Drawing.Point(530, 70);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(150, 30);
            this.btnBackup.Text = "Realizar Backup";
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);

            // btnRestore
            this.btnRestore.Location = new System.Drawing.Point(530, 110);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(150, 30);
            this.btnRestore.Text = "Restore";
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);

            // btnDelete
            this.btnDelete.Location = new System.Drawing.Point(530, 150);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(150, 30);
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            // Form
            this.ClientSize = new System.Drawing.Size(700, 350);
            this.Controls.Add(this.dgvBackups);
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.txtBackupName);
            this.Controls.Add(this.lblName);
            this.Name = "FrmBackups";
            this.Text = "Gestión de Backups";
            this.Load += new System.EventHandler(this.FrmBackups_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBackups)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
