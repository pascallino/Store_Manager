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
    public partial class frmRestockItems : Form
    {
        private int _currentId;
        public frmRestockItems(int Id)
        {
            InitializeComponent();
            dgvRItems.CellClick += dgvRItems_CellClick;
            dgvRItems.CellMouseEnter += dgvRItems_CellMouseEnter;
            dgvRItems.CellMouseLeave += dgvRItems_CellMouseLeave;
            txtCartonQty.LostFocus += txtCartonQty_LostFocus;
            txtQty.LostFocus += txtQty_LostFocus;
            _currentId = Id;
        }
        private void ResetButtonColors()
        {
            if (!dgvRItems.Columns.Contains("Edit") || !dgvRItems.Columns.Contains("Delete"))
                return;

            foreach (DataGridViewRow row in dgvRItems.Rows)
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
            dgvRItems.BorderStyle = BorderStyle.None;
            dgvRItems.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvRItems.RowHeadersVisible = false;

            // Header style
            dgvRItems.EnableHeadersVisualStyles = false;
            dgvRItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 144, 255);
            dgvRItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvRItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvRItems.ColumnHeadersHeight = 35;

            // Row style
            dgvRItems.DefaultCellStyle.BackColor = Color.White;
            dgvRItems.DefaultCellStyle.ForeColor = Color.Black;
            dgvRItems.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvRItems.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 240, 255);
            dgvRItems.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Alternate row color
            dgvRItems.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);

            // Auto-size
            dgvRItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRItems.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvRItems.AllowUserToResizeRows = false;

            // Full row select
            dgvRItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Now safe to reset button colors
            ResetButtonColors();


        }
        private void AddButtonsToGrid()
        {
            // Prevent duplicate buttons
            if (!dgvRItems.Columns.Contains("Select"))
            {
                DataGridViewButtonColumn SelectBtn = new DataGridViewButtonColumn();
                SelectBtn.Name = "Select";

                SelectBtn.Text = "Select";
                SelectBtn.UseColumnTextForButtonValue = true;
                SelectBtn.Width = 40;
                dgvRItems.Columns.Add(SelectBtn);
                SelectBtn.FlatStyle = FlatStyle.Flat;   // <-- REQUIRED

                // Style (Green Button)
                dgvRItems.Columns["Select"].DefaultCellStyle.BackColor = Color.LavenderBlush;
                dgvRItems.Columns["Select"].DefaultCellStyle.ForeColor = Color.Black;
                dgvRItems.Columns["Select"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
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
                dgvRItems.DataSource = dtItems;
                // Show total records
                lblPageInfo.Text = $"Total Products: {dtItems.Rows.Count}";
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
                        txtBarcode.Text = dr["Barcode"].ToString();
                        txtItemName.Text = dr["ItemName"].ToString();
                        lblPerCartonQty.Text = dr["Per_Carton_Quantity"].ToString();
                        lbltotal.Text = dr["Total_Quantity"].ToString();
                        lbltotalinfo.Text = $"Total Quantity Remaining: { dr["Total_Quantity"].ToString() }";
                        int totalQty = Convert.ToInt32(dr["Total_Quantity"]);

                        if (totalQty < 5)
                        {
                            lbltotalinfo.ForeColor = Color.Red;
                        }
                        else
                        {
                            lbltotalinfo.ForeColor = Color.Green;
                        }
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
            txtBarcode.Clear();
            txtItemName.Clear();

            // Reset numeric fields to zero
            txtCartonQty.Text = "0";
            txtQty.Text = "0";
            lblPerCartonQty.Text = "";
            lbltotal.Text = "";
            txtItemID.Text = "";
            lbltotalinfo.Text = "";


            // Set focus if you want
            txtItemName.Focus();
        }

        private void dgvRItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void dgvRItems_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string col = dgvRItems.Columns[e.ColumnIndex].Name;

                ResetButtonColors();
                if (col == "Select")
                    dgvRItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.CornflowerBlue;

            }
        }

        private void dgvRItems_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string col = dgvRItems.Columns[e.ColumnIndex].Name;

                ResetButtonColors();
                if (col == "Select")
                    dgvRItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Ivory;
            }
        }

        private void dgvRItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string col = dgvRItems.Columns[e.ColumnIndex].Name;

            // Get item ID for actions
            string itemID = dgvRItems.Rows[e.RowIndex].Cells["ItemID"].Value.ToString();

            if (col == "Select")
            {
                txtItemID.Text = itemID;
                LoadItemDetails();   // Your function to load item by ID
            }
        }

            private void frmRestockItems_Load(object sender, EventArgs e)
        {
            LoadItems();
            // LoadItemsPage();
            StyleGrid();
            AddButtonsToGrid();
        }

        private void txtRSearch_TextChanged(object sender, EventArgs e)
        {
            if (dtItems == null) return;

            string filter = txtSearch.Text.Replace("'", "''"); // Escape single quotes
            (dgvRItems.DataSource as DataTable).DefaultView.RowFilter =
                $"ItemName LIKE '%{filter}%' OR Barcode LIKE '%{filter}%'";

        }
        private void txtCartonQty_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCartonQty.Text))
                txtCartonQty.Text = "0";
        }
        private void txtQty_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtQty.Text))
                txtQty.Text = "0";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtItemID.Text))
            {
                MessageBox.Show("Please select an item.", "Error");
                return;
            }

            if (!int.TryParse(txtQty.Text, out int unitQty))
            {
                MessageBox.Show("Invalid unit quantity.", "Error");
                return;
            }

            if (!int.TryParse(txtCartonQty.Text, out int cartonQty))
            {
                MessageBox.Show("Invalid carton quantity.", "Error");
                return;
            }

            if (!int.TryParse(lblPerCartonQty.Text, out int perCarton))
            {
                MessageBox.Show("Invalid per-carton quantity.", "Error");
                return;
            }

            int totalToAdd = unitQty + (cartonQty * perCarton);

            if (totalToAdd <= 0)
            {
                MessageBox.Show("Quantity must be greater than zero.", "Error");
                return;
            }

            int itemID = int.Parse(txtItemID.Text);

            using (SqlConnection con = DB.GetCon())
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    // 1️⃣ Increase stock
                    SqlCommand cmdUpdate = new SqlCommand(@"
                UPDATE Items
                SET Total_Quantity = Total_Quantity + @qty
                WHERE ItemID = @id
            ", con, tran);

                    cmdUpdate.Parameters.AddWithValue("@qty", totalToAdd);
                    cmdUpdate.Parameters.AddWithValue("@id", itemID);
                    cmdUpdate.ExecuteNonQuery();

                    // 2️⃣ Log into PurchasedItems (CR only)
                    SqlCommand cmdLog = new SqlCommand(@"
                INSERT INTO PurchasedItems
                (ItemID, Cr, Dr, Purchase_Status, AddedBy)
                VALUES
                (@itemID, @cr, 0, 'Purchase', @AddedBy)
            ", con, tran);

                    cmdLog.Parameters.AddWithValue("@itemID", itemID);
                    cmdLog.Parameters.AddWithValue("@cr", totalToAdd);
                    cmdLog.Parameters.AddWithValue("@AddedBy", _currentId);
                    cmdLog.ExecuteNonQuery();

                    tran.Commit();

                    MessageBox.Show("Item restocked successfully.", "Success");

                    ClearItemFields();
                    LoadItems();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show("Restock failed: " + ex.Message);
                }
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            ClearItemFields();
        }
    }
}
