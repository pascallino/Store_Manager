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
    public partial class frmSales : Form
    {
        public frmSales()
        {
            InitializeComponent();
            // wire events
            txtSearch.KeyDown += txtSearch_KeyDown;
            dgvCart.CellEndEdit += dgvCart_CellEndEdit;
            dgvCart.CellValueChanged += dgvCart_CellValueChanged;
            dgvCart.CurrentCellDirtyStateChanged += dgvCart_CurrentCellDirtyStateChanged;
            btnCheckout.Click += btnCheckout_Click;
            btnClearCart.Click += btnClearCart_Click;

        }

        // REQUIRED controls assumed on your form:
        // txtSearch (TextBox) - where user types barcode or item name
        // dgvCart (DataGridView) - cart grid
        // lblGrandTotal (Label) - shows page total
        // btnCheckout (Button) - finalise sale / print
        // btnClearCart (Button) - clears the cart
        // txtCashReceived, lblBalance (optional) - payment fields
        //
        // Also assumes DB.GetCon() exists and returns SqlConnection

        // ---------- Form-level fields ----------
        private void frmSales_Load(object sender, EventArgs e)
        {
            SetupCartGrid();

            // wire events
            txtSearch.KeyDown += txtSearch_KeyDown;
            dgvCart.CellEndEdit += dgvCart_CellEndEdit;
            dgvCart.CellValueChanged += dgvCart_CellValueChanged;
            dgvCart.CurrentCellDirtyStateChanged += dgvCart_CurrentCellDirtyStateChanged;
            btnCheckout.Click += btnCheckout_Click;
            btnClearCart.Click += btnClearCart_Click;

            UpdateGrandTotal();
        }

        // ---------- Grid setup ----------
        private void SetupCartGrid()
        {
            dgvCart.AllowUserToAddRows = false;
            dgvCart.Columns.Clear();

            // Hidden ItemID
            DataGridViewTextBoxColumn idCol = new DataGridViewTextBoxColumn();
            idCol.Name = "ItemID";
            idCol.HeaderText = "ItemID";
            idCol.Visible = false;
            dgvCart.Columns.Add(idCol);

            // Item Name
            DataGridViewTextBoxColumn nameCol = new DataGridViewTextBoxColumn();
            nameCol.Name = "ItemName";
            nameCol.HeaderText = "Item";
            nameCol.ReadOnly = true;
            nameCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvCart.Columns.Add(nameCol);

            // Barcode (hidden)
            DataGridViewTextBoxColumn bcCol = new DataGridViewTextBoxColumn();
            bcCol.Name = "Barcode";
            bcCol.HeaderText = "Barcode";
            bcCol.Visible = false;
            dgvCart.Columns.Add(bcCol);

            // Unit Price
            DataGridViewTextBoxColumn priceCol = new DataGridViewTextBoxColumn();
            priceCol.Name = "UnitPrice";
            priceCol.HeaderText = "Unit price";
            priceCol.ReadOnly = true;
            priceCol.DefaultCellStyle.Format = "N2";
            priceCol.Width = 80;
            dgvCart.Columns.Add(priceCol);

            // PerCarton (hidden, used for calculations)
            DataGridViewTextBoxColumn perCartonCol = new DataGridViewTextBoxColumn();
            perCartonCol.Name = "PerCarton";
            perCartonCol.HeaderText = "PerCarton";
            perCartonCol.Visible = false;
            dgvCart.Columns.Add(perCartonCol);

            // Cartons (ComboBox)
            DataGridViewComboBoxColumn cartonCol = new DataGridViewComboBoxColumn();
            cartonCol.Name = "CartonQty";
            cartonCol.HeaderText = "Cartons";
            // reasonable default options; adjust as needed
            for (int i = 0; i <= 50; i++) cartonCol.Items.Add(i.ToString());
            cartonCol.Width = 60;
            dgvCart.Columns.Add(cartonCol);

            // Units (editable text cell)
            DataGridViewTextBoxColumn unitCol = new DataGridViewTextBoxColumn();
            unitCol.Name = "UnitQty";
            unitCol.HeaderText = "Units";
            unitCol.Width = 60;
            dgvCart.Columns.Add(unitCol);

            // Total (read-only)
            DataGridViewTextBoxColumn totalCol = new DataGridViewTextBoxColumn();
            totalCol.Name = "LineTotal";
            totalCol.HeaderText = "Line Total";
            totalCol.ReadOnly = true;
            totalCol.DefaultCellStyle.Format = "N2";
            totalCol.Width = 90;
            dgvCart.Columns.Add(totalCol);
        }

        // ---------- Search handling (barcode or name) ----------
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string s = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(s))
                {
                    LoadItemBySearch(s);
                    txtSearch.SelectAll();
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void LoadItemBySearch(string search)
        {
            try
            {
                using (SqlConnection con = DB.GetCon())
                {
                    con.Open();

                    string query = @"
                SELECT TOP 1 ItemID, ItemName, Barcode, Quantity AS UnitsInStock,
                       Carton_Quantity AS CartonsInStock, Per_Carton_Quantity, Quantity_SP AS UnitPrice
                FROM Items
                WHERE Barcode = @s OR ItemName LIKE @s + '%'
            ";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@s", search);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                string itemId = dr["ItemID"].ToString();
                                string name = dr["ItemName"].ToString();
                                string barcode = dr["Barcode"].ToString();
                                decimal unitPrice = dr["UnitPrice"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["UnitPrice"]);
                                int perCarton = dr["Per_Carton_Quantity"] == DBNull.Value ? 1 : Convert.ToInt32(dr["Per_Carton_Quantity"]);

                                AddItemToCart(itemId, name, barcode, unitPrice, perCarton);
                            }
                            else
                            {
                                // not found
                                MessageBox.Show("Item not found.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search error: " + ex.Message);
            }
        }

        // ---------- Add item to cart (merge if exists) ----------
        private void AddItemToCart(string itemId, string name, string barcode, decimal unitPrice, int perCarton)
        {
            // find existing row by ItemID
            foreach (DataGridViewRow row in dgvCart.Rows)
            {
                if (row.Cells["ItemID"].Value?.ToString() == itemId)
                {
                    // Increase units by 1 (scanner behavior) and update
                    int units = 0;
                    int.TryParse(row.Cells["UnitQty"].Value?.ToString() ?? "0", out units);
                    units += 1;
                    row.Cells["UnitQty"].Value = units.ToString();
                    UpdateRowTotal(row);
                    UpdateGrandTotal();
                    return;
                }
            }

            // add new row
            int idx = dgvCart.Rows.Add();
            var newRow = dgvCart.Rows[idx];

            newRow.Cells["ItemID"].Value = itemId;
            newRow.Cells["ItemName"].Value = name;
            newRow.Cells["Barcode"].Value = barcode;
            newRow.Cells["UnitPrice"].Value = unitPrice;
            newRow.Cells["PerCarton"].Value = perCarton.ToString();

            // default quantities
            newRow.Cells["CartonQty"].Value = "0";
            newRow.Cells["UnitQty"].Value = "1";

            UpdateRowTotal(newRow);
            UpdateGrandTotal();
        }

        // ---------- Update totals ----------
        private void UpdateRowTotal(DataGridViewRow row)
        {
            // parse values safely
            int cartons = 0;
            int units = 0;
            decimal unitPrice = 0m;
            int perCarton = 1;

            // Cartons (combo) may be string
            if (row.Cells["CartonQty"].Value != null)
                int.TryParse(row.Cells["CartonQty"].Value.ToString(), out cartons);

            if (row.Cells["UnitQty"].Value != null)
                int.TryParse(row.Cells["UnitQty"].Value.ToString(), out units);

            if (row.Cells["UnitPrice"].Value != null)
                decimal.TryParse(row.Cells["UnitPrice"].Value.ToString(), out unitPrice);

            if (row.Cells["PerCarton"].Value != null)
                int.TryParse(row.Cells["PerCarton"].Value.ToString(), out perCarton);

            // compute total units first: cartons->units + units
            int totalUnits = (cartons * perCarton) + units;

            decimal lineTotal = totalUnits * unitPrice;
            row.Cells["LineTotal"].Value = lineTotal;

            // optional: you might want to store total units in a hidden cell if needed
            // row.Cells["TotalUnits"].Value = totalUnits;
        }
        private void UpdateGrandTotal()
        {
            decimal grand = 0m;

            foreach (DataGridViewRow r in dgvCart.Rows)
            {
                decimal lineTotal = 0m;  // Declare once per row

                if (r.Cells["LineTotal"].Value != null)
                {
                    decimal.TryParse(r.Cells["LineTotal"].Value.ToString(), out lineTotal);
                }

                grand += lineTotal;
            }

            lblGrandTotal.Text = grand.ToString("N2");
        }


        // ---------- Events to handle edits and combos ----------
        private void dgvCart_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // needed so combo changes commit immediately
            if (dgvCart.IsCurrentCellDirty)
                dgvCart.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvCart_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var colName = dgvCart.Columns[e.ColumnIndex].Name;

            // If CartonQty or UnitQty changed, update totals
            if (colName == "CartonQty" || colName == "UnitQty")
            {
                var row = dgvCart.Rows[e.RowIndex];
                // validate non-negative numeric
                if (colName == "UnitQty")
                {
                    if (!int.TryParse(row.Cells["UnitQty"].Value?.ToString() ?? "0", out int val) || val < 0)
                    {
                        MessageBox.Show("Units must be a non-negative number.");
                        row.Cells["UnitQty"].Value = "0";
                    }
                }

                UpdateRowTotal(row);
                UpdateGrandTotal();
            }
        }

        private void dgvCart_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // When editing finishes, ensure value is numeric (for UnitQty)
            if (e.RowIndex < 0) return;

            string col = dgvCart.Columns[e.ColumnIndex].Name;
            var row = dgvCart.Rows[e.RowIndex];

            if (col == "UnitQty")
            {
                if (!int.TryParse(row.Cells["UnitQty"].Value?.ToString() ?? "0", out int v) || v < 0)
                {
                    MessageBox.Show("Please enter a valid non-negative number for Units.");
                    row.Cells["UnitQty"].Value = "0";
                }
                UpdateRowTotal(row);
                UpdateGrandTotal();
            }
        }

        // ---------- Clear / Remove ----------
        private void btnClearCart_Click(object sender, EventArgs e)
        {
            dgvCart.Rows.Clear();
            UpdateGrandTotal();
        }

        // optional: remove a single selected row
        private void RemoveSelectedRow()
        {
            if (dgvCart.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow r in dgvCart.SelectedRows)
                    dgvCart.Rows.Remove(r);

                UpdateGrandTotal();
            }
        }

        // ---------- Checkout (sample) ----------
        private void btnCheckout_Click(object sender, EventArgs e)
        {
            if (dgvCart.Rows.Count == 0)
            {
                MessageBox.Show("Cart is empty.");
                return;
            }

            // build invoice data
            string itemsList = "";
            decimal total = 0m;
            foreach (DataGridViewRow row in dgvCart.Rows)
            {
                string nm = row.Cells["ItemName"].Value.ToString();
                int cartons = int.TryParse(row.Cells["CartonQty"].Value?.ToString(), out int c) ? c : 0;
                int units = int.TryParse(row.Cells["UnitQty"].Value?.ToString(), out int u) ? u : 0;
                int perCarton = int.TryParse(row.Cells["PerCarton"].Value?.ToString(), out int p) ? p : 1;
                decimal price = decimal.TryParse(row.Cells["UnitPrice"].Value?.ToString(), out decimal pr) ? pr : 0m;

                int totalUnits = (cartons * perCarton) + units;
                decimal line = totalUnits * price;
                itemsList += $"{nm.PadRight(18).Substring(0, Math.Min(18, nm.Length))} x{totalUnits} ₦{line:n2}\n";
                total += line;

                // OPTIONAL: Update DB stock here per sold items (deduct). Be careful: transaction recommended.
            }

            // show total and optionally print
            lblGrandTotal.Text = total.ToString("N2");

            // example print call (ensure RawPrinterHelper + PrintReceipt exist)
            // PrintReceipt("YourPrinterName", invoiceNo, cashier, itemsList, total, paid, balance);

            MessageBox.Show("Checkout done. Implement DB write & printing as needed.");
            // after successful sale:
            // btnClearCart_Click(null, null);
        }

    }
}
