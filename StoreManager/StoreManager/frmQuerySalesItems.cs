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
    public partial class frmQuerySalesItems : Form
    {
        private DataTable dtSummary;

        public frmQuerySalesItems()
        {
            InitializeComponent();
            txtSearchItemName.TextChanged += txtSearchItemName_TextChanged;
            dtFrom.ValueChanged += dtFrom_ValueChanged;
            dtTo.ValueChanged += dtTo_ValueChanged;
            cmbUsers.SelectedIndexChanged += cmbUsers_SelectedIndexChanged;
            
        }

        private void frmQuerySalesItems_Load(object sender, EventArgs e)
        {
            dtFrom.Value = DateTime.Today;
            dtTo.Value = DateTime.Today;
            LoadSalesItems();
            LoadUsers();
        }
        private void LoadUsers()
        {
            using (SqlConnection con = DB.GetCon())
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT UserID,
                 Firstname + ' ' + Lastname AS FullName
          FROM Users
          ORDER BY Firstname", con))
            {
                DataTable dt = new DataTable();
                con.Open();
                dt.Load(cmd.ExecuteReader());

                // 🔹 Insert "Select User" at the top
                DataRow row = dt.NewRow();
                row["UserID"] = 0;              // fake ID
                row["FullName"] = "-- Select User --";
                dt.Rows.InsertAt(row, 0);

                cmbUsers.DataSource = dt;
                cmbUsers.DisplayMember = "FullName";
                cmbUsers.ValueMember = "UserID";
                cmbUsers.SelectedIndex = 0;     // default selection
            }
        }

        private void ApplyFiltersAndTotals()
        {
            if (dtSummary == null) return;

            List<string> filters = new List<string>();

            // DATE FILTER
            string from = dtFrom.Value.ToString("yyyy-MM-dd 00:00:00");
            string to = dtTo.Value.ToString("yyyy-MM-dd 23:59:59");
            filters.Add($"Date_Sold >= '#{from}#' AND Date_Sold <= '#{to}#'");

            // TEXT SEARCH
            string search = txtSearchItemName.Text.Trim().Replace("'", "''");
            if (!string.IsNullOrEmpty(search))
            {
                filters.Add(
                    $"ItemName LIKE '%{search}%' " 
                );
            }

            // USER FILTER
            if (cmbUsers.SelectedIndex > 0)
            {
                int userId = Convert.ToInt32(cmbUsers.SelectedValue);
                filters.Add($"SoldByID = {userId}");
            }

            // APPLY FILTER
            dtSummary.DefaultView.RowFilter = string.Join(" AND ", filters);

            RecalculateTotal();
        }

        private void LoadSalesItems()
        {
            try
            {
                using (SqlConnection con = DB.GetCon())
                {
                    con.Open();

                    string query = @"
                                    SELECT
                                        s.Invoice_No,
                                        s.Receipt_No,
                                        i.ItemName,
                                        s.Carton_Qty,
                                        s.Carton_SP,
                                        s.Units,
                                        s.Unit_SP,
                                        s.Total_Qty,
                                        s.Subtotal,
                                        s.Date_Sold,
                                        s.SoldBy AS SoldByID,
                                        u.Firstname AS SoldBy
                                    FROM Sales s
                                    INNER JOIN Items i ON s.ItemID = i.ItemID
                                    INNER JOIN Users u ON s.SoldBy = u.UserID
                                    WHERE s.Date_Sold >= @start
                                      AND s.Date_Sold <= @end
                                    ORDER BY s.Date_Sold DESC";


                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@start", dtFrom.Value.Date);
                    cmd.Parameters.AddWithValue("@end", dtTo.Value.Date.AddDays(1).AddSeconds(-1));

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    dtSummary = new DataTable();
                    da.Fill(dtSummary);

                    dgvSummary.DataSource = dtSummary;

                    RecalculateTotal();
                    dgvSummary.DataSource = dtSummary;
                    if (dgvSummary.Columns.Contains("SoldByID"))
                    {
                        dgvSummary.Columns["SoldByID"].Visible = false;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sales items: " + ex.Message);
            }

            StyleGrid();
        }

        private void ApplyFilters()
        {
            if (dtSummary == null) return;

            string search = txtSearchItemName.Text.Trim().Replace("'", "''");

            string dateFrom = dtFrom.Value.ToString("yyyy-MM-dd 00:00:00");
            string dateTo = dtTo.Value.ToString("yyyy-MM-dd 23:59:59");

            string filter = $"Date_Sold >= '#{dateFrom}#' AND Date_Sold <= '#{dateTo}#'";

            if (!string.IsNullOrEmpty(search))
                filter += $" AND ItemName LIKE '%{search}%'";

            dtSummary.DefaultView.RowFilter = filter;

            RecalculateTotal();
        }

        private void RecalculateTotal()
        {
            if (dtSummary == null || dtSummary.DefaultView.Count == 0)
            {
                lblTotalItems.Text = "0";
                return;
            }

            int total = dtSummary.DefaultView
                                 .Cast<DataRowView>()
                                 .Sum(r => Convert.ToInt32(r["Total_Qty"]));


            lblTotalItems.Text = total.ToString();
        }

        private void StyleGrid()
        {
            dgvSummary.BorderStyle = BorderStyle.None;
            dgvSummary.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvSummary.RowHeadersVisible = false;

            dgvSummary.EnableHeadersVisualStyles = false;
            dgvSummary.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvSummary.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSummary.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            dgvSummary.ColumnHeadersHeight = 40;

            dgvSummary.DefaultCellStyle.Font = new Font("Segoe UI", 11);
            dgvSummary.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);

            dgvSummary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgvSummary.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dgvSummary.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadSalesItems();
        }

        private void txtSearchItemName_TextChanged(object sender, EventArgs e)
        {
           
            ApplyFiltersAndTotals();
        }

        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadSalesItems();
            ApplyFiltersAndTotals();
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            LoadSalesItems();
            ApplyFiltersAndTotals();
        }

        private void cmbUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSalesItems();
            ApplyFiltersAndTotals();
        }
    }
}
