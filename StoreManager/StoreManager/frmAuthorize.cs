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
    public partial class frmAuthorize : Form
    {
        string checkform;

        public frmAuthorize(string value)
        {

            InitializeComponent();
            checkform = value;
        }

       

        private void btnAuthorize_Click(object sender, EventArgs e)
        {

            string password = txtpassword.Text;
            if (password == "admin")
            {
                if (checkform == "Audit")
                {
                    frmQueryDeletedSale q = new frmQueryDeletedSale();
                    q.ShowDialog();
                    this.Hide();
                }
                else
                {
                    frmAdjustQuanity adjustQ = new frmAdjustQuanity();
                    adjustQ.ShowDialog();
                    this.Hide();
                }
            }
            else
            {
                DialogResult dr = MessageBox.Show(
                   "Wrong Password, Please try again?",
                   "Confirm Password", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            }
        }
   
        private void frmAuthorize_Load(object sender, EventArgs e)
        {

        }
    }
}
