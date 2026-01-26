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
        private int editmode = 0;
        private int _currentId;
        public frmSales(int Id)
        {
            InitializeComponent();
            _currentId = Id;
            // wire events
            txtSearch.KeyDown += txtSearch_KeyDown;
            dgvCart.CellEndEdit += dgvCart_CellEndEdit;
            //dgvCart.CellValueChanged += dgvCart_CellValueChanged;
            dgvCart.CurrentCellDirtyStateChanged += dgvCart_CurrentCellDirtyStateChanged;
            btnCheckout.Click += btnCheckout_Click;
            btnClearCart.Click += btnClearCart_Click;
            dgvCart.CellClick += dgvCart_CellClick;
            txtCashReceived.TextChanged += txtCashReceived_TextChanged;
            btnDeleteTransaction.Click += btnDeleteTransaction_Click;
            lstSuggestions.Click += lstSuggestions_Click;
            dgvCart.Leave += dgvCart_Leave;
            dgvCart.MouseLeave += dgvCart_MouseLeave;



        }
        private void StyleGrid()
        {
            // Remove ugly borders
            dgvCart.BorderStyle = BorderStyle.None;
            dgvCart.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvCart.RowHeadersVisible = false;

            // Header style
            dgvCart.EnableHeadersVisualStyles = false;
            dgvCart.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 144, 255);
            dgvCart.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCart.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            dgvCart.ColumnHeadersHeight = 35;

            // Row style
            dgvCart.DefaultCellStyle.BackColor = Color.White;
            dgvCart.DefaultCellStyle.ForeColor = Color.Black;
            dgvCart.DefaultCellStyle.Font = new Font("Segoe UI", 22);
            dgvCart.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 240, 255);
            dgvCart.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Alternate row color
            dgvCart.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);

            // Auto-size
            dgvCart.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCart.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvCart.AllowUserToResizeRows = false;

            // Full row select
            dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
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
            StyleGrid();


            UpdateGrandTotal();
        }

        private bool IsCurrentUserAdmin()
        {
            using (SqlConnection con = DB.GetCon()) // or GetCon()
            using (SqlCommand cmd = new SqlCommand(
                "SELECT UserType FROM Users WHERE UserID = @ID", con))
            {
                cmd.Parameters.AddWithValue("@ID", _currentId);

                con.Open();
                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    return false;

                string userType = result.ToString().Trim();
                return userType == "Admin";
            }
        }


        // ---------- Grid setup ----------
        private void SetupCartGrid()
        {
            dgvCart.AllowUserToAddRows = false;
            dgvCart.Columns.Clear();

            DataGridViewTextBoxColumn colSaleID = new DataGridViewTextBoxColumn();
            colSaleID.Name = "SaleID";
            colSaleID.DataPropertyName = "SaleID"; // matches your data source field
            colSaleID.Visible = false;             // hide column
            dgvCart.Columns.Add(colSaleID);

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
            MessageBox.Show(IsCurrentUserAdmin().ToString());
            MessageBox.Show(_currentId.ToString());
            if (IsCurrentUserAdmin())
            {
                priceCol.ReadOnly = false;
            }
            else
            {
                priceCol.ReadOnly = true;
            }
            priceCol.DefaultCellStyle.Format = "N2";
            priceCol.Width = 80;
            dgvCart.Columns.Add(priceCol);

            // PerCarton (hidden, used for calculations)
            DataGridViewTextBoxColumn perCartonCol = new DataGridViewTextBoxColumn();
            perCartonCol.Name = "PerCarton";
            perCartonCol.HeaderText = "PerCarton";
            perCartonCol.Visible = false;
            dgvCart.Columns.Add(perCartonCol);

            // Carton Price (read-only)
            DataGridViewTextBoxColumn cartonPriceCol = new DataGridViewTextBoxColumn();
            cartonPriceCol.Name = "CartonPrice";
            cartonPriceCol.HeaderText = "Carton Price";
            if (IsCurrentUserAdmin())
            {
                cartonPriceCol.ReadOnly = false;
            }
            else
            {
                cartonPriceCol.ReadOnly = true;
            }
            cartonPriceCol.DefaultCellStyle.Format = "N2";
            cartonPriceCol.Width = 80;
            dgvCart.Columns.Add(cartonPriceCol);

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

            // Remove Button Column
            DataGridViewButtonColumn removeCol = new DataGridViewButtonColumn();
            removeCol.Name = "Remove";
            removeCol.HeaderText = "Remove";
            removeCol.Text = "Remove";
            removeCol.UseColumnTextForButtonValue = true;
            removeCol.Width = 70;
            dgvCart.Columns.Add(removeCol);


            // ✅ Set EditMode here
            dgvCart.EditMode = DataGridViewEditMode.EditOnEnter;

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

        // ---------- Search handling (barcode or name) ----------



        private void LoadItemBySearch(string search)
        {
            try
            {
                using (SqlConnection con = DB.GetCon())
                {
                    con.Open();

                    string query = @"
                SELECT TOP 1 ItemID, ItemName, Barcode, Quantity AS UnitsInStock, Carton_SP,
                       Carton_Quantity AS CartonsInStock, Per_Carton_Quantity, Total_quantity, Quantity_SP AS UnitPrice
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
                                decimal cartonPrice = dr["Carton_SP"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["Carton_SP"]);
                                decimal unitPrice = dr["UnitPrice"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["UnitPrice"]);
                                int perCarton = dr["Per_Carton_Quantity"] == DBNull.Value ? 1 : Convert.ToInt32(dr["Per_Carton_Quantity"]);
                                int totalQty = dr["Total_Quantity"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Total_Quantity"]);

                                // --------------------------------------------------------
                                // 🛑 STOCK VALIDATION USING ONLY Total_Quantity
                                // --------------------------------------------------------
                                if (totalQty <= 0)
                                {
                                    MessageBox.Show(
                                        $"Item '{name}' is OUT OF STOCK!",
                                        "Stock",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning
                                    );
                                    return;
                                }

                                AddItemToCart(itemId, name, barcode, unitPrice, perCarton, cartonPrice);
                                txtSearch.Text = "";
                            }
                            else
                            {
                                return;
                                // not found
                                //     MessageBox.Show("Item not found.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        private void UpdateBalance()
        {
            if (decimal.TryParse(txtCashReceived.Text, out decimal cashReceived) &&
                decimal.TryParse(lblGrandTotal.Text, out decimal grandTotal) &&
                cashReceived != 0)
            {
                decimal balance = cashReceived - grandTotal;
                lblBalance.Text = "Balance: " + balance.ToString("N2");
            }
            else
            {
                lblBalance.Text = "Balance: 0.00";
            }
        }

        private void dgvCart_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure user clicked on a valid row and the Remove column
            if (e.RowIndex >= 0 && dgvCart.Columns[e.ColumnIndex].Name == "Remove")
            {
                dgvCart.Rows.RemoveAt(e.RowIndex);
                UpdateGrandTotal();  // Deduct the removed item's total from grand total
                UpdateBalance();  // recalc balance if cash already entered
            }
        }

        private int GetItemStock(string itemId)
        {
            using (SqlConnection con = DB.GetCon())
            {
                con.Open();
                string query = "SELECT Total_Quantity FROM Items WHERE ItemID = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", itemId);

                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                    return Convert.ToInt32(result);
                else
                    return 0;
            }
        }


        // ---------- Add item to cart (merge if exists) ----------
        private void AddItemToCart(string itemId, string name, string barcode, decimal unitPrice, int perCarton, decimal cartonPrice)
        {
            // 1️⃣ Check item exists and has stock
            int totalQty = GetItemStock(itemId);
            if (totalQty <= 0)
            {
                MessageBox.Show($"{name} is OUT OF STOCK!", "Stock Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // check if item already exists in the cart
            foreach (DataGridViewRow row in dgvCart.Rows)
            {
                if (row.Cells["ItemID"].Value?.ToString() == itemId)
                {
                    // item already in cart, ignore adding
                    return; // just exit the method
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
            newRow.Cells["CartonPrice"].Value = cartonPrice; // from DB: i.Carton_SP
            // default quantities
            int cartonQty = 0;
            var comboCell = (DataGridViewComboBoxCell)newRow.Cells["CartonQty"];

            // Ensure the value exists in ComboBox items
            if (!comboCell.Items.Contains(cartonQty))
            {
                comboCell.Items.Add(cartonQty);
            }

            // Assign value
            comboCell.Value = cartonQty;
            //  newRow.Cells["CartonQty"].Value = "0";
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
            decimal cartonPrice = 0m;

            // Cartons (combo) may be string
            if (row.Cells["CartonQty"].Value != null)
                int.TryParse(row.Cells["CartonQty"].Value.ToString(), out cartons);

            if (row.Cells["UnitQty"].Value != null)
                int.TryParse(row.Cells["UnitQty"].Value.ToString(), out units);

            if (row.Cells["UnitPrice"].Value != null)
                decimal.TryParse(row.Cells["UnitPrice"].Value.ToString(), out unitPrice);

            if (row.Cells["PerCarton"].Value != null)
                int.TryParse(row.Cells["PerCarton"].Value.ToString(), out perCarton);

            decimal.TryParse(row.Cells["CartonPrice"].Value?.ToString(), out cartonPrice);

            // compute total units first: cartons->units + units
            int totalUnits = (cartons * perCarton) + units;

            // Line total = (cartons × carton price) + (units × unit price)
            decimal lineTotal = (cartons * cartonPrice) + (units * unitPrice);
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
            lblGrandTotal2.Text = grand.ToString("N2");
        }


        // ---------- Events to handle edits and combos ----------
        private void dgvCart_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // needed so combo changes commit immediately
            if (dgvCart.IsCurrentCellDirty)
                dgvCart.CommitEdit(DataGridViewDataErrorContexts.Commit);

        }
        private void dgvCart_MouseLeave(object sender, EventArgs e)
        {
            if (dgvCart.IsCurrentCellDirty)
            {
                dgvCart.EndEdit(); // Forces commit
            }
        }
        private void dgvCart_Leave(object sender, EventArgs e)
        {
            dgvCart.EndEdit();
        }

       
        private bool isUpdating = false;

        //private void dgvCart_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (isUpdating) return; // skip if updating programmatically
        //    if (e.RowIndex < 0) return;

        //    string colName = dgvCart.Columns[e.ColumnIndex].Name;
        //    if (colName != "CartonQty" && colName != "UnitQty") return;

        //    DataGridViewRow row = dgvCart.Rows[e.RowIndex];
        //    string itemId = row.Cells["ItemID"].Value?.ToString();
        //    if (string.IsNullOrEmpty(itemId)) return;

        //    int perCarton = int.TryParse(row.Cells["PerCarton"].Value?.ToString(), out int p) ? p : 1;
        //    int cartons = int.TryParse(row.Cells["CartonQty"].Value?.ToString(), out int c) ? c : 0;
        //    int units = int.TryParse(row.Cells["UnitQty"].Value?.ToString(), out int u) ? u : 0;
        //    if (cartons < 0) cartons = 0;
        //    if (units < 0) units = 0;

        //    row.Cells["CartonQty"].Value = cartons;
        //    row.Cells["UnitQty"].Value = units;

        //    int totalRequested = (cartons * perCarton) + units;
        //    int stock = GetItemStock(itemId);

        //    isUpdating = true; // prevent reentry

        //    if (editmode == 0)
        //    {
        //        if (totalRequested > stock)
        //        {
        //            MessageBox.Show(
        //                $"Out of Stock!\nYou requested {totalRequested}, but only {stock} available.",
        //                "Stock Limit", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //            row.Cells["CartonQty"].Value = 0;
        //            row.Cells["UnitQty"].Value = 0;
        //        }
        //    }
        //    else if (editmode == 1)
        //    {
        //        var saleId = row.Cells["SaleID"].Value?.ToString();
        //        if (!string.IsNullOrEmpty(saleId))
        //        {
        //            int oldCarton = getoldcarton(saleId);
        //            int oldUnit = getoldunit(saleId);

        //            int delta = totalRequested - ((oldCarton * perCarton) + oldUnit);
        //            if (delta > 0 && stock < delta)
        //            {
        //                MessageBox.Show(
        //                    $"Out of Stock!\nYou requested extra {delta}, but only {stock} available.",
        //                    "Stock Limit", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //                row.Cells["CartonQty"].Value = oldCarton;
        //                row.Cells["UnitQty"].Value = oldUnit;
        //            }
        //        }
        //    }

        //    isUpdating = false; // allow normal handling again

        //    UpdateRowTotal(row);
        //    UpdateGrandTotal();
        //    UpdateBalance();
        //}
        private void dgvCart_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = dgvCart.Columns[e.ColumnIndex].Name;
            // 🔑 Trigger on qty OR price change
            if (colName != "CartonQty" &&
                colName != "UnitQty" &&
                colName != "UnitPrice" &&
                colName != "CartonPrice")
                return;

            DataGridViewRow row = dgvCart.Rows[e.RowIndex];

            string itemId = row.Cells["ItemID"].Value?.ToString();
            if (string.IsNullOrEmpty(itemId)) return;

            // --- Parse quantities safely ---
            int cartons = int.TryParse(row.Cells["CartonQty"].Value?.ToString(), out int c) ? c : 0;
           
            var comboCell = (DataGridViewComboBoxCell)row.Cells["CartonQty"];

            // Ensure the value exists in ComboBox items
            if (!comboCell.Items.Contains(cartons))
            {
                comboCell.Items.Add(cartons);
            }

            // Assign value
            comboCell.Value = cartons;
            int units = int.TryParse(row.Cells["UnitQty"].Value?.ToString(), out int u) ? u : 0;

            if (cartons < 0) cartons = 0;
            if (units < 0) units = 0;

            row.Cells["CartonQty"].Value = cartons;
            row.Cells["UnitQty"].Value = units;

            // --- Get per carton qty ---
            int perCarton = int.TryParse(row.Cells["PerCarton"].Value?.ToString(), out int p) ? p : 1;

            int totalRequested = (cartons * perCarton) + units;
            int stock = GetItemStock(itemId);

            // ==============================
            // NEW SALE MODE
            // ==============================
            if (editmode == 0)
            {
                if (totalRequested > stock)
                {
                    MessageBox.Show(
                        $"Out of Stock!\nRequested: {totalRequested}\nAvailable: {stock}",
                        "Stock Limit",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    row.Cells["CartonQty"].Value = 0;
                    row.Cells["UnitQty"].Value = 0;
                }
            }
            // ==============================
            // EDIT / REFUND MODE
            // ==============================
            else
            {
                var saleId = row.Cells["SaleID"].Value?.ToString();
                if (!string.IsNullOrEmpty(saleId))
                {
                    int oldCarton = getoldcarton(saleId);
                    int oldUnit = getoldunit(saleId);

                    int oldTotal = (oldCarton * perCarton) + oldUnit;
                    int delta = totalRequested - oldTotal;

                    if (delta > 0 && stock < delta)
                    {
                        MessageBox.Show(
                            $"Out of Stock!\nExtra requested: {delta}\nAvailable: {stock}",
                            "Stock Limit",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        row.Cells["CartonQty"].Value = oldCarton;
                        row.Cells["UnitQty"].Value = oldUnit;
                    }
                }
            }

            // --- Always recalc ---
            UpdateRowTotal(row);
            UpdateGrandTotal();
            UpdateBalance();
        }



        // Get old carton quantity for a sale
        private int getoldcarton(string saleId)
        {
            int oldCarton = 0;
            string query = "SELECT Carton_Qty FROM Sales WHERE SaleID = @SaleID";

            using (SqlConnection con = DB.GetCon())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SaleID", saleId);
                    con.Open();
                    var result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int c))
                    {
                        oldCarton = c;
                    }
                }
            }

            return oldCarton;
        }

        // Get old unit quantity for a sale
        private int getoldunit(string saleId)
        {
            int oldUnit = 0;
            string query = "SELECT Units FROM Sales WHERE SaleID = @SaleID";

            using (SqlConnection con = DB.GetCon())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SaleID", saleId);
                    con.Open();
                    var result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int u))
                    {
                        oldUnit = u;
                    }
                }
            }

            return oldUnit;
        }


        //private void dgvCart_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        //{
        //    // When editing finishes, ensure value is numeric (for UnitQty)
        //    if (e.RowIndex < 0) return;

        //    string col = dgvCart.Columns[e.ColumnIndex].Name;
        //    var row = dgvCart.Rows[e.RowIndex];

        //    if (col == "UnitQty")
        //    {
        //        if (!int.TryParse(row.Cells["UnitQty"].Value?.ToString() ?? "0", out int v) || v < 0)
        //        {
        //            MessageBox.Show("Please enter a valid non-negative number for Units.");
        //            row.Cells["UnitQty"].Value = "0";
        //        }
        //        UpdateRowTotal(row);
        //        UpdateGrandTotal();
        //        UpdateBalance();
        //    }
        //}

        // ---------- Clear / Remove ----------
        private void btnClearCart_Click(object sender, EventArgs e)
        {
            dgvCart.Rows.Clear();
            txtSearch.Text = "";
            txtSearch_recipt_invoice.Text = "";
            lblBalance.Text = "Balance: 0.00";
            txtCashReceived.Text = "";
            lstSuggestions.Items.Clear();
            lstSuggestions.Visible = false;
            txtSearchItemName.Clear();
            txtSearch_recipt_invoice.Clear();
            UpdateGrandTotal();
            editmode = 0;
            resettxtsearchrev();
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

        private bool ValidateCartStock()
        {
            foreach (DataGridViewRow row in dgvCart.Rows)
            {
                if (row.IsNewRow) continue;

                string itemId = row.Cells["ItemID"].Value?.ToString();
                string itemName = row.Cells["ItemName"].Value?.ToString();

                if (string.IsNullOrEmpty(itemId)) continue;

                int cartons = int.TryParse(row.Cells["CartonQty"].Value?.ToString(), out int c) ? c : 0;
                int units = int.TryParse(row.Cells["UnitQty"].Value?.ToString(), out int u) ? u : 0;
                int perCarton = int.TryParse(row.Cells["PerCarton"].Value?.ToString(), out int p) ? p : 1;

                int totalRequested = (cartons * perCarton) + units;

                if (totalRequested <= 0)
                {
                    MessageBox.Show(
                        $"Invalid quantity for item: {itemName}",
                        "Quantity Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return false;
                }

                int stockLeft = GetItemStock(itemId);

                // 🔴 FINAL SAFETY CHECK
                if (stockLeft - totalRequested < 0)
                {
                    MessageBox.Show(
                        $"Insufficient stock for '{itemName}'.\n" +
                        $"Available: {stockLeft}\n" +
                        $"Requested: {totalRequested}",
                        "Stock Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return false;
                }
            }

            return true; // ✅ ALL ROWS PASSED
        }


        // ---------- Checkout (sample) ----------
        private void btnCheckout_Click(object sender, EventArgs e)
        {
            if (dgvCart.Rows.Count == 0)
            {
                MessageBox.Show("Cart is empty.");
                return;
            }
            // 🔒 FINAL STOCK VALIDATION
            if (!ValidateCartStock())
            {
                return; // STOP checkout immediately
            }

            Random rnd = new Random();
            string suffix = rnd.Next(100, 999).ToString();

            string invoiceNo = "INV-" + DateTime.Now.ToString("yyMMddHHmmss") + "-" + suffix;
            string receiptNo = "RCPT-" + DateTime.Now.ToString("yyMMddHHmmss") + "-" + suffix;


            decimal cashReceived = 0m;
            decimal Grandtotal = 0m;
            decimal.TryParse(txtCashReceived.Text, out cashReceived);
            decimal.TryParse(lblGrandTotal.Text, out Grandtotal);

            if (Grandtotal <= 0)
            {
                return;
            }
            if (cashReceived <= 0)
            {
                MessageBox.Show("Please Enter the cash Received from the customer");
                txtCashReceived.Focus();
                return;
            }

            decimal subtotal = 0m;
            string itemsList = "";

            using (SqlConnection con = DB.GetCon())
            {
                con.Open();

                using (SqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        foreach (DataGridViewRow row in dgvCart.Rows)
                        {
                            string itemId = row.Cells["ItemID"].Value.ToString();
                            string name = row.Cells["ItemName"].Value.ToString();
                            int cartons = int.Parse(row.Cells["CartonQty"].Value?.ToString() ?? "0");
                            int units = int.Parse(row.Cells["UnitQty"].Value?.ToString() ?? "0");
                            int perCarton = int.Parse(row.Cells["PerCarton"].Value?.ToString() ?? "1");
                            decimal cartonPrice = decimal.Parse(row.Cells["CartonPrice"].Value.ToString());
                            decimal unitPrice = decimal.Parse(row.Cells["UnitPrice"].Value.ToString());
                            int totalUnits = (cartons * perCarton) + units;
                            decimal lineTotal = (cartons * cartonPrice) + (units * unitPrice);
                            subtotal += lineTotal;

                            // Build printing line
                            itemsList += $"{name,-18} x{totalUnits,-4} NGN{lineTotal,10:n2}\n";

                            // SAVE TO DB
                            SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO Sales (Invoice_No, Receipt_No, ItemID, Carton_Qty, Carton_Sp, Units, Unit_Sp, Total_Qty, Subtotal, Cash_Received, Balance, Date_Sold, SoldBy)
                        VALUES (@inv, @rcp, @item, @carton, @carton_sp, @units, @units_sp, @totalqty,  @subtotal, @cash, @bal, GETDATE(), @SoldBy)", con, tran);

                            cmd.Parameters.AddWithValue("@inv", invoiceNo);
                            cmd.Parameters.AddWithValue("@rcp", receiptNo);
                            cmd.Parameters.AddWithValue("@item", itemId);
                            cmd.Parameters.AddWithValue("@carton", cartons);
                            cmd.Parameters.AddWithValue("@carton_sp", cartonPrice);
                            cmd.Parameters.AddWithValue("@units", units);
                            cmd.Parameters.AddWithValue("@units_sp", unitPrice);
                            cmd.Parameters.AddWithValue("@totalqty", totalUnits);
                            cmd.Parameters.AddWithValue("@subtotal", lineTotal);
                            cmd.Parameters.AddWithValue("@cash", cashReceived);
                            cmd.Parameters.AddWithValue("@bal", cashReceived - subtotal);
                            cmd.Parameters.AddWithValue("@SoldBy", _currentId);

                            cmd.ExecuteNonQuery();






                            // OPTIONAL: Deduct stock here
                            //-----------------------------------
                            // DEDUCT STOCK FROM ITEMS TABLE
                            //-----------------------------------
                            SqlCommand cmdStock = new SqlCommand(@"
                            UPDATE Items 
                            SET Total_Quantity = Total_Quantity - @soldQty
                            WHERE ItemID = @item", con, tran);

                            cmdStock.Parameters.AddWithValue("@soldQty", totalUnits);
                            cmdStock.Parameters.AddWithValue("@item", itemId);

                            cmdStock.ExecuteNonQuery();
                        }
                        //save to sales summary
                        SqlCommand cmd2 = new SqlCommand(@"INSERT INTO Sales_Summary
                            (Invoice_No, Receipt_No, Subtotal, CashReceived, Balance, Date_Sold, SoldBy)
                            VALUES (@Invoice_No, @Receipt_No, @Subtotal, @CashReceived, @Balance, GETDATE(), @SoldBy)", con, tran);


                        cmd2.Parameters.AddWithValue("@Invoice_No", invoiceNo);
                        cmd2.Parameters.AddWithValue("@Receipt_No", receiptNo);
                        cmd2.Parameters.AddWithValue("@Subtotal", subtotal);
                        cmd2.Parameters.AddWithValue("@CashReceived", cashReceived);
                        cmd2.Parameters.AddWithValue("@Balance", cashReceived - subtotal);
                        cmd2.Parameters.AddWithValue("@SoldBy", _currentId);

                        cmd2.ExecuteNonQuery();

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        MessageBox.Show("Error saving sale: " + ex.Message);
                        return;
                    }
                }
            }

            decimal balance = cashReceived - subtotal;

            // Update UI
            lblGrandTotal.Text = subtotal.ToString("N2");
            lblGrandTotal2.Text = subtotal.ToString("N2");
            lblBalance.Text = balance.ToString("N2");

            // ---- PRINT RECEIPT ----
            string cashier = Sessiion.FullName; // OR your logged in user
            string receiptText = BuildReceipt(
                invoiceNo,
                cashier,
                itemsList,
                subtotal,
                cashReceived,
                balance
            );

            string printer = "XP-80C";
            RawPrinterHelper.SendStringToPrinter(printer, receiptText);

            MessageBox.Show("Checkout successful, saved & printed!");

            // Clear cart
            dgvCart.Rows.Clear();
            lblBalance.Text = "Balance: 0.00";
            txtCashReceived.Text = "";
            UpdateGrandTotal();
        }


        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string s = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(s))
            {
                LoadItemBySearch(s);
                
            }
        }
        private void txtCashReceived_TextChanged(object sender, EventArgs e)
        {
            UpdateBalance();  // recalc balance if cash already entered
        }


        private void dgvCart_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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
            // Set line spacing and font
            r += ESC + "!" + "\x00"; // normal font
            r += ESC + "2";          // default line spacing

            // Shop Name
            r += CENTER + DOUBLE_ON + BOLD_ON + "Amazing Super Store\n";
            r += DOUBLE_OFF + BOLD_OFF;
            r += CENTER + "Block 297 Abesan estate, akinyele bustop\n";
            r += CENTER + "Ipaja lagos\n";
            r += CENTER + "Tel: +2349112476966, +2349016829957 \n\n";

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
            r += RIGHT + $"Subtotal: NGN{subtotal:n2}\n";

            // Amount Paid
            r += RIGHT + $"Cash: NGN{cashReceived:n2}\n";

            // Balance (Double Size)
            r += DOUBLE_ON + RIGHT + $"Balance: NGN{balance:n2}\n";
            r += DOUBLE_OFF;

            r += "------------------------------------------\n";
            r += CENTER + "Thank you for your purchase!\n";
            r += CENTER + "Powered by Amazing Super Store POS\n\n\n";

            // --- Reset font & alignment ---
            r += ESC + "!" + "\x00";   // normal font
            r += ESC + "a" + "\x01";   // center

           

            // --- FEED PAPER (VERY IMPORTANT) ---
            r += "\n\n\n\n";          // feed 4 lines minimum

            // --- CUT AFTER FEED ---
            r += ESC + "i";            // full cut

            return r;
        }

        private void btnDeleteTransaction_Click(object sender, EventArgs e)
        {

            string search = txtSearch_recipt_invoice.Text.Trim();
            if (string.IsNullOrEmpty(search)) return;

            DialogResult dr = MessageBox.Show(
                "Are you sure you want to delete this transaction?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr != DialogResult.Yes) return;

            using (SqlConnection con = DB.GetCon())
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    // 🔹 0. LOG INTO DeletedSales (VERY IMPORTANT)
                    SqlCommand cmdLog = new SqlCommand(@"
                INSERT INTO DeletedSales
                (Invoice_No, Receipt_No, Subtotal, CashReceived, Balance, DeletedBy)
                SELECT 
                    Invoice_No, Receipt_No, Subtotal, CashReceived, Balance, @user
                FROM Sales_Summary
                WHERE Invoice_No = @search OR Receipt_No = @search
            ", con, tran);

                    cmdLog.Parameters.AddWithValue("@search", search);
                    cmdLog.Parameters.AddWithValue("@user", _currentId); // later use logged-in user
                    cmdLog.ExecuteNonQuery();

                    // 🔹 0b. LOG DELETED ITEMS (VERY IMPORTANT)
                    SqlCommand cmdLogItems = new SqlCommand(@"
                     INSERT INTO DeletedItems
                        (
                            Invoice_No, Receipt_No, ItemID,
                            Carton_Qty, Units, Total_Qty,
                            Subtotal, Cash_Received, Balance, Date_Sold,
                            DeletedBy
                        )
                        SELECT
                            Invoice_No, Receipt_No, ItemID,
                            Carton_Qty, Units, Total_Qty,
                            Subtotal, Cash_Received, Balance, Date_Sold,
                            @user
                        FROM Sales
                        WHERE Invoice_No = @search OR Receipt_No = @search;
                        ", con, tran);

                    cmdLogItems.Parameters.AddWithValue("@search", search);
                    cmdLogItems.Parameters.AddWithValue("@user", _currentId);
                    cmdLogItems.ExecuteNonQuery();


                    // 1. Restore stock first
                    SqlCommand cmdStock = new SqlCommand(@"
                UPDATE Items 
                SET Total_Quantity = Total_Quantity + s.Total_Qty
                FROM Sales s
                WHERE Items.ItemID = s.ItemID
                  AND (s.Invoice_No = @search OR s.Receipt_No = @search)
            ", con, tran);
                    cmdStock.Parameters.AddWithValue("@search", search);
                    cmdStock.ExecuteNonQuery();

                    // 2. Delete items from Sales table
                    SqlCommand cmdDelSales = new SqlCommand(@"
                DELETE FROM Sales 
                WHERE Invoice_No = @search OR Receipt_No = @search
            ", con, tran);
                    cmdDelSales.Parameters.AddWithValue("@search", search);
                    cmdDelSales.ExecuteNonQuery();

                    // 3. Delete record from Sales_Summary table
                    SqlCommand cmdDelSummary = new SqlCommand(@"
                DELETE FROM Sales_Summary 
                WHERE Invoice_No = @search OR Receipt_No = @search
            ", con, tran);
                    cmdDelSummary.Parameters.AddWithValue("@search", search);
                    cmdDelSummary.ExecuteNonQuery();

                    tran.Commit();

                    MessageBox.Show("Transaction deleted successfully! You can now modify items and checkout again.");
                    dgvCart.Rows.Clear();
                    btnDeleteTransaction.Visible = false;
                    btnUpdateTransaction.Visible = false;
                    txtSearch_recipt_invoice.Text = "";
                    lblGrandTotal.Text = "0.00";
                    lblGrandTotal2.Text = "0.00";
                    lblBalance.Text = "Balance: 0.00";
                    txtCashReceived.Text = ""; ;
                    UpdateGrandTotal();
                    UpdateBalance();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show("Error deleting transaction: " + ex.Message);
                }
            }


        }


        private void resettxtsearch()
        {
            label3.Visible = false;
            label2.Visible = false;
            txtSearchItemName.Visible = false;
            txtSearch.Visible = false;
        }
        private void resettxtsearchrev()
        {
            label3.Visible = true;
            label2.Visible = true;
            txtSearchItemName.Visible = true;
            txtSearch.Visible = true;
        }

        private void txtSearch_recipt_invoice_TextChanged(object sender, EventArgs e)
        {

            string search = txtSearch_recipt_invoice.Text.Trim();

            if (string.IsNullOrEmpty(search))
            {
                btnDeleteTransaction.Visible = false;
                btnUpdateTransaction.Visible = false;
                resettxtsearchrev();
                
                return;
            }
            resettxtsearch();

            // Check if this invoice/receipt exists
            using (SqlConnection con = DB.GetCon())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Sales_Summary WHERE Invoice_No=@n OR Receipt_No=@n", con);
                cmd.Parameters.AddWithValue("@n", search);
                int count = (int)cmd.ExecuteScalar();
                btnDeleteTransaction.Visible = count > 0; // show button if exists
                btnUpdateTransaction.Visible = count > 0; // show button if exists
            }

            // Optional: load items into dgvCart
            LoadSale(search);
        }
        private void LoadSale(string search)
        {
            try
            {
                dgvCart.Rows.Clear();

                using (SqlConnection con = DB.GetCon())
                {
                    con.Open();

                    string query = @"
                SELECT s.ItemID, i.ItemName, s.Carton_Qty, s.Units, s.Total_Qty, 
                       s.Unit_SP AS UnitPrice, s.Carton_SP AS CartonPrice, i.Per_Carton_Quantity,
                       s.Invoice_No, s.Receipt_No, s.Cash_Received, s.SaleID
                FROM Sales s
                JOIN Items i ON s.ItemID = i.ItemID
                WHERE s.Invoice_No = @search OR s.Receipt_No = @search";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@search", search);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            editmode = 1;
                            int idx = dgvCart.Rows.Add();
                            var row = dgvCart.Rows[idx];
                            row.Cells["SaleID"].Value = dr["SaleID"];
                            row.Cells["ItemID"].Value = dr["ItemID"];
                            row.Cells["ItemName"].Value = dr["ItemName"];
                            row.Cells["CartonPrice"].Value = dr["cartonprice"].ToString(); // from DB: i.Carton_SP
                            int cartonQty = dr["Carton_Qty"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Carton_Qty"]);
                            var comboCell = (DataGridViewComboBoxCell)row.Cells["CartonQty"];

                            // Ensure the value exists in ComboBox items
                            if (!comboCell.Items.Contains(cartonQty))
                            {
                                comboCell.Items.Add(cartonQty);
                            }

                            // Assign value
                            comboCell.Value = cartonQty;

                            row.Cells["UnitQty"].Value = dr["Units"].ToString();
                            row.Cells["UnitPrice"].Value = dr["UnitPrice"];
                            // ✅ Set the correct PerCarton value from Items table
                            row.Cells["PerCarton"].Value = dr["Per_Carton_Quantity"].ToString();


                            // Calculate line total: (cartons * carton price) + (units * unit price)
                            int cartons = dr["Carton_Qty"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Carton_Qty"]);
                            int units = dr["Units"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Units"]);
                            decimal unitPrice = dr["UnitPrice"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["UnitPrice"]);
                            decimal cartonPrice = dr["CartonPrice"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["CartonPrice"]);

                            decimal lineTotal = (cartons * cartonPrice) + (units * unitPrice);
                            row.Cells["LineTotal"].Value = lineTotal;

                            txtCashReceived.Text = dr["Cash_Received"].ToString();

                            // Keep invoice in tag for refund/deletion logic
                            txtSearch_recipt_invoice.Tag = dr["Invoice_No"].ToString();
                        }
                    }
                }

                UpdateGrandTotal(); // recalc total for loaded items
                UpdateBalance();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sale: " + ex.Message);
            }
        }

        private void btnUpdateTransaction_Click(object sender, EventArgs e)
        {
            string search = txtSearch_recipt_invoice.Text.Trim();
            if (string.IsNullOrEmpty(search))
            {
                MessageBox.Show("Please enter a valid Invoice or Receipt number.");
                return;
            }

            if (dgvCart.Rows.Count == 0)
            {
                MessageBox.Show("Cart is empty. Nothing to update.");
                return;
            }

            DialogResult dr = MessageBox.Show(
                "Are you sure you want to update this transaction?",
                "Confirm Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;

            using (SqlConnection con = DB.GetCon())
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    // 1. Restore stock from old transaction
                    // Restore stock from old transaction
                    SqlCommand cmdRestore = new SqlCommand(@"
                        UPDATE i
                        SET i.Total_Quantity = i.Total_Quantity + s.Total_Qty
                        FROM Items i
                        INNER JOIN Sales s ON i.ItemID = s.ItemID
                        WHERE s.Invoice_No = @search OR s.Receipt_No = @search
                    ", con, tran);

                    cmdRestore.Parameters.AddWithValue("@search", search);
                    cmdRestore.ExecuteNonQuery();


                    // 2. Delete old transaction
                    SqlCommand cmdDelSales = new SqlCommand(@"
                DELETE FROM Sales 
                WHERE Invoice_No = @search OR Receipt_No = @search
            ", con, tran);
                    cmdDelSales.Parameters.AddWithValue("@search", search);
                    cmdDelSales.ExecuteNonQuery();

                    SqlCommand cmdDelSummary = new SqlCommand(@"
                DELETE FROM Sales_Summary 
                WHERE Invoice_No = @search OR Receipt_No = @search
            ", con, tran);
                    cmdDelSummary.Parameters.AddWithValue("@search", search);
                    cmdDelSummary.ExecuteNonQuery();

                    // 3. Insert updated transaction from grid
                    decimal subtotal = 0m;
                    decimal cashReceived = 0m;
                    decimal.TryParse(txtCashReceived.Text, out cashReceived);

                    foreach (DataGridViewRow row in dgvCart.Rows)
                    {
                        string itemId = row.Cells["ItemID"].Value.ToString();
                        int cartons = int.Parse(row.Cells["CartonQty"].Value?.ToString() ?? "0");
                        int units = int.Parse(row.Cells["UnitQty"].Value?.ToString() ?? "0");
                        int perCarton = int.Parse(row.Cells["PerCarton"].Value?.ToString() ?? "1");
                        decimal unitPrice = Convert.ToDecimal(row.Cells["UnitPrice"].Value);
                        decimal cartonPrice = Convert.ToDecimal(row.Cells["CartonPrice"].Value);

                        int totalUnits = (cartons * perCarton) + units;
                        decimal lineTotal = (cartons * cartonPrice) + (units * unitPrice);
                        subtotal += lineTotal;

                        // Insert into Sales table
                        SqlCommand cmdInsert = new SqlCommand(@"
                    INSERT INTO Sales (Invoice_No, Receipt_No, ItemID, Carton_Qty, Carton_Sp, Units, Unit_Sp, Total_Qty, Subtotal, Cash_Received, Balance, Date_Sold, SoldBy)
                    VALUES (@inv, @rcp, @item, @carton, @carton_sp, @units, @units_sp, @totalqty, @subtotal, @cash, @bal, GETDATE(), @SoldBy)
                ", con, tran);

                        cmdInsert.Parameters.AddWithValue("@inv", txtSearch_recipt_invoice.Tag.ToString());
                        cmdInsert.Parameters.AddWithValue("@rcp", txtSearch_recipt_invoice.Text.Trim());
                        cmdInsert.Parameters.AddWithValue("@item", itemId);
                        cmdInsert.Parameters.AddWithValue("@carton", cartons);
                        cmdInsert.Parameters.AddWithValue("@carton_sp", cartonPrice);
                        cmdInsert.Parameters.AddWithValue("@units", units);
                        cmdInsert.Parameters.AddWithValue("@units_sp", unitPrice);
                        cmdInsert.Parameters.AddWithValue("@totalqty", totalUnits);
                        cmdInsert.Parameters.AddWithValue("@subtotal", lineTotal);
                        cmdInsert.Parameters.AddWithValue("@cash", cashReceived);
                        cmdInsert.Parameters.AddWithValue("@bal", cashReceived - subtotal);
                        cmdInsert.Parameters.AddWithValue("@SoldBy", _currentId);

                        cmdInsert.ExecuteNonQuery();

                        // Update stock for new transaction
                        SqlCommand cmdStock = new SqlCommand(@"
                    UPDATE Items 
                    SET Total_Quantity = Total_Quantity - @soldQty
                    WHERE ItemID = @item
                ", con, tran);
                        cmdStock.Parameters.AddWithValue("@soldQty", totalUnits);
                        cmdStock.Parameters.AddWithValue("@item", itemId);
                        cmdStock.ExecuteNonQuery();
                    }

                    // Insert into Sales_Summary
                    SqlCommand cmdSummary = new SqlCommand(@"
                INSERT INTO Sales_Summary (Invoice_No, Receipt_No, Subtotal, CashReceived, Balance, Date_Sold, SoldBy)
                VALUES (@inv, @rcp, @subtotal, @cash, @bal, GETDATE(), @SoldBy)
            ", con, tran);
                    cmdSummary.Parameters.AddWithValue("@inv", txtSearch_recipt_invoice.Tag.ToString());
                    cmdSummary.Parameters.AddWithValue("@rcp", txtSearch_recipt_invoice.Text.Trim());
                    cmdSummary.Parameters.AddWithValue("@subtotal", subtotal);
                    cmdSummary.Parameters.AddWithValue("@cash", cashReceived);
                    cmdSummary.Parameters.AddWithValue("@bal", cashReceived - subtotal);
                    cmdSummary.Parameters.AddWithValue("@SoldBy", _currentId);
                    cmdSummary.ExecuteNonQuery();

                    tran.Commit();

                    MessageBox.Show("Transaction updated successfully!");
                    btnDeleteTransaction.Visible = false;
                    btnUpdateTransaction.Visible = false;
                    txtSearch_recipt_invoice.Text = "";
                    lblGrandTotal.Text = "0.00";
                    lblGrandTotal2.Text = "0.00";
                    lblBalance.Text = "Balance: 0.00";
                    txtCashReceived.Text = ""; ;
                    UpdateGrandTotal();
                    btnClearCart_Click(null, null);
                    UpdateBalance();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show("Error updating transaction: " + ex.Message);
                }
            }
        }

        private void txtSearchItemName_TextChanged(object sender, EventArgs e)
        {
            lstSuggestions.Items.Clear();
            

            if (txtSearchItemName.Text.Trim().Length < 1)
            {
                lstSuggestions.Visible = false;
                return;
            }

            using (SqlConnection con = DB.GetCon())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT TOP 20 ItemID, ItemName FROM Items WHERE ItemName LIKE @s + '%' ORDER BY ItemName",
                    con
                );
                cmd.Parameters.AddWithValue("@s", txtSearchItemName.Text.Trim());

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lstSuggestions.Items.Add(new
                        {
                            ID = dr["ItemID"].ToString(),
                            Name = dr["ItemName"].ToString()
                        });
                    }
                }
            }

            lstSuggestions.DisplayMember = "Name";
            lstSuggestions.ValueMember = "ID";
            lstSuggestions.Visible = lstSuggestions.Items.Count > 0;
        }


        private void lstSuggestions_Click(object sender, EventArgs e)
        {
            
            if (lstSuggestions.SelectedItem == null) return;

            var selected = (dynamic)lstSuggestions.SelectedItem;
            string itemId = selected.ID;

            // Retrieve full item details
            using (SqlConnection con = DB.GetCon())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
            SELECT ItemID, ItemName, Barcode, Quantity_SP, 
                   Per_Carton_Quantity, Carton_SP
            FROM Items
            WHERE ItemID = @id", con);

                cmd.Parameters.AddWithValue("@id", itemId);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        string id = dr["ItemID"].ToString();
                        string name = dr["ItemName"].ToString();
                        string barcode = dr["Barcode"].ToString();

                        decimal unitPrice =
                            dr["Quantity_SP"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["Quantity_SP"]);

                        int perCarton =
                            dr["Per_Carton_Quantity"] == DBNull.Value ? 1 : Convert.ToInt32(dr["Per_Carton_Quantity"]);

                        decimal cartonPrice =
                            dr["Carton_SP"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["Carton_SP"]);

                        // ✔ Now call your function with correct parameters
                        AddItemToCart(id, name, barcode, unitPrice, perCarton, cartonPrice);
                    }
                }
            }

            // Hide suggestion dropdown
            lstSuggestions.Visible = false;
            txtSearchItemName.Clear();
            txtSearchItemName.Focus();
        }

        private void lstSuggestions_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


    }
}