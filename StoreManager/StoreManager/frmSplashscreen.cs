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
    public partial class frmSplashscreen : Form
    {
        public frmSplashscreen()
        {
            InitializeComponent();
     
        }

        private void frmSplashscreen_Load(object sender, EventArgs e)
        {
            // Anything you want to run on load
        }


        private void timer1_Tick(object sender, EventArgs e)
        {

            progressBar1.Value += 80;
            frmmain main = new frmmain();

            if (progressBar1.Value  > 100)
            {
                this.Hide();
                main.FormClosed += (s, args) => Application.Exit();
                main.Show();
                timer1.Stop();
                


            }

        }
    }
}
