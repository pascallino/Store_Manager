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
    public partial class frmQueryPurchase : Form
    {
        private DataTable dtSummary;

        public frmQueryPurchase()
        {
            InitializeComponent();
            btnSearch.Click += btnSearch_Click;
            txtSearchItemName.TextChanged += txtSearchItemName_TextChanged;
        }
        private void LoadPurchaseSummary()
        {
            try
            {
                using (SqlConnection con = DB.GetCon())
                {
                    con.Open();

                    string query = @"
                SELECT 
                    i.ItemName,
                    p.Cr,
                    p.Dr,
                    p.Purchase_Status,
                    p.Created_At
                FROM PurchasedItems p
                INNER JOIN Items i ON p.ItemID = i.ItemID
                WHERE p.Created_At >= @start
                  AND p.Created_At <= @end
                ORDER BY p.Created_At DESC;
            ";

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
                MessageBox.Show("Error loading purchases: " + ex.Message);
            }

            StyleGrid();
        }

        private void StyleGrid()
        {
            dgvSummary.BorderStyle = BorderStyle.None;
            dgvSummary.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvSummary.RowHeadersVisible = false;

            dgvSummary.EnableHeadersVisualStyles = false;
            dgvSummary.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvSummary.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSummary.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            dgvSummary.ColumnHeadersHeight = 40;

            dgvSummary.DefaultCellStyle.Font = new Font("Segoe UI", 20);
            dgvSummary.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);

            dgvSummary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgvSummary.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dgvSummary.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
        private void ApplyFilters()
        {
            if (dtSummary == null) return;

            string search = txtSearchItemName.Text.Trim().Replace("'", "''");

            string dateFrom = dtFrom.Value.ToString("yyyy-MM-dd 00:00:00");
            string dateTo = dtTo.Value.ToString("yyyy-MM-dd 23:59:59");

            string filter = $"Created_At >= '#{dateFrom}#' AND Created_At <= '#{dateTo}#'";

            if (!string.IsNullOrEmpty(search))
            {
                filter += $" AND ItemName LIKE '%{search}%'";
            }

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

            decimal total = 0;

            foreach (DataRowView row in dtSummary.DefaultView)
            {
                decimal cr = row["Cr"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Cr"]);
                decimal dr = row["Dr"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Dr"]);

                total += (cr - dr);
            }

            lblTotalItems.Text = total.ToString("N0");
        }

        private void frmQueryPurchase_Load(object sender, EventArgs e)
        {
            dtFrom.Value = DateTime.Today;
            dtTo.Value = DateTime.Today;

            LoadPurchaseSummary();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadPurchaseSummary();
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
