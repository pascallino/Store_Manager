namespace StoreManager
{
    partial class frmViewSale
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
        private System.Windows.Forms.DataGridView dgvItems;
        private void InitializeComponent()
        {
            this.dgvItems = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvItems
            // 
            this.dgvItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvItems.Location = new System.Drawing.Point(10, 10);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.Size = new System.Drawing.Size(760, 430);
            this.dgvItems.TabIndex = 0;
            // 
            // frmViewSale
            // 
            this.ClientSize = new System.Drawing.Size(780, 450);
            this.Controls.Add(this.dgvItems);
            this.Name = "frmViewSale";
            this.Text = "Receipt Item Details";
            this.Load += new System.EventHandler(this.frmViewSale_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.ResumeLayout(false);

        }

    }
}