namespace StoreManager
{
    partial class frmmain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmmain));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.logOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inventoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restockItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAdminAdjustItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLowInStock = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAudit = new System.Windows.Forms.ToolStripMenuItem();
            this.salesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSellItem = new System.Windows.Forms.ToolStripMenuItem();
            this.seachToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSearchSales = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPurchaseRrcords = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSoldItems = new System.Windows.Forms.ToolStripMenuItem();
            this.newUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.inventoryToolStripMenuItem,
            this.salesToolStripMenuItem,
            this.seachToolStripMenuItem,
            this.newUserToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 40);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuExit,
            this.logOutToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(64, 36);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // menuExit
            // 
            this.menuExit.Name = "menuExit";
            this.menuExit.Size = new System.Drawing.Size(178, 36);
            this.menuExit.Text = "Exit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // logOutToolStripMenuItem
            // 
            this.logOutToolStripMenuItem.Name = "logOutToolStripMenuItem";
            this.logOutToolStripMenuItem.Size = new System.Drawing.Size(178, 36);
            this.logOutToolStripMenuItem.Text = "Log out";
            this.logOutToolStripMenuItem.Click += new System.EventHandler(this.logOutToolStripMenuItem_Click);
            // 
            // inventoryToolStripMenuItem
            // 
            this.inventoryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAddItem,
            this.restockItemsToolStripMenuItem,
            this.menuAdminAdjustItem,
            this.menuLowInStock,
            this.menuAudit});
            this.inventoryToolStripMenuItem.Name = "inventoryToolStripMenuItem";
            this.inventoryToolStripMenuItem.Size = new System.Drawing.Size(128, 36);
            this.inventoryToolStripMenuItem.Text = "&Inventory";
            // 
            // menuAddItem
            // 
            this.menuAddItem.Name = "menuAddItem";
            this.menuAddItem.Size = new System.Drawing.Size(428, 36);
            this.menuAddItem.Text = "Register Items";
            this.menuAddItem.Click += new System.EventHandler(this.menuAddItem_Click);
            // 
            // restockItemsToolStripMenuItem
            // 
            this.restockItemsToolStripMenuItem.Name = "restockItemsToolStripMenuItem";
            this.restockItemsToolStripMenuItem.Size = new System.Drawing.Size(428, 36);
            this.restockItemsToolStripMenuItem.Text = "Restock Items";
            this.restockItemsToolStripMenuItem.Click += new System.EventHandler(this.restockItemsToolStripMenuItem_Click);
            // 
            // menuAdminAdjustItem
            // 
            this.menuAdminAdjustItem.Name = "menuAdminAdjustItem";
            this.menuAdminAdjustItem.Size = new System.Drawing.Size(428, 36);
            this.menuAdminAdjustItem.Text = "Authorize Quantity Adjustment";
            this.menuAdminAdjustItem.Click += new System.EventHandler(this.menuAdminAdjustItem_Click);
            // 
            // menuLowInStock
            // 
            this.menuLowInStock.Name = "menuLowInStock";
            this.menuLowInStock.Size = new System.Drawing.Size(428, 36);
            this.menuLowInStock.Text = "Low In Stock";
            this.menuLowInStock.Click += new System.EventHandler(this.menuLowInStock_Click);
            // 
            // menuAudit
            // 
            this.menuAudit.Name = "menuAudit";
            this.menuAudit.Size = new System.Drawing.Size(428, 36);
            this.menuAudit.Text = "Audit Trail";
            this.menuAudit.Click += new System.EventHandler(this.menuAudit_Click);
            // 
            // salesToolStripMenuItem
            // 
            this.salesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSellItem});
            this.salesToolStripMenuItem.Name = "salesToolStripMenuItem";
            this.salesToolStripMenuItem.Size = new System.Drawing.Size(81, 36);
            this.salesToolStripMenuItem.Text = "&Sales";
            // 
            // menuSellItem
            // 
            this.menuSellItem.Name = "menuSellItem";
            this.menuSellItem.Size = new System.Drawing.Size(189, 36);
            this.menuSellItem.Text = "Sell Item";
            this.menuSellItem.Click += new System.EventHandler(this.menuSellItem_Click);
            // 
            // seachToolStripMenuItem
            // 
            this.seachToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSearchSales,
            this.menuPurchaseRrcords,
            this.menuSoldItems});
            this.seachToolStripMenuItem.Name = "seachToolStripMenuItem";
            this.seachToolStripMenuItem.Size = new System.Drawing.Size(98, 36);
            this.seachToolStripMenuItem.Text = "Search";
            // 
            // menuSearchSales
            // 
            this.menuSearchSales.Name = "menuSearchSales";
            this.menuSearchSales.Size = new System.Drawing.Size(348, 36);
            this.menuSearchSales.Text = "Sales Summary Records";
            this.menuSearchSales.Click += new System.EventHandler(this.menuSearchSales_Click);
            // 
            // menuPurchaseRrcords
            // 
            this.menuPurchaseRrcords.Name = "menuPurchaseRrcords";
            this.menuPurchaseRrcords.Size = new System.Drawing.Size(348, 36);
            this.menuPurchaseRrcords.Text = "Purchase Records";
            this.menuPurchaseRrcords.Click += new System.EventHandler(this.menuPurchaseRrcords_Click);
            // 
            // menuSoldItems
            // 
            this.menuSoldItems.Name = "menuSoldItems";
            this.menuSoldItems.Size = new System.Drawing.Size(348, 36);
            this.menuSoldItems.Text = "Sold Items Records";
            this.menuSoldItems.Click += new System.EventHandler(this.menuSoldItems_Click);
            // 
            // newUserToolStripMenuItem
            // 
            this.newUserToolStripMenuItem.Name = "newUserToolStripMenuItem";
            this.newUserToolStripMenuItem.Size = new System.Drawing.Size(129, 36);
            this.newUserToolStripMenuItem.Text = "New User";
            this.newUserToolStripMenuItem.Click += new System.EventHandler(this.newUserToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(77, 36);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(216, 36);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // frmmain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "frmmain";
            this.Text = "Store Manager";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmmain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.ToolStripMenuItem inventoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuAddItem;
        private System.Windows.Forms.ToolStripMenuItem restockItemsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuSellItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuAdminAdjustItem;
        private System.Windows.Forms.ToolStripMenuItem menuLowInStock;
        private System.Windows.Forms.ToolStripMenuItem seachToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuSearchSales;
        private System.Windows.Forms.ToolStripMenuItem menuAudit;
        private System.Windows.Forms.ToolStripMenuItem menuPurchaseRrcords;
        private System.Windows.Forms.ToolStripMenuItem menuSoldItems;
        private System.Windows.Forms.ToolStripMenuItem logOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newUserToolStripMenuItem;
    }
}