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
 

    public partial class frmQuerySale : Form
    {
        public frmQuerySale()
        {
            InitializeComponent();
         
           
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
            // --- PRINT BUTTON ---
            if (!dgvSummary.Columns.Contains("Print"))
            {
                DataGridViewButtonColumn printBtn = new DataGridViewButtonColumn();
                printBtn.HeaderText = "Print";
                printBtn.Text = "Print";
                printBtn.Name = "Print";
                printBtn.UseColumnTextForButtonValue = true;
                printBtn.Width = 70;
                dgvSummary.Columns.Add(printBtn);
            }


            dgvSummary.Columns["Subtotal"].DefaultCellStyle.Format = "N2";
            dgvSummary.Columns["CashReceived"].DefaultCellStyle.Format = "N2";
            dgvSummary.Columns["Balance"].DefaultCellStyle.Format = "N2";
            dgvSummary.Columns["Date_Sold"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
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
            string filter = $"Date_Sold >= '#{dateFrom}#' AND Date_Sold <= '#{dateTo}#'";

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
                    Date_Sold
                FROM Sales_Summary
                WHERE Date_Sold >= @start AND Date_Sold <= @end
                ORDER BY Date_Sold DESC;
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


        private void frmQuerySale_Load(object sender, EventArgs e)
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
                var f = new frmViewSale(invoice, receipt, "Sales");
                f.ShowDialog();
            }
            else if (col == "Print")
            {
                PrintReceipt(invoice, receipt);
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
        private void PrintReceipt(string invoiceNo, string receiptNo)
        {
            try
            {
                // 1) Load sale lines and build itemsList (same format as checkout)
                string itemsList = "";
                decimal subtotal = 0m;
                decimal cashReceived = 0m;
                decimal balance = 0m;

                using (SqlConnection con = DB.GetCon())
                {
                    con.Open();

                    // Query sales lines + item prices
                    string qLines = @"
                SELECT s.ItemID, i.ItemName, s.Carton_Qty, s.Units, s.Total_Qty, 
                       i.Carton_SP AS CartonPrice, i.Quantity_SP AS UnitPrice,
                       s.Subtotal AS LineTotal
                FROM Sales s
                JOIN Items i ON s.ItemID = i.ItemID
                WHERE s.Invoice_No = @inv OR s.Receipt_No = @rec
                ORDER BY s.SaleID"; // optional ordering
                    using (SqlCommand cmd = new SqlCommand(qLines, con))
                    {
                        cmd.Parameters.AddWithValue("@inv", invoiceNo);
                        cmd.Parameters.AddWithValue("@rec", receiptNo);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                string name = dr["ItemName"].ToString();
                                int cartons = dr["Carton_Qty"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Carton_Qty"]);
                                int units = dr["Units"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Units"]);
                                int totalUnits = dr["Total_Qty"] == DBNull.Value ? (cartons + units) : Convert.ToInt32(dr["Total_Qty"]);
                                decimal cartonPrice = dr["CartonPrice"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["CartonPrice"]);
                                decimal unitPrice = dr["UnitPrice"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["UnitPrice"]);

                                // Prefer stored line total if present; otherwise compute using carton/unit prices
                                decimal lineTotal = dr["LineTotal"] == DBNull.Value
                                    ? ((cartons * cartonPrice) + (units * unitPrice))
                                    : Convert.ToDecimal(dr["LineTotal"]);

                                // Keep same formatting as checkout
                                itemsList += $"{name,-18} x{totalUnits,-4} ₦{lineTotal,10:n2}\n";

                                subtotal += lineTotal; // accumulate in case summary lookup fails
                            }
                        }
                    }

                    // 2) Load summary info (subtotal, cash, balance) from Sales_Summary
                    string qSummary = @"
                SELECT TOP 1 Subtotal, CashReceived, Balance
                FROM Sales_Summary
                WHERE Invoice_No = @inv OR Receipt_No = @rec
                ORDER BY Date_Sold DESC";
                    using (SqlCommand cmd2 = new SqlCommand(qSummary, con))
                    {
                        cmd2.Parameters.AddWithValue("@inv", invoiceNo);
                        cmd2.Parameters.AddWithValue("@rec", receiptNo);

                        using (SqlDataReader dr2 = cmd2.ExecuteReader())
                        {
                            if (dr2.Read())
                            {
                                // override subtotal with canonical value from summary (if available)
                                subtotal = dr2["Subtotal"] == DBNull.Value ? subtotal : Convert.ToDecimal(dr2["Subtotal"]);
                                cashReceived = dr2["CashReceived"] == DBNull.Value ? 0m : Convert.ToDecimal(dr2["CashReceived"]);
                                balance = dr2["Balance"] == DBNull.Value ? (cashReceived - subtotal) : Convert.ToDecimal(dr2["Balance"]);
                            }
                        }
                    }
                } // using con

                // 3) Build receipt text using the same BuildReceipt method you use in checkout
                string cashier = "Admin"; // or pull logged-in user if you have one
                string receiptText = BuildReceipt(invoiceNo, cashier, itemsList, subtotal, cashReceived, balance);

                // 4) Send to printer (same printer name used in checkout)
                string printer = "EPSON TM-T20";
                RawPrinterHelper.SendStringToPrinter(printer, receiptText);

                MessageBox.Show("Receipt reprinted successfully!", "Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing receipt: " + ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private string BuildReceipt(
   string invoiceNo,
   string cashier,
   string itemsList,
   decimal subtotal,
   decimal cashReceived,
   decimal balance)
        {
            // ESC/POS COMMANDS
            string ESC = "\x1B";
            string GS = "\x1D";

            string CENTER = ESC + "a" + "\x01";
            string LEFT = ESC + "a" + "\x00";
            string RIGHT = ESC + "a" + "\x02";

            string BOLD_ON = ESC + "E" + "\x01";
            string BOLD_OFF = ESC + "E" + "\x00";

            string DOUBLE_ON = GS + "!" + "\x11";  // Double height + width
            string DOUBLE_OFF = GS + "!" + "\x00";

            string CUT = ESC + "i";

            // Build text
            string r = "";

            // Shop Name
            r += CENTER + DOUBLE_ON + BOLD_ON + "STORE MANAGER\n";
            r += DOUBLE_OFF + BOLD_OFF;
            r += CENTER + "No. 12 Main Street, Lagos\n";
            r += CENTER + "Tel: 0800-123-4567\n\n";

            // Invoice Info
            r += LEFT;
            r += $"Invoice: {invoiceNo}\n";
            r += $"Cashier: {cashier}\n";
            r += $"Date: {DateTime.Now:dd/MM/yyyy  HH:mm}\n";
            r += "------------------------------------------\n";

            // Items
            r += itemsList;
            r += "------------------------------------------\n";

            // Subtotal
            r += RIGHT + $"Subtotal: ₦{subtotal:n2}\n";

            // Amount Paid
            r += RIGHT + $"Cash: ₦{cashReceived:n2}\n";

            // Balance (Double Size)
            r += DOUBLE_ON + RIGHT + $"Balance: ₦{balance:n2}\n";
            r += DOUBLE_OFF;

            r += "------------------------------------------\n";
            r += CENTER + "Thank you for your purchase!\n";
            r += CENTER + "Powered by StoreManager POS\n\n\n";

            // Auto Cut
            r += CUT;

            return r;
        }

         
    }
}
