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


    public partial class frmQueryDeletedSale : Form
    {
        public frmQueryDeletedSale()
        {
            InitializeComponent();
            dgvSummary.CellContentClick += dgvSummary_CellContentClick;
            
        }
        private DataTable dtSummary;   // store summary table for filtering

        private void FormatSummaryGrid()
        {
            if (!dgvSummary.Columns.Contains("View"))
            {
                DataGridViewButtonColumn viewBtn = new DataGridViewButtonColumn();
                viewBtn.HeaderText = "Action";
                viewBtn.Text = "View";
                viewBtn.Name = "View";
                viewBtn.UseColumnTextForButtonValue = true;
                viewBtn.Width = 80;
                dgvSummary.Columns.Add(viewBtn);
            }



            dgvSummary.Columns["Subtotal"].DefaultCellStyle.Format = "N2";
            dgvSummary.Columns["CashReceived"].DefaultCellStyle.Format = "N2";
            dgvSummary.Columns["Balance"].DefaultCellStyle.Format = "N2";
            dgvSummary.Columns["DeletedAt"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
        }

        private void StyleGrid()
        {
            // Remove borders + smooth look
            dgvSummary.BorderStyle = BorderStyle.None;
            dgvSummary.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvSummary.RowHeadersVisible = false;

            // Header Style
            dgvSummary.EnableHeadersVisualStyles = false;
            dgvSummary.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);   // Blue
            dgvSummary.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSummary.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            dgvSummary.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvSummary.ColumnHeadersHeight = 40;

            // Row Style
            dgvSummary.DefaultCellStyle.BackColor = Color.White;
            dgvSummary.DefaultCellStyle.ForeColor = Color.Black;
            dgvSummary.DefaultCellStyle.Font = new Font("Segoe UI", 11);
            dgvSummary.DefaultCellStyle.Padding = new Padding(5);
            dgvSummary.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 240, 255);
            dgvSummary.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Alternate row color
            dgvSummary.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);

            // Fit columns to content — BEST LOOK
            dgvSummary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            // Rows scale with content height
            dgvSummary.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;



            // Prevent ugly long-height rows
            dgvSummary.RowTemplate.Height = 35;
            dgvSummary.AllowUserToResizeRows = false;

            // Improve readability
            dgvSummary.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Smooth scrollbar
            dgvSummary.ScrollBars = ScrollBars.Both;

            // Prevent column text from cutting off
            dgvSummary.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
        }

        private void ApplyFiltersAndTotals()
        {
            if (dtSummary == null) return;

            List<string> filters = new List<string>();

            // DATE FILTER
            string from = dtFrom.Value.ToString("yyyy-MM-dd 00:00:00");
            string to = dtTo.Value.ToString("yyyy-MM-dd 23:59:59");
            filters.Add($"DeletedAt >= '#{from}#' AND DeletedAt <= '#{to}#'");

            // TEXT SEARCH
            string search = txtCashReceived_ReceiptNo.Text.Trim().Replace("'", "''");
            if (!string.IsNullOrEmpty(search))
            {
                filters.Add(
                    $"(Invoice_No LIKE '%{search}%' " +
                    $"OR Receipt_No LIKE '%{search}%')"
                );
            }

            // USER FILTER
            if (cmbUsers.SelectedIndex > 0)
            {
                int userId = Convert.ToInt32(cmbUsers.SelectedValue);
                filters.Add($"DeletedBy = {userId}");
            }

            // APPLY FILTER
            dtSummary.DefaultView.RowFilter = string.Join(" AND ", filters);

            // ✅ SUM ONLY FILTERED ROWS
            decimal total = 0m;
            foreach (DataRowView row in dtSummary.DefaultView)
            {
                if (row["Subtotal"] != DBNull.Value)
                    total += Convert.ToDecimal(row["Subtotal"]);
            }

            lblTotalSales.Text = total.ToString("N2");


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

        private void LoadSummary()
        {
            try
            {
                using (SqlConnection con = DB.GetCon())
                {
                    con.Open();

                    string query = @"
                SELECT 
                    Invoice_No,
                    Receipt_No,
                    Subtotal,
                    CashReceived,
                    Balance,
                    DeletedAt,
                    DeletedBy
                FROM DeletedSales
                WHERE DeletedAt >= @start AND DeletedAt <= @end
                ORDER BY DeletedAt DESC;
            ";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@start", dtFrom.Value.Date);
                    cmd.Parameters.AddWithValue("@end", dtTo.Value.Date.AddDays(1).AddSeconds(-1));

                    SqlDataAdapter da = new SqlDataAdapter(cmd);

                    dtSummary = new DataTable();
                    da.Fill(dtSummary);
                    // ⬇️ SUM ALL SUBTOTAL VALUES
                    decimal totalSales = dtSummary.AsEnumerable()
                                                  .Sum(r => r.Field<decimal>("Subtotal"));

                    lblTotalSales.Text = totalSales.ToString("N2");   // SHOW IN LABEL


                    dgvSummary.DataSource = dtSummary;
                    if (dgvSummary.Columns.Contains("DeletedBy"))
                    {
                        dgvSummary.Columns["DeletedBy"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading summary: " + ex.Message);
            }

            FormatSummaryGrid();
            StyleGrid();
        }


        private void frmQueryDeletedSale_Load(object sender, EventArgs e)
        {
            dtFrom.Value = DateTime.Today;
            dtTo.Value = DateTime.Today;

            LoadSummary();
            LoadUsers();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadSummary();
        }

        private void dgvSummary_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string col = dgvSummary.Columns[e.ColumnIndex].Name;
            string invoice = dgvSummary.Rows[e.RowIndex].Cells["Invoice_No"].Value.ToString();
            string receipt = dgvSummary.Rows[e.RowIndex].Cells["Receipt_No"].Value.ToString();

            if (col == "View")
            {
                var f = new frmViewSale(invoice, receipt, "DeletedItems");
                f.ShowDialog();
            }
        }


        private void txtCashReceived_ReceiptNo_TextChanged(object sender, EventArgs e)
        {
            ApplyFiltersAndTotals();
        }

        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadSummary();
            ApplyFiltersAndTotals();
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            LoadSummary();
            ApplyFiltersAndTotals();
        }

        private void cmbUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFiltersAndTotals();
        }

        private void dtTo_ValueChanged_1(object sender, EventArgs e)
        {

        }

       
    }
}