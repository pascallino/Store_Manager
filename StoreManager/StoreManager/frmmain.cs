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
        private string _currentUser;
        private int _currentId;
        public frmmain(string u, int id)
        {
       
            InitializeComponent();
            this.Resize += frmmain_Resize;
            _currentUser = u;
            _currentId = id;

            ToolStrip toolStrip = new ToolStrip();
            ToolStripLabel lblUser = new ToolStripLabel();

            lblUser.Font = new Font("Segoe UI", 17, FontStyle.Bold);

            toolStrip.Dock = DockStyle.Top;
            lblUser.Font = new Font("Segoe UI", 17, FontStyle.Bold);
            lblUser.Text = $"Logged in as {_currentUser}";
            lblUser.Alignment = ToolStripItemAlignment.Right;

            toolStrip.Items.Add(lblUser);

            this.Controls.Add(toolStrip);


        }
        private void frmmain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
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
            frmItems item = new frmItems(_currentId);
            item.ShowDialog();

        }

        private void restockItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmRestockItems restockitem = new frmRestockItems(_currentId);
            restockitem.ShowDialog();
        }

        private void menuAdminAdjustItem_Click(object sender, EventArgs e)
        {
            frmAuthorize authorize = new frmAuthorize("adjust");
            authorize.ShowDialog();
        }

        private void menuSellItem_Click(object sender, EventArgs e)
        {
            frmSales sale = new frmSales(_currentId);
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

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Close ALL child forms (MDI children, dialogs, etc.)
            foreach (Form frm in Application.OpenForms.Cast<Form>().ToList())
            {
                if (!(frm is frmmain))
                    frm.Hide();
            }

            // Hide main form
            this.Hide();

            // Show login
            frmLogin login = new frmLogin();
            login.Show();
        }




        private void newUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAuthorize a = new frmAuthorize("user");
            a.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmAbout about = new frmAbout())
            {
                about.ShowDialog(this);
            }
        }
    }
}
