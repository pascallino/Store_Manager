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
    public partial class frmUser : Form
    {
        private int selectedUserId = 0;

        private int _currentId;
        public frmUser(int id)
        {
            InitializeComponent();
            _currentId = id;
            StyleGrid();
        }

        private void frmUser_Load(object sender, EventArgs e)
        {
            LoadUsers();
        }
        private void StyleGrid()
        {
            dgvUsers.BorderStyle = BorderStyle.None;
            dgvUsers.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvUsers.RowHeadersVisible = false;

            dgvUsers.EnableHeadersVisualStyles = false;
            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            dgvUsers.ColumnHeadersHeight = 40;

            dgvUsers.DefaultCellStyle.Font = new Font("Segoe UI", 18);
            dgvUsers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);

            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgvUsers.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
        

        private void LoadUsers()
        {
            using (SqlConnection con = DB.GetCon())
            using (SqlDataAdapter da = new SqlDataAdapter(
                "SELECT TOP 1000 UserID, Firstname, Lastname, Username, Password, UserType FROM Users ORDER BY UserID DESC", con))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvUsers.DataSource = dt;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm(isUpdate: false)) return;
            string Usertype = "";

            if (chkadmin.Checked == true)
            {
                Usertype = "Admin";
            }
            else
            {
                Usertype = "User";
            }
            using (SqlConnection con = DB.GetCon())
            using (SqlCommand cmd = new SqlCommand(
                "INSERT INTO Users (Firstname, Lastname, Username, Password, UserType) VALUES (@F, @L, @U, @P, @j)", con))
            {
                cmd.Parameters.AddWithValue("@F", txtFirstname.Text.Trim());
                cmd.Parameters.AddWithValue("@L", txtLastname.Text.Trim());
                cmd.Parameters.AddWithValue("@U", txtUsername.Text.Trim());
                cmd.Parameters.AddWithValue("@P", txtPassword.Text); // hash later
                cmd.Parameters.AddWithValue("@j", Usertype);  
                con.Open();
                cmd.ExecuteNonQuery();
            }
            LoadUsers();
            ClearForm();
        }
    

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        if (selectedUserId == 0) return;
        if (!ValidateForm(isUpdate: false)) return;

            string Usertype = "";

            if (chkadmin.Checked == true)
            {
                Usertype = "Admin";
            }
            else
            {
                Usertype = "User";
            }
            using (SqlConnection con = DB.GetCon())
        using (SqlCommand cmd = new SqlCommand(
            "UPDATE Users SET Firstname=@F, Lastname=@L, Username=@U" +
            (string.IsNullOrEmpty(txtPassword.Text) ? "" : ", Password=@P, UserType=@T") +
            " WHERE UserID=@ID", con))
        {
            cmd.Parameters.AddWithValue("@F", txtFirstname.Text.Trim());
            cmd.Parameters.AddWithValue("@L", txtLastname.Text.Trim());
            cmd.Parameters.AddWithValue("@U", txtUsername.Text.Trim());
            if (!string.IsNullOrEmpty(txtPassword.Text))
                cmd.Parameters.AddWithValue("@P", txtPassword.Text);
            cmd.Parameters.AddWithValue("@T", Usertype);
            cmd.Parameters.AddWithValue("@ID", selectedUserId);
            con.Open();
            cmd.ExecuteNonQuery();
        }
        LoadUsers();
        ClearForm();
    }


private void btnDelete_Click(object sender, EventArgs e)
{
    if (selectedUserId == 0) return;

    if (MessageBox.Show("Delete this user?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
    {
        using (SqlConnection con = DB.GetCon())
        using (SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE UserID=@ID", con))
        {
            cmd.Parameters.AddWithValue("@ID", selectedUserId);
            con.Open();
            cmd.ExecuteNonQuery();
                    if (_currentId == selectedUserId)
                    {
                        this.Hide();
                        frmLogin l = new frmLogin();
                        l.ShowDialog();
                    }
        }
        LoadUsers();
        ClearForm();
    }
}

private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
{
            string usert;
    if (e.RowIndex < 0) return;

    selectedUserId = Convert.ToInt32(dgvUsers.Rows[e.RowIndex].Cells["UserID"].Value);
    txtFirstname.Text = dgvUsers.Rows[e.RowIndex].Cells["Firstname"].Value.ToString();
    txtLastname.Text = dgvUsers.Rows[e.RowIndex].Cells["Lastname"].Value.ToString();
    txtUsername.Text = dgvUsers.Rows[e.RowIndex].Cells["Username"].Value.ToString();
            txtPassword.Text = dgvUsers.Rows[e.RowIndex].Cells["Password"].Value.ToString();
            usert = dgvUsers.Rows[e.RowIndex].Cells["UserType"].Value.ToString();
            if (usert.Trim() == "Admin")
            {
                chkadmin.Checked = true;
            }
            else
            {
                chkadmin.Checked = false;
            }
            btnDelete.Enabled = true;
            btnUpdate.Enabled = true;
            btnSave.Enabled = false;
}

private void btnClear_Click(object sender, EventArgs e)
{
    ClearForm();
}

private void ClearForm()
{
    txtFirstname.Clear();
    txtLastname.Clear();
    txtUsername.Clear();
            btnSave.Enabled = true;
            txtPassword.Clear();
            btnDelete.Enabled = false;
            btnUpdate.Enabled = false;
            txtPassword.Clear();
            chkadmin.Checked = false;
    selectedUserId = 0;
}

private bool ValidateForm(bool isUpdate)
{
    if (string.IsNullOrWhiteSpace(txtFirstname.Text))
    {
        MessageBox.Show("Firstname is required", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        txtFirstname.Focus();
        return false;
    }
    if (string.IsNullOrWhiteSpace(txtLastname.Text))
    {
        MessageBox.Show("Lastname is required", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        txtLastname.Focus();
        return false;
    }
    if (string.IsNullOrWhiteSpace(txtUsername.Text))
    {
        MessageBox.Show("Username is required", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        txtUsername.Focus();
        return false;
    }
    if (!isUpdate && string.IsNullOrWhiteSpace(txtPassword.Text))
    {
        MessageBox.Show("Password is required", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        txtPassword.Focus();
        return false;
    }
    return true;
}
    }
}

