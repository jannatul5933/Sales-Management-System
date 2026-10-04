using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMS_Project
{
    public partial class frmMasterDetails : Form
    {
     
        SqlConnection con = new SqlConnection("Data Source=WIN-S18J4MLRQMQ\\SA; Initial Catalog=SalesDB;Integrated Security=True");

        public frmMasterDetails()
        {
            InitializeComponent();
            LoadInvoiceIDs();
        }

        private void frmMasterDetails_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadMasterInvoiceData();
        }

        private void LoadMasterInvoiceData()
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT InvoiceID, CustomerName, InvoiceDate FROM Invoice", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvMasterInvoice.DataSource = dt;
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error loading Master data: " + ex.Message;
            }
        }

        private void LoadData()
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM InvoiceDetails", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error loading Details data: " + ex.Message;
            }
        }
        private void btnInsert_Click(object sender, EventArgs e)
        {
          
            if (string.IsNullOrEmpty(txtProductName.Text) || string.IsNullOrEmpty(txtUnitPrice.Text))
            {
                lblMsg.ForeColor = Color.Red;
                lblMsg.Text = "Please fill all required fields (Product Name, Price).";
                return;
            }

            string customerName = "Walk-in Customer"; 
            DateTime invoiceDate = DateTime.Now;      

            if (dgvMasterInvoice.CurrentRow != null)
            {
                var row = dgvMasterInvoice.CurrentRow;

                if (row.Cells[1].Value != null && !string.IsNullOrEmpty(row.Cells[1].Value.ToString()))
                {
                    customerName = row.Cells[1].Value.ToString();
                }

                if (row.Cells[2].Value != null && !string.IsNullOrEmpty(row.Cells[2].Value.ToString()))
                {
                    DateTime.TryParse(row.Cells[2].Value.ToString(), out invoiceDate);
                }
            }

            con.Open();
            SqlTransaction transaction = con.BeginTransaction();

            try
            {
                string masterQuery = "INSERT INTO Invoice (CustomerName, InvoiceDate) VALUES (@custName, @invDate); SELECT SCOPE_IDENTITY();";

                int newInvoiceId = 0;
                using (SqlCommand masterCmd = new SqlCommand(masterQuery, con, transaction))
                {
                    masterCmd.Parameters.AddWithValue("@custName", customerName.Trim());
                    masterCmd.Parameters.AddWithValue("@invDate", invoiceDate);

                    
                    newInvoiceId = Convert.ToInt32(masterCmd.ExecuteScalar());
                }

               
                string detailsQuery = "INSERT INTO InvoiceDetails (InvoiceID, ProductName, ProductQuantity, ProductImage, UnitPrice, ExpiryDate, IsWarrantyAvailable) " +
                                      "VALUES (@invoiceId, @productName, @quantity, @img, @unitPrice, @expiryDate, @isWarranty)";

                using (SqlCommand cmd = new SqlCommand(detailsQuery, con, transaction))
                {
                    int qty = 0;
                    int.TryParse(numQuantity.Text, out qty);

                    decimal price = 0;
                    decimal.TryParse(txtUnitPrice.Text, out price);

                    cmd.Parameters.AddWithValue("@invoiceId", newInvoiceId); 
                    cmd.Parameters.AddWithValue("@productName", txtProductName.Text.Trim());
                    cmd.Parameters.AddWithValue("@quantity", qty);
                    cmd.Parameters.AddWithValue("@img", txtImagePath.Text);
                    cmd.Parameters.AddWithValue("@unitPrice", price);
                    cmd.Parameters.AddWithValue("@expiryDate", dtpExpiryDate.Value);
                    cmd.Parameters.AddWithValue("@isWarranty", chkIsWarranty.Checked);

                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();

                lblMsg.ForeColor = Color.Green;
                lblMsg.Text = "Master & Details Inserted Successfully!";

                LoadData();
                LoadMasterInvoiceData();
                LoadInvoiceIDs();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                lblMsg.ForeColor = Color.Red;
                lblMsg.Text = "Error: " + ex.Message;
            }
            finally
            {
                con.Close();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            int detailId = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
            string query = "UPDATE InvoiceDetails SET InvoiceID = @invoiceId, ProductName = @productName, " +
                           "ProductQuantity = @quantity, ProductImage = @img, UnitPrice = @unitPrice, " +
                           "ExpiryDate = @expiryDate, IsWarrantyAvailable = @isWarranty " +
                           "WHERE InvoiceDetailID = @detailId";

            try
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@invoiceId", cmbInvoiceID.SelectedValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@productName", txtProductName.Text.Trim());
                    cmd.Parameters.AddWithValue("@quantity", Convert.ToInt32(numQuantity.Value));
                    cmd.Parameters.AddWithValue("@img", txtImagePath.Text);
                    cmd.Parameters.AddWithValue("@unitPrice", Convert.ToDecimal(txtUnitPrice.Text));
                    cmd.Parameters.AddWithValue("@expiryDate", dtpExpiryDate.Value);
                    cmd.Parameters.AddWithValue("@isWarranty", chkIsWarranty.Checked);
                    cmd.Parameters.AddWithValue("@detailId", detailId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    LoadData();
                    LoadMasterInvoiceData();
                    lblMsg.ForeColor = Color.Green;
                    lblMsg.Text = "Product Updated Successfully";
                }
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                lblMsg.ForeColor = Color.Red;
                lblMsg.Text = "Error: " + ex.Message;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

            try
            {
                con.Open();

                if (dgvMasterInvoice.SelectedRows.Count > 0 || dgvMasterInvoice.CurrentRow != null)
                {
                    int invId = Convert.ToInt32(dgvMasterInvoice.CurrentRow.Cells["InvoiceID"].Value);

                    if (invId > 0)
                    {
                        SqlCommand cmd = new SqlCommand("DELETE FROM Invoice WHERE InvoiceID = " + invId, con);
                        cmd.ExecuteNonQuery();
                        lblMsg.Text = "Full Invoice & Details Deleted!";
                    }
                }

                if (dataGridView1.SelectedRows.Count > 0 || dataGridView1.CurrentRow != null)
                {
                    int detailId = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);

                    if (detailId > 0)
                    {
                        SqlCommand cmd = new SqlCommand("DELETE FROM InvoiceDetails WHERE InvoiceDetailID = " + detailId, con);
                        cmd.ExecuteNonQuery();
                        lblMsg.Text = "Single Product Deleted!";
                    }
                }

                con.Close();

                LoadData();
                LoadMasterInvoiceData();
                LoadInvoiceIDs();

                lblMsg.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                lblMsg.ForeColor = Color.Red;
                lblMsg.Text = "Error: " + ex.Message;
            }
        }
        private void LoadInvoiceIDs()
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT InvoiceID FROM Invoice", con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbInvoiceID.DataSource = dt;
                cmbInvoiceID.ValueMember = "InvoiceID";
                cmbInvoiceID.DisplayMember = "InvoiceID";
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";

            if (open.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    txtImagePath.Text = open.FileName;
                    pictureBox1.Image = Image.FromFile(open.FileName);
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid file!");
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtProductName.Clear();
            txtUnitPrice.Clear();
            txtImagePath.Clear();
            numQuantity.Value = 1;

            if (cmbInvoiceID.Items.Count > 0)
            {
                cmbInvoiceID.SelectedIndex = 0;
            }
            dtpExpiryDate.Value = DateTime.Now;
            chkIsWarranty.Checked = false;

            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
            lblMsg.Text = "";
            txtProductName.Focus();
        }
    }
}
