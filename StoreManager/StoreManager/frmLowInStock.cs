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
    public partial class frmLowInStock : Form
    {
        public frmLowInStock()
        {
            InitializeComponent();
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
            dgvRItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            dgvRItems.ColumnHeadersHeight = 35;

            // Row style
            dgvRItems.DefaultCellStyle.BackColor = Color.White;
            dgvRItems.DefaultCellStyle.ForeColor = Color.Black;
            dgvRItems.DefaultCellStyle.Font = new Font("Segoe UI", 18);
            dgvRItems.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 240, 255);
            dgvRItems.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Alternate row color
            dgvRItems.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);
            dgvRItems.AlternatingRowsDefaultCellStyle.Font = new Font("Segoe UI", 18);

            // Auto-size
            dgvRItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRItems.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvRItems.AllowUserToResizeRows = false;

            // Full row select
            dgvRItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Now safe to reset button colors
          


        }

        private DataTable dtItems;
        private void LoadItems()
        {
            using (SqlConnection con = DB.GetCon())
            {
                con.Open();

                string query = @"
        SELECT Barcode, ItemName, Total_Quantity, Per_Carton_Quantity
        FROM Items
        WHERE 
            (Per_Carton_Quantity > 0 AND Total_Quantity <= (Per_Carton_Quantity * 2))
            OR
            (Per_Carton_Quantity = 0 AND Total_Quantity <= 3)
        ORDER BY ItemName ASC";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                dtItems = new DataTable();
                da.Fill(dtItems);

                dtItems.DefaultView.RowFilter = "";
                dgvRItems.DataSource = dtItems;

                lblPageInfo.Text = $"Total Low Stock Items: {dtItems.Rows.Count}";
            }
        }


        private void frmLowInStock_Load(object sender, EventArgs e)
        {
            LoadItems();
            StyleGrid();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (dtItems == null) return;

            string filter = txtSearch.Text.Replace("'", "''"); // Escape single quotes
            (dgvRItems.DataSource as DataTable).DefaultView.RowFilter =
                $"ItemName LIKE '%{filter}%' OR Barcode LIKE '%{filter}%'";
        }
    }
}
