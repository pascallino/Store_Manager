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
            // Remove borders + smooth look
            dgvRItems.BorderStyle = BorderStyle.None;
            dgvRItems.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvRItems.RowHeadersVisible = false;

            // Header Style
            dgvRItems.EnableHeadersVisualStyles = false;
            dgvRItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);   // Blue
            dgvRItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvRItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            dgvRItems.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvRItems.ColumnHeadersHeight = 40;

            // Row Style
            dgvRItems.DefaultCellStyle.BackColor = Color.White;
            dgvRItems.DefaultCellStyle.ForeColor = Color.Black;
            dgvRItems.DefaultCellStyle.Font = new Font("Segoe UI", 11);
            dgvRItems.DefaultCellStyle.Padding = new Padding(5);
            dgvRItems.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 240, 255);
            dgvRItems.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Alternate row color
            dgvRItems.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);

            // Fit columns to content — BEST LOOK
            dgvRItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            // Rows scale with content height
            dgvRItems.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;



            // Prevent ugly long-height rows
            dgvRItems.RowTemplate.Height = 35;
            dgvRItems.AllowUserToResizeRows = false;

            // Improve readability
            dgvRItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Smooth scrollbar
            dgvRItems.ScrollBars = ScrollBars.Both;

            // Prevent column text from cutting off
            dgvRItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
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
