using StoreManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;


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
            if (string.IsNullOrWhiteSpace(txtpassword.Text))
            {
                MessageBox.Show(
                    "Please enter admin password",
                    "Authorization",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            int adminUserId = 0;

            using (SqlConnection con = DB.GetCon())
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT TOP 1 UserID 
          FROM Users 
          WHERE UserType = 'Admin' AND Password = @P", con))
            {
                cmd.Parameters.AddWithValue("@P", txtpassword.Text.Trim());

                con.Open();
                object result = cmd.ExecuteScalar();

                if (result == null)
                {
                    MessageBox.Show(
                        "Wrong admin password.\nAccess denied.",
                        "Authorization Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                adminUserId = Convert.ToInt32(result);
            }

            // ✅ AUTHORIZED ADMIN
            if (checkform == "Audit")
            {
                frmQueryDeletedSale q = new frmQueryDeletedSale();
                q.ShowDialog();
            }
            else if (checkform == "user")
            {
                frmUser u = new frmUser(adminUserId);
                u.ShowDialog();
            }
            else
            {
                frmAdjustQuanity adjustQ = new frmAdjustQuanity();
                adjustQ.ShowDialog();
            }

            this.Close(); // close authorization form
        }


        private void frmAuthorize_Load(object sender, EventArgs e)
        {
            this.AcceptButton = this.btnAuthorize;
        }
    }
}
