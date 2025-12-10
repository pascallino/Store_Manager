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
        public frmAuthorize()
        {
            InitializeComponent();
        }

       

        private void btnAuthorize_Click(object sender, EventArgs e)
        {
            frmAdjustQuanity adjustQ = new frmAdjustQuanity();
            adjustQ.ShowDialog();
            this.Hide();
        }
    }
}
