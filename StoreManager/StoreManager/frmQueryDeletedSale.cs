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

        private void ApplyFilters()
        {
            if (dtSummary == null || dtSummary.Rows.Count == 0)
                return;

            string search = txtCashReceived_ReceiptNo.Text.Trim().Replace("'", "''");

            string dateFrom = dtFrom.Value.ToString("yyyy-MM-dd 00:00:00");
            string dateTo = dtTo.Value.ToString("yyyy-MM-dd 23:59:59");

            // base date filter
            string filter = $"DeletedAt >= '#{dateFrom}#' AND DeletedAt <= '#{dateTo}#'";

            if (!string.IsNullOrEmpty(search))
            {
                filter += $" AND (Invoice_No LIKE '%{search}%' " +
                          $"OR Receipt_No LIKE '%{search}%' " +
                          $"OR CONVERT(CashReceived, 'System.String') LIKE '%{search}%')";
            }

            dtSummary.DefaultView.RowFilter = filter;
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
                    DeletedAt
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
            ApplyFilters();
        }

        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }
       

    }
}
