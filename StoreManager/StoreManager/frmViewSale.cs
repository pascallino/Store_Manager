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
    public partial class frmViewSale : Form
    {
        public frmViewSale(string invoice, string receipt, string table)
        {
            InitializeComponent();
            LoadSaleItems(invoice, receipt, table);
            StyleGrid();
        }
        private void LoadSaleItems(string invoice, string receipt, string table)
        {
            try
            {
                using (SqlConnection con = DB.GetCon())
                {
                    con.Open();

                    string query = $@"
                SELECT 
                    i.ItemName,
                    s.Carton_Qty,
                    i.Carton_SP AS CartonPrice,
                    s.Units,
                    i.Quantity_SP AS UnitPrice,
                    s.Subtotal
                FROM {table}  s
                JOIN Items i ON s.ItemID = i.ItemID
                WHERE s.Invoice_No = @inv AND s.Receipt_No = @rec
            ";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@inv", invoice);
                    cmd.Parameters.AddWithValue("@rec", receipt);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvItems.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading items: " + ex.Message);
            }
        }

        private void StyleGrid()
        {
            // Remove borders + smooth look
            dgvItems.BorderStyle = BorderStyle.None;
            dgvItems.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvItems.RowHeadersVisible = false;

            // Header Style
            dgvItems.EnableHeadersVisualStyles = false;
            dgvItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);   // Blue
            dgvItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            dgvItems.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvItems.ColumnHeadersHeight = 40;

            // Row Style
            dgvItems.DefaultCellStyle.BackColor = Color.White;
            dgvItems.DefaultCellStyle.ForeColor = Color.Black;
            dgvItems.DefaultCellStyle.Font = new Font("Segoe UI", 11);
            dgvItems.DefaultCellStyle.Padding = new Padding(5);
            dgvItems.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 240, 255);
            dgvItems.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Alternate row color
            dgvItems.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);

            // Fit columns to content — BEST LOOK
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            // Rows scale with content height
            dgvItems.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Prevent ugly long-height rows
            dgvItems.RowTemplate.Height = 35;
            dgvItems.AllowUserToResizeRows = false;

            // Improve readability
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Smooth scrollbar
            dgvItems.ScrollBars = ScrollBars.Both;

            // Prevent column text from cutting off
            dgvItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
        }



        private void frmViewSale_Load(object sender, EventArgs e)
        {

        }
    }
}
