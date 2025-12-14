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
    public partial class frmItems : Form
    {
        private int pageSize = 19;       // rows per page
        private int currentPage = 1;     // current page
        private int totalRecords = 0;    // total rows in table
        private int totalPages = 0;      // total pages

        public frmItems()
        {
            InitializeComponent();
            dgvItems.CellClick += dgvItems_CellClick;
            dgvItems.CellContentClick += dgvItems_CellClick;
            dgvItems.CellMouseEnter += dgvItems_CellMouseEnter;
            dgvItems.CellMouseLeave += dgvItems_CellMouseLeave;
            txtCartonQty.LostFocus += txtCartonQty_LostFocus;
            txtQty.LostFocus += txtQty_LostFocus;
            txtQtyCP.LostFocus += txtQtyCP_LostFocus;
            txtQtySP.LostFocus += txtQtySP_LostFocus;
            txtCartonSP.LostFocus += txtCartonSP_LostFocus;
            txtCartonCP.LostFocus += txtCartonCP_LostFocus;
            txtPerCartonQty.LostFocus += txtPerCartonQty_LostFocus;
            btnPrev.Click += btnPrev_Click;
            btnNext.Click += btnNext_Click;

        }
        private void LoadItemsPage()
        {
            using (SqlConnection con = DB.GetCon())
            {
                con.Open();

                // Get total record count
                SqlCommand countCmd = new SqlCommand("SELECT COUNT(*) FROM Items", con);
                totalRecords = (int)countCmd.ExecuteScalar();
                totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                // Fetch current page
                string query = @"
            SELECT ItemID, Barcode, ItemName, Total_Quantity
            FROM Items
            ORDER BY ItemID
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Offset", (currentPage - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvItems.DataSource = dt;
            }
            lblPageInfo.Text = $"Page {currentPage} of {totalPages}";

        }


        private void StyleGrid()
        {
            // Remove ugly borders
            dgvItems.BorderStyle = BorderStyle.None;
            dgvItems.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvItems.RowHeadersVisible = false;

            // Header style
            dgvItems.EnableHeadersVisualStyles = false;
            dgvItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 144, 255);
            dgvItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvItems.ColumnHeadersHeight = 35;

            // Row style
            dgvItems.DefaultCellStyle.BackColor = Color.White;
            dgvItems.DefaultCellStyle.ForeColor = Color.Black;
            dgvItems.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvItems.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 240, 255);
            dgvItems.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Alternate row color
            dgvItems.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);

            // Auto-size
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvItems.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvItems.AllowUserToResizeRows = false;

            // Full row select
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Now safe to reset button colors
            ResetButtonColors();


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
                dgvItems.DataSource = dtItems;
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
                        txtQty.ReadOnly = true;
                        txtCartonQty.ReadOnly = true;
                        txtBarcode.Text = dr["Barcode"].ToString();
                        txtItemName.Text = dr["ItemName"].ToString();
                        txtCartonQty.Text = dr["Carton_Quantity"].ToString();
                        txtQty.Text = dr["Quantity"].ToString();
                        txtPerCartonQty.Text = dr["Per_Carton_Quantity"].ToString();
                        txtCartonCP.Text = dr["Carton_CP"].ToString();
                        txtCartonSP.Text = dr["Carton_SP"].ToString();
                        txtQtyCP.Text = dr["Quantity_CP"].ToString();
                        txtQtySP.Text = dr["Quantity_SP"].ToString();
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
        private void AddButtonsToGrid()
        {
            // Prevent duplicate buttons
            if (!dgvItems.Columns.Contains("Edit"))
            {
                DataGridViewButtonColumn editBtn = new DataGridViewButtonColumn();
                editBtn.Name = "Edit";
             
                editBtn.Text = "Edit";
                editBtn.UseColumnTextForButtonValue = true;
                editBtn.Width = 40;
                dgvItems.Columns.Add(editBtn);
                editBtn.FlatStyle = FlatStyle.Flat;   // <-- REQUIRED

                // Style (Green Button)
                dgvItems.Columns["Edit"].DefaultCellStyle.BackColor = Color.LightGreen;
            dgvItems.Columns["Edit"].DefaultCellStyle.ForeColor = Color.Black;
            dgvItems.Columns["Edit"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
           


            if (!dgvItems.Columns.Contains("Delete"))
            {
                DataGridViewButtonColumn deleteBtn = new DataGridViewButtonColumn();
                deleteBtn.Name = "Delete";
                deleteBtn.Text = "Delete";
                deleteBtn.UseColumnTextForButtonValue = true;
                deleteBtn.Width = 40;
                dgvItems.Columns.Add(deleteBtn);
                deleteBtn.FlatStyle = FlatStyle.Flat;   // <-- REQUIRED
                // Style (Red Button)
                dgvItems.Columns["Delete"].DefaultCellStyle.BackColor = Color.Salmon;
                dgvItems.Columns["Delete"].DefaultCellStyle.ForeColor = Color.Black;
                dgvItems.Columns["Delete"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            }
        }
        private void ResetButtonColors()
        {
            if (!dgvItems.Columns.Contains("Edit") || !dgvItems.Columns.Contains("Delete"))
                return;

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                if (!row.IsNewRow)
                {
                    row.Cells["Edit"].Style.BackColor = Color.LightGreen;
                    row.Cells["Delete"].Style.BackColor = Color.Salmon;
                }
            }
        }


        private void ClearItemFields()
        {
            // Clear string fields
            txtBarcode.Clear();
            txtItemName.Clear();
            txtQty.ReadOnly = false;
            txtCartonQty.ReadOnly = false;;
            // Reset numeric fields to zero
            txtCartonQty.Text = "0";
            txtQty.Text = "0";
            txtPerCartonQty.Text = "0";
            txtCartonCP.Text = "0";
            txtCartonSP.Text = "0";
            txtQtyCP.Text = "0";
            txtQtySP.Text = "0";

            // Set focus if you want
            txtItemName.Focus();
        }


        private void dgvItems_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string col = dgvItems.Columns[e.ColumnIndex].Name;

                ResetButtonColors();
                if (col == "Edit")
                    dgvItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.MediumSeaGreen;

                if (col == "Delete")
                    dgvItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.IndianRed;
            }
        }

        private void dgvItems_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string col = dgvItems.Columns[e.ColumnIndex].Name;

                ResetButtonColors();
                if (col == "Edit")
                    dgvItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightGreen;

                if (col == "Delete")
                    dgvItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Salmon;
            }
        }


        private void DeleteItem(string id)
        {
            try
            {
                using (SqlConnection con = DB.GetCon())
                {
                    con.Open();

                    string query = "DELETE FROM Items WHERE ItemID = @id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete failed: " + ex.Message);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void frmItems_Load(object sender, EventArgs e)
        {
            LoadItems();
            // LoadItemsPage();
            StyleGrid();
            AddButtonsToGrid();




        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            // Validate string fields

            if (string.IsNullOrWhiteSpace(txtItemName.Text))
            {
                MessageBox.Show("Item Name cannot be empty");
                return;
            }


            // Validate numeric fields
            if (!int.TryParse(txtCartonQty.Text, out int cartonQty))
            {
                MessageBox.Show("Invalid carton quantity");
                return;
            }

            if (!int.TryParse(txtQty.Text, out int qty))
            {
                MessageBox.Show("Invalid quantity");
                return;
            }

            if (!int.TryParse(txtPerCartonQty.Text, out int perCartonQty))
            {
                MessageBox.Show("Invalid per-carton quantity");
                return;
            }
            if (!int.TryParse(txtQtyCP.Text, out int QtyCP))
            {
                MessageBox.Show("Invalid Cost Price");
                return;
            }
            if(!int.TryParse(txtQtySP.Text, out int QtySP))
            {
                MessageBox.Show("Invalid Selling Price");
                return;
            }
            if (!int.TryParse(txtCartonSP.Text, out int CartonSP))
            {
                MessageBox.Show("Invalid Selling Price");
                return;
            }
            if (!int.TryParse(txtCartonCP.Text, out int CartonCP))
            {
                MessageBox.Show("Invalid Cost Price");
                return;
            }





            // Calculate total quantity
            int totalQty = (cartonQty * perCartonQty) + qty;

            using (SqlConnection con = DB.GetCon())
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    if (string.IsNullOrWhiteSpace(txtItemID.Text))
                    {
                        // 🔹 1. INSERT ITEM
                        SqlCommand cmdItem = new SqlCommand(@"
                    INSERT INTO Items 
                    (Barcode, ItemName, Carton_Quantity, Quantity, Per_Carton_Quantity, Total_Quantity,
                     Carton_CP, Carton_SP, Quantity_CP, Quantity_SP, Created_at)
                    VALUES
                    (@Barcode, @ItemName, @CartonQty, @Qty, @PerCartonQty, @TotalQty,
                     @CartonCP, @CartonSP, @QtyCP, @QtySP, GETDATE());

                    SELECT SCOPE_IDENTITY();
                ", con, tran);

                        cmdItem.Parameters.AddWithValue("@Barcode", txtBarcode.Text);
                        cmdItem.Parameters.AddWithValue("@ItemName", txtItemName.Text);
                        cmdItem.Parameters.AddWithValue("@CartonQty", cartonQty);
                        cmdItem.Parameters.AddWithValue("@Qty", qty);
                        cmdItem.Parameters.AddWithValue("@PerCartonQty", perCartonQty);
                        cmdItem.Parameters.AddWithValue("@TotalQty", totalQty);
                        cmdItem.Parameters.AddWithValue("@CartonCP", txtCartonCP.Text);
                        cmdItem.Parameters.AddWithValue("@CartonSP", txtCartonSP.Text);
                        cmdItem.Parameters.AddWithValue("@QtyCP", txtQtyCP.Text);
                        cmdItem.Parameters.AddWithValue("@QtySP", txtQtySP.Text);

                        int newItemID = Convert.ToInt32(cmdItem.ExecuteScalar());

                        // 🔹 2. LOG INITIAL ENTRY INTO PURCHASED ITEMS
                        SqlCommand cmdLedger = new SqlCommand(@"
                    INSERT INTO PurchasedItems
                    (ItemID, Cr, Dr, Purchase_Status, Created_At)
                    VALUES
                    (@ItemID, @Cr, 0, 'INITIAL_ENTRY', GETDATE())
                ", con, tran);

                        cmdLedger.Parameters.AddWithValue("@ItemID", newItemID);
                        cmdLedger.Parameters.AddWithValue("@Cr", totalQty);
                        cmdLedger.ExecuteNonQuery();

                        tran.Commit();

                        MessageBox.Show("Item saved successfully with initial stock entry!");
                    }
                    else
                    {
                        // 🔹 UPDATE ITEM DETAILS ONLY (NO STOCK MOVEMENT)
                        SqlCommand cmdUpdate = new SqlCommand(@"
                    UPDATE Items
                    SET Barcode = @Barcode,
                        ItemName = @ItemName,
                        Per_Carton_Quantity = @PerCartonQty,
                        Carton_CP = @CartonCP,
                        Carton_SP = @CartonSP,
                        Quantity_CP = @QtyCP,
                        Quantity_SP = @QtySP,
                        Updated_at = GETDATE()
                    WHERE ItemID = @ID
                ", con, tran);

                        cmdUpdate.Parameters.AddWithValue("@ID", txtItemID.Text);
                        cmdUpdate.Parameters.AddWithValue("@Barcode", txtBarcode.Text);
                        cmdUpdate.Parameters.AddWithValue("@ItemName", txtItemName.Text);
                        cmdUpdate.Parameters.AddWithValue("@PerCartonQty", perCartonQty);
                        cmdUpdate.Parameters.AddWithValue("@CartonCP", txtCartonCP.Text);
                        cmdUpdate.Parameters.AddWithValue("@CartonSP", txtCartonSP.Text);
                        cmdUpdate.Parameters.AddWithValue("@QtyCP", txtQtyCP.Text);
                        cmdUpdate.Parameters.AddWithValue("@QtySP", txtQtySP.Text);

                        cmdUpdate.ExecuteNonQuery();
                        tran.Commit();

                        MessageBox.Show("Item updated successfully!");
                    }

                    LoadItems();
                    ClearItemFields();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }


        private void dgvItems_SelectionChanged(object sender, EventArgs e)
        {
         
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgvItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string col = dgvItems.Columns[e.ColumnIndex].Name;

            // Get item ID for actions
            string itemID = dgvItems.Rows[e.RowIndex].Cells["ItemID"].Value.ToString();
           
            if (col == "Edit")
            {
                txtItemID.Text = itemID;
                LoadItemDetails();   // Your function to load item by ID
            }

            if (col == "Delete")
            {
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to delete this item?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    DeleteItem(itemID);
                    ClearItemFields();
                    LoadItems();  // Refresh grid
                    // LoadItemsPage();
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearItemFields();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearItemFields();
        }

        // Live search
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (dtItems == null) return;

            string filter = txtSearch.Text.Replace("'", "''"); // Escape single quotes
            (dgvItems.DataSource as DataTable).DefaultView.RowFilter =
                $"ItemName LIKE '%{filter}%' OR Barcode LIKE '%{filter}%'";
        }


        private void txtCartonQty_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCartonQty.Text))
                txtCartonQty.Text = "0";
        }

        private void txtQtyCP_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtQtyCP.Text))
                txtQtyCP.Text = "0";
        }
        private void txtQtySP_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtQtySP.Text))
                txtQtySP.Text = "0";
        }
        private void txtQty_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtQty.Text))
                txtQty.Text = "0";
        }
        private void txtCartonCP_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCartonCP.Text))
                txtCartonCP.Text = "0";
        }
        private void txtCartonSP_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCartonSP.Text))
                txtCartonSP.Text = "0";
        }

        private void txtPerCartonQty_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPerCartonQty.Text))
                txtPerCartonQty.Text = "0";
        }

        private void txtCartonQty_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadItemsPage();


            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadItemsPage();
            }
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }
    }
}
