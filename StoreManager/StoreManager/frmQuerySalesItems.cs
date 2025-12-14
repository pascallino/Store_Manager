using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace StoreManager
{
    public partial class frmQuerySalesItems : Form
    {
        private DataTable dtSummary;

        public frmQuerySalesItems()
        {
            InitializeComponent();
            txtSearchItemName.TextChanged += txtSearchItemName_TextChanged;
        }

        private void frmQuerySalesItems_Load(object sender, EventArgs e)
        {
            dtFrom.Value = DateTime.Today;
            dtTo.Value = DateTime.Today;
            LoadSalesItems();
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
                            s.Units,
                            s.Total_Qty,
                            s.Subtotal,
                            s.Date_Sold
                        FROM Sales s
                        INNER JOIN Items i ON s.ItemID = i.ItemID
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

            lblTotalItems.Text = total.ToString("N0");
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
