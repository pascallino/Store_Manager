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
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter username and password",
                    "Login",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

           // string hashedPassword = Security.HashPassword(txtPassword.Text);

            using (SqlConnection con = DB.GetCon()) // or your GetCon()
            using (SqlCommand cmd = new SqlCommand(
                "SELECT UserID, Firstname, Lastname, Username " +
                "FROM Users WHERE Username=@U AND Password=@P", con))
            {
                cmd.Parameters.AddWithValue("@U", txtUsername.Text.Trim());
                cmd.Parameters.AddWithValue("@P", txtPassword.Text.Trim());

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    string fullName = dr["Firstname"] + " " + dr["Lastname"];
                    int Id = int.Parse(dr["UserID"].ToString());

                    MessageBox.Show($"Welcome {fullName}!",
                        "Login Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    frmmain main = new frmmain(fullName, Id);
                    main.FormClosed += (s, args) => Application.Exit();
                    main.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid username or password",
                        "Login Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
