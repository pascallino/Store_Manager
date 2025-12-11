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
        private System.Windows.Forms.Button btnDeleteTransaction;

        private void InitializeComponent()
        {
            this.btnDeleteTransaction = new System.Windows.Forms.Button();
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
            this.txtSearch_recipt_invoice = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnUpdateTransaction = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).BeginInit();
            this.SuspendLayout();
            // 
            // btnDeleteTransaction
            // 
            this.btnDeleteTransaction.BackColor = System.Drawing.Color.Red;
            this.btnDeleteTransaction.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnDeleteTransaction.ForeColor = System.Drawing.Color.White;
            this.btnDeleteTransaction.Location = new System.Drawing.Point(920, 581);
            this.btnDeleteTransaction.Name = "btnDeleteTransaction";
            this.btnDeleteTransaction.Size = new System.Drawing.Size(151, 51);
            this.btnDeleteTransaction.TabIndex = 10;
            this.btnDeleteTransaction.Text = "DELETE";
            this.btnDeleteTransaction.UseVisualStyleBackColor = false;
            this.btnDeleteTransaction.Visible = false;
            this.btnDeleteTransaction.Click += new System.EventHandler(this.btnDeleteTransaction_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtSearch.Location = new System.Drawing.Point(20, 20);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(450, 34);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
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
            this.dgvCart.Size = new System.Drawing.Size(1228, 442);
            this.dgvCart.TabIndex = 1;
            this.dgvCart.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCart_CellContentClick);
            // 
            // lblGrandTotal
            // 
            this.lblGrandTotal.AutoSize = true;
            this.lblGrandTotal.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblGrandTotal.ForeColor = System.Drawing.Color.Green;
            this.lblGrandTotal.Location = new System.Drawing.Point(145, 510);
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
            this.btnCheckout.Location = new System.Drawing.Point(608, 587);
            this.btnCheckout.Name = "btnCheckout";
            this.btnCheckout.Size = new System.Drawing.Size(150, 45);
            this.btnCheckout.TabIndex = 8;
            this.btnCheckout.Text = "CHECKOUT";
            this.btnCheckout.UseVisualStyleBackColor = false;
            // 
            // btnClearCart
            // 
            this.btnClearCart.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnClearCart.Location = new System.Drawing.Point(445, 587);
            this.btnClearCart.Name = "btnClearCart";
            this.btnClearCart.Size = new System.Drawing.Size(150, 45);
            this.btnClearCart.TabIndex = 9;
            this.btnClearCart.Text = "CLEAR CART";
            // 
            // labelTotal
            // 
            this.labelTotal.AutoSize = true;
            this.labelTotal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.labelTotal.Location = new System.Drawing.Point(15, 517);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(127, 28);
            this.labelTotal.TabIndex = 2;
            this.labelTotal.Text = "Grand Total:";
            // 
            // txtCashReceived
            // 
            this.txtCashReceived.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtCashReceived.Location = new System.Drawing.Point(145, 554);
            this.txtCashReceived.Name = "txtCashReceived";
            this.txtCashReceived.Size = new System.Drawing.Size(150, 34);
            this.txtCashReceived.TabIndex = 4;
            // 
            // labelCash
            // 
            this.labelCash.AutoSize = true;
            this.labelCash.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.labelCash.Location = new System.Drawing.Point(15, 557);
            this.labelCash.Name = "labelCash";
            this.labelCash.Size = new System.Drawing.Size(139, 28);
            this.labelCash.TabIndex = 5;
            this.labelCash.Text = "Cash Received:";
            // 
            // labelBalance
            // 
            this.labelBalance.AutoSize = true;
            this.labelBalance.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.labelBalance.Location = new System.Drawing.Point(15, 597);
            this.labelBalance.Name = "labelBalance";
            this.labelBalance.Size = new System.Drawing.Size(82, 28);
            this.labelBalance.TabIndex = 6;
            this.labelBalance.Text = "Balance:";
            // 
            // lblBalance
            // 
            this.lblBalance.AutoSize = true;
            this.lblBalance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBalance.ForeColor = System.Drawing.Color.Blue;
            this.lblBalance.Location = new System.Drawing.Point(145, 597);
            this.lblBalance.Name = "lblBalance";
            this.lblBalance.Size = new System.Drawing.Size(77, 41);
            this.lblBalance.TabIndex = 7;
            this.lblBalance.Text = "0.00";
            // 
            // txtSearch_recipt_invoice
            // 
            this.txtSearch_recipt_invoice.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtSearch_recipt_invoice.Location = new System.Drawing.Point(958, 20);
            this.txtSearch_recipt_invoice.Name = "txtSearch_recipt_invoice";
            this.txtSearch_recipt_invoice.Size = new System.Drawing.Size(290, 34);
            this.txtSearch_recipt_invoice.TabIndex = 11;
            this.txtSearch_recipt_invoice.TextChanged += new System.EventHandler(this.txtSearch_recipt_invoice_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(758, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(194, 28);
            this.label1.TabIndex = 12;
            this.label1.Text = "Receipt\\Invoice No";
            // 
            // btnUpdateTransaction
            // 
            this.btnUpdateTransaction.BackColor = System.Drawing.Color.SlateBlue;
            this.btnUpdateTransaction.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnUpdateTransaction.ForeColor = System.Drawing.Color.White;
            this.btnUpdateTransaction.Location = new System.Drawing.Point(1078, 581);
            this.btnUpdateTransaction.Name = "btnUpdateTransaction";
            this.btnUpdateTransaction.Size = new System.Drawing.Size(150, 51);
            this.btnUpdateTransaction.TabIndex = 13;
            this.btnUpdateTransaction.Text = "UPDATE";
            this.btnUpdateTransaction.UseVisualStyleBackColor = false;
            this.btnUpdateTransaction.Visible = false;
            this.btnUpdateTransaction.Click += new System.EventHandler(this.btnUpdateTransaction_Click);
            // 
            // frmSales
            // 
            this.ClientSize = new System.Drawing.Size(1275, 649);
            this.Controls.Add(this.btnUpdateTransaction);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSearch_recipt_invoice);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.dgvCart);
            this.Controls.Add(this.labelTotal);
            this.Controls.Add(this.lblGrandTotal);
            this.Controls.Add(this.txtCashReceived);
            this.Controls.Add(this.labelCash);
            this.Controls.Add(this.labelBalance);
            this.Controls.Add(this.lblBalance);
            this.Controls.Add(this.btnCheckout);
            this.Controls.Add(this.btnDeleteTransaction);
            this.Controls.Add(this.btnClearCart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSales";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sales / POS";
            this.Load += new System.EventHandler(this.frmSales_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.TextBox txtSearch_recipt_invoice;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnUpdateTransaction;
    }
}