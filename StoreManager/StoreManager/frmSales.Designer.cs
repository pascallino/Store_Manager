namespace StoreManager
{
    partial class frmSales
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

        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.DataGridView dgvCart;
        private System.Windows.Forms.Label lblGrandTotal;
        private System.Windows.Forms.Button btnCheckout;
        private System.Windows.Forms.Button btnClearCart;
        private System.Windows.Forms.Label labelTotal;
        private System.Windows.Forms.TextBox txtCashReceived;
        private System.Windows.Forms.Label labelCash;
        private System.Windows.Forms.Label labelBalance;
        private System.Windows.Forms.Label lblBalance;

        private void InitializeComponent()
        {
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.dgvCart = new System.Windows.Forms.DataGridView();
            this.lblGrandTotal = new System.Windows.Forms.Label();
            this.btnCheckout = new System.Windows.Forms.Button();
            this.btnClearCart = new System.Windows.Forms.Button();
            this.labelTotal = new System.Windows.Forms.Label();
            this.txtCashReceived = new System.Windows.Forms.TextBox();
            this.labelCash = new System.Windows.Forms.Label();
            this.labelBalance = new System.Windows.Forms.Label();
            this.lblBalance = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).BeginInit();
            this.SuspendLayout();
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtSearch.Location = new System.Drawing.Point(20, 20);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(450, 34);
            this.txtSearch.TabIndex = 0;
            // 
            // dgvCart
            // 
            this.dgvCart.AllowUserToAddRows = false;
            this.dgvCart.AllowUserToDeleteRows = false;
            this.dgvCart.BackgroundColor = System.Drawing.Color.White;
            this.dgvCart.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCart.Location = new System.Drawing.Point(20, 65);
            this.dgvCart.Name = "dgvCart";
            this.dgvCart.RowTemplate.Height = 28;
            this.dgvCart.Size = new System.Drawing.Size(760, 350);
            this.dgvCart.TabIndex = 1;
            // 
            // lblGrandTotal
            // 
            this.lblGrandTotal.AutoSize = true;
            this.lblGrandTotal.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblGrandTotal.ForeColor = System.Drawing.Color.Green;
            this.lblGrandTotal.Location = new System.Drawing.Point(150, 423);
            this.lblGrandTotal.Name = "lblGrandTotal";
            this.lblGrandTotal.Size = new System.Drawing.Size(77, 41);
            this.lblGrandTotal.TabIndex = 3;
            this.lblGrandTotal.Text = "0.00";
            // 
            // btnCheckout
            // 
            this.btnCheckout.BackColor = System.Drawing.Color.Green;
            this.btnCheckout.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCheckout.ForeColor = System.Drawing.Color.White;
            this.btnCheckout.Location = new System.Drawing.Point(630, 500);
            this.btnCheckout.Name = "btnCheckout";
            this.btnCheckout.Size = new System.Drawing.Size(150, 45);
            this.btnCheckout.TabIndex = 8;
            this.btnCheckout.Text = "CHECKOUT";
            this.btnCheckout.UseVisualStyleBackColor = false;
            // 
            // btnClearCart
            // 
            this.btnClearCart.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnClearCart.Location = new System.Drawing.Point(450, 500);
            this.btnClearCart.Name = "btnClearCart";
            this.btnClearCart.Size = new System.Drawing.Size(150, 45);
            this.btnClearCart.TabIndex = 9;
            this.btnClearCart.Text = "CLEAR CART";
            // 
            // labelTotal
            // 
            this.labelTotal.AutoSize = true;
            this.labelTotal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.labelTotal.Location = new System.Drawing.Point(20, 430);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(127, 28);
            this.labelTotal.TabIndex = 2;
            this.labelTotal.Text = "Grand Total:";
            // 
            // txtCashReceived
            // 
            this.txtCashReceived.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtCashReceived.Location = new System.Drawing.Point(150, 467);
            this.txtCashReceived.Name = "txtCashReceived";
            this.txtCashReceived.Size = new System.Drawing.Size(150, 34);
            this.txtCashReceived.TabIndex = 4;
            // 
            // labelCash
            // 
            this.labelCash.AutoSize = true;
            this.labelCash.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.labelCash.Location = new System.Drawing.Point(20, 470);
            this.labelCash.Name = "labelCash";
            this.labelCash.Size = new System.Drawing.Size(139, 28);
            this.labelCash.TabIndex = 5;
            this.labelCash.Text = "Cash Received:";
            // 
            // labelBalance
            // 
            this.labelBalance.AutoSize = true;
            this.labelBalance.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.labelBalance.Location = new System.Drawing.Point(20, 510);
            this.labelBalance.Name = "labelBalance";
            this.labelBalance.Size = new System.Drawing.Size(82, 28);
            this.labelBalance.TabIndex = 6;
            this.labelBalance.Text = "Balance:";
            // 
            // lblBalance
            // 
            this.lblBalance.AutoSize = true;
            this.lblBalance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblBalance.ForeColor = System.Drawing.Color.Blue;
            this.lblBalance.Location = new System.Drawing.Point(150, 510);
            this.lblBalance.Name = "lblBalance";
            this.lblBalance.Size = new System.Drawing.Size(53, 28);
            this.lblBalance.TabIndex = 7;
            this.lblBalance.Text = "0.00";
            // 
            // frmSales
            // 
            this.ClientSize = new System.Drawing.Size(800, 570);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.dgvCart);
            this.Controls.Add(this.labelTotal);
            this.Controls.Add(this.lblGrandTotal);
            this.Controls.Add(this.txtCashReceived);
            this.Controls.Add(this.labelCash);
            this.Controls.Add(this.labelBalance);
            this.Controls.Add(this.lblBalance);
            this.Controls.Add(this.btnCheckout);
            this.Controls.Add(this.btnClearCart);
            this.Name = "frmSales";
            this.Text = "Sales / POS";
            this.Load += new System.EventHandler(this.frmSales_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    }
}