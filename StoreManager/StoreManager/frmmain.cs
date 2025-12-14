using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StoreManager
{
    public partial class frmmain : Form
    {
        public frmmain()
        {
            InitializeComponent();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void frmmain_Load(object sender, EventArgs e)
        {
          
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to exit the application?",
                "Exit",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.No)
            {
                e.Cancel = true;   // Cancel closing
            }
            else
            {
                Application.Exit();
            }
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuAddItem_Click(object sender, EventArgs e)
        {
            frmItems item = new frmItems();
            item.ShowDialog();

        }

        private void restockItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmRestockItems restockitem = new frmRestockItems();
            restockitem.ShowDialog();
        }

        private void menuAdminAdjustItem_Click(object sender, EventArgs e)
        {
            frmAuthorize authorize = new frmAuthorize("adjust");
            authorize.ShowDialog();
        }

        private void menuSellItem_Click(object sender, EventArgs e)
        {
            frmSales sale = new frmSales();
            sale.Show();
               
        }

        private void menuLowInStock_Click(object sender, EventArgs e)
        {
            frmLowInStock low = new frmLowInStock();
            low.ShowDialog();
        }

        private void menuSearchSales_Click(object sender, EventArgs e)
        {
            frmQuerySale q = new frmQuerySale();
            q.ShowDialog();
        }

        private void menuAudit_Click(object sender, EventArgs e)
        {
            frmAuthorize a = new frmAuthorize("Audit");
            a.ShowDialog();
        }

        private void menuPurchaseRrcords_Click(object sender, EventArgs e)
        {
            frmQueryPurchase p = new frmQueryPurchase();
            p.ShowDialog();
        }

        private void menuSoldItems_Click(object sender, EventArgs e)
        {
            frmQuerySalesItems s = new frmQuerySalesItems();
            s.ShowDialog();
        }
    }
}
