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
    public partial class frmAdjustQuanity : Form
    {
        public frmAdjustQuanity()
        {
            InitializeComponent();
            dgvA.CellClick += dgvA_CellClick;
            dgvA.CellContentClick += dgvA_CellClick;
            dgvA.CellMouseEnter += dgvA_CellMouseEnter;
            dgvA.CellMouseLeave += dgvA_CellMouseLeave;
    
        }

        private void ResetButtonColors()
        {
            if (!dgvA.Columns.Contains("Edit") || !dgvA.Columns.Contains("Delete"))
                return;

            foreach (DataGridViewRow row in dgvA.Rows)
            {
                if (!row.IsNewRow)
                {
                    row.Cells["Select"].Style.BackColor = Color.CadetBlue;
                }
            }
        }
        private void StyleGrid()
        {
            // Remove ugly borders
            dgvA.BorderStyle = BorderStyle.None;
            dgvA.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvA.RowHeadersVisible = false;

            // Header style
            dgvA.EnableHeadersVisualStyles = false;
            dgvA.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 144, 255);
            dgvA.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvA.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvA.ColumnHeadersHeight = 35;

            // Row style
            dgvA.DefaultCellStyle.BackColor = Color.White;
            dgvA.DefaultCellStyle.ForeColor = Color.Black;
            dgvA.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvA.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 240, 255);
            dgvA.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Alternate row color
            dgvA.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);

            // Auto-size
            dgvA.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvA.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvA.AllowUserToResizeRows = false;

            // Full row select
            dgvA.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Now safe to reset button colors
            ResetButtonColors();


        }
        private void AddButtonsToGrid()
        {
            // Prevent duplicate buttons
            if (!dgvA.Columns.Contains("Select"))
            {
                DataGridViewButtonColumn SelectBtn = new DataGridViewButtonColumn();
                SelectBtn.Name = "Select";

                SelectBtn.Text = "Select";
                SelectBtn.UseColumnTextForButtonValue = true;
                SelectBtn.Width = 40;
                dgvA.Columns.Add(SelectBtn);
                SelectBtn.FlatStyle = FlatStyle.Flat;   // <-- REQUIRED

                // Style (Green Button)
                dgvA.Columns["Select"].DefaultCellStyle.BackColor = Color.LavenderBlush;
                dgvA.Columns["Select"].DefaultCellStyle.ForeColor = Color.Black;
                dgvA.Columns["Select"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
        }
        private DataTable dtItems;
        private void LoadItems()
        {
            using (SqlConnection con = DB.GetCon())
            {
                con.Open();
                string query = "SELECT ItemID, Barcode, ItemName, Total_Quantity FROM Items order by ItemName asc";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                dtItems = new DataTable();
                da.Fill(dtItems);
                // Reset any previous filter
                dtItems.DefaultView.RowFilter = "";
                dgvA.DataSource = dtItems;
                // Show total records
                lblPageInfo.Text = $"Total Products: {dtItems.Rows.Count + 1}";
            }
        }

        private void LoadItemDetails()
        {
            if (string.IsNullOrWhiteSpace(txtItemID.Text))
                return;

            try
            {
                using (SqlConnection con = DB.GetCon())
                {
                    con.Open();

                    string query = @"SELECT * FROM Items WHERE ItemID = @itemid";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@itemid", txtItemID.Text);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        
                        txtItemName.Text = dr["ItemName"].ToString();
                        txtTotalQty.Text = dr["Total_Quantity"].ToString();
                       
                    }

                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ClearItemFields()
        {
            // Clear string fields
            txtTotalQty.Text = "";
            txtItemName.Text = "";
            txtItemID.Text = "";
            txtTotalQty.Focus();
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validate ItemID
            if (string.IsNullOrWhiteSpace(txtItemID.Text))
            {
                MessageBox.Show("Please select an item.", "Error");
                return;
            }

            // Validate numeric quantity
            if (!int.TryParse(txtTotalQty.Text, out int newQty))
            {
                MessageBox.Show("Invalid quantity entered.", "Error");
                return;
            }

            // Prevent negative values
            if (newQty < 0)
            {
                MessageBox.Show("Quantity cannot be negative.", "Error");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtItemID.Text))
            {
                MessageBox.Show("Please select an item.", "Error");
                return;
            }

            int itemID = int.Parse(txtItemID.Text);

            using (SqlConnection con = DB.GetCon())
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    // 1️⃣ Get current stock
                    int currentQty = 0;
                    SqlCommand cmdGet = new SqlCommand(
                        "SELECT Total_Quantity FROM Items WHERE ItemID=@id",
                        con, tran);
                    cmdGet.Parameters.AddWithValue("@id", itemID);
                    currentQty = Convert.ToInt32(cmdGet.ExecuteScalar());

                    // 2️⃣ Calculate difference
                    int diff = newQty - currentQty;

                    int cr = 0;
                    int dr = 0;

                    if (diff > 0)
                        cr = diff;     // Increase
                    else if (diff < 0)
                        dr = Math.Abs(diff); // Reduction

                    // 3️⃣ Update Items stock
                    SqlCommand cmdUpdate = new SqlCommand(@"
                UPDATE Items 
                SET Total_Quantity = @qty 
                WHERE ItemID = @id",
                        con, tran);

                    cmdUpdate.Parameters.AddWithValue("@qty", newQty);
                    cmdUpdate.Parameters.AddWithValue("@id", itemID);
                    cmdUpdate.ExecuteNonQuery();

                    // 4️⃣ Log into PurchasedItems
                    SqlCommand cmdLog = new SqlCommand(@"
                INSERT INTO PurchasedItems
                (ItemID, Cr, Dr, Purchase_Status)
                VALUES
                (@itemID, @cr, @dr, 'Adjustment')
            ", con, tran);

                    cmdLog.Parameters.AddWithValue("@itemID", itemID);
                    cmdLog.Parameters.AddWithValue("@cr", cr);
                    cmdLog.Parameters.AddWithValue("@dr", dr);
                    cmdLog.ExecuteNonQuery();

                    tran.Commit();

                    MessageBox.Show("Stock adjusted successfully.", "Success");
                    ClearItemFields();
                    LoadItems();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show("Adjustment failed: " + ex.Message);
                }
            }
        }



        private void frmAdjustQuanity_Load(object sender, EventArgs e)
        {
            LoadItems();
            // LoadItemsPage();
            StyleGrid();
            AddButtonsToGrid();
        }
        private void dgvA_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string col = dgvA.Columns[e.ColumnIndex].Name;

                ResetButtonColors();
                if (col == "Select")
                    dgvA.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.CornflowerBlue;

            }
        }

        private void dgvA_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string col = dgvA.Columns[e.ColumnIndex].Name;

                ResetButtonColors();
                if (col == "Select")
                    dgvA.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Ivory;
            }
        }

        private void dgvA_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string col = dgvA.Columns[e.ColumnIndex].Name;

            // Get item ID for actions
            string itemID = dgvA.Rows[e.RowIndex].Cells["ItemID"].Value.ToString();

            if (col == "Select")
            {
                txtItemID.Text = itemID;
                LoadItemDetails();   // Your function to load item by ID
            }
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (dtItems == null) return;

            string filter = txtSearch.Text.Replace("'", "''"); // Escape single quotes
            (dgvA.DataSource as DataTable).DefaultView.RowFilter =
                $"ItemName LIKE '%{filter}%' OR Barcode LIKE '%{filter}%'";
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgvA_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearItemFields();
        }
    }
}
