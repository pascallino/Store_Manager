namespace StoreManager
{
    partial class frmQuerySale
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
        private System.Windows.Forms.TextBox txtCashReceived_ReceiptNo;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.DataGridView dgvSummary;
    

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.txtCashReceived_ReceiptNo = new System.Windows.Forms.TextBox();
            this.dgvSummary = new System.Windows.Forms.DataGridView();
            this.labelTotal = new System.Windows.Forms.Label();
            this.lblTotalSales = new System.Windows.Forms.Label();
            this.cmbUsers = new System.Windows.Forms.ComboBox();
            this.lblUser = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSummary)).BeginInit();
            this.SuspendLayout();
            // 
            // dtFrom
            // 
            this.dtFrom.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtFrom.Location = new System.Drawing.Point(168, 20);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(394, 38);
            this.dtFrom.TabIndex = 0;
            // 
            // dtTo
            // 
            this.dtTo.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtTo.Location = new System.Drawing.Point(573, 20);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(393, 38);
            this.dtTo.TabIndex = 1;
            // 
            // txtCashReceived_ReceiptNo
            // 
            this.txtCashReceived_ReceiptNo.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCashReceived_ReceiptNo.Location = new System.Drawing.Point(1141, 24);
            this.txtCashReceived_ReceiptNo.Name = "txtCashReceived_ReceiptNo";
            this.txtCashReceived_ReceiptNo.Size = new System.Drawing.Size(280, 31);
            this.txtCashReceived_ReceiptNo.TabIndex = 2;
            this.txtCashReceived_ReceiptNo.TextChanged += new System.EventHandler(this.txtCashReceived_ReceiptNo_TextChanged);
            // 
            // dgvSummary
            // 
            this.dgvSummary.AllowUserToAddRows = false;
            this.dgvSummary.AllowUserToDeleteRows = false;
            this.dgvSummary.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSummary.Location = new System.Drawing.Point(7, 102);
            this.dgvSummary.Name = "dgvSummary";
            this.dgvSummary.Size = new System.Drawing.Size(1534, 450);
            this.dgvSummary.TabIndex = 4;
            this.dgvSummary.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSummary_CellContentClick);
            // 
            // labelTotal
            // 
            this.labelTotal.AutoSize = true;
            this.labelTotal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.labelTotal.Location = new System.Drawing.Point(610, 566);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(64, 28);
            this.labelTotal.TabIndex = 5;
            this.labelTotal.Text = "Total:";
            // 
            // lblTotalSales
            // 
            this.lblTotalSales.AutoSize = true;
            this.lblTotalSales.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTotalSales.ForeColor = System.Drawing.Color.Green;
            this.lblTotalSales.Location = new System.Drawing.Point(679, 559);
            this.lblTotalSales.Name = "lblTotalSales";
            this.lblTotalSales.Size = new System.Drawing.Size(77, 41);
            this.lblTotalSales.TabIndex = 6;
            this.lblTotalSales.Text = "0.00";
            // 
            // cmbUsers
            // 
            this.cmbUsers.BackColor = System.Drawing.Color.White;
            this.cmbUsers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbUsers.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbUsers.ForeColor = System.Drawing.Color.Black;
            this.cmbUsers.FormattingEnabled = true;
            this.cmbUsers.Location = new System.Drawing.Point(1142, 61);
            this.cmbUsers.Name = "cmbUsers";
            this.cmbUsers.Size = new System.Drawing.Size(280, 31);
            this.cmbUsers.TabIndex = 7;
            this.cmbUsers.SelectedIndexChanged += new System.EventHandler(this.cmbUsers_SelectedIndexChanged);
            // 
            // lblUser
            // 
            this.lblUser.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblUser.Location = new System.Drawing.Point(996, 26);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(150, 23);
            this.lblUser.TabIndex = 0;
            this.lblUser.Text = "Receipt\\Invoice No";
            // 
            // frmQuerySale
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1590, 609);
            this.Controls.Add(this.lblUser);
            this.Controls.Add(this.cmbUsers);
            this.Controls.Add(this.labelTotal);
            this.Controls.Add(this.lblTotalSales);
            this.Controls.Add(this.dtFrom);
            this.Controls.Add(this.dtTo);
            this.Controls.Add(this.txtCashReceived_ReceiptNo);
            this.Controls.Add(this.dgvSummary);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmQuerySale";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sales Summary Query";
            this.Load += new System.EventHandler(this.frmQuerySale_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSummary)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTotal;
        private System.Windows.Forms.Label lblTotalSales;
        private System.Windows.Forms.ComboBox cmbUsers;
        private System.Windows.Forms.Label lblUser;
    }
}