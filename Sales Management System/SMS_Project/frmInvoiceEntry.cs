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
    public partial class frmInvoiceEntry : Form
    {
SqlConnection con = new SqlConnection("Data Source=WIN-S18J4MLRQMQ\\SA; Initial Catalog=SalesDB;Integrated Security=True");        
        public frmInvoiceEntry()
        {
            InitializeComponent();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            
            var cmd = new SqlCommand("INSERT INTO Invoice (CustomerName, InvoiceDate) VALUES (@CustomerName, @InvoiceDate)", con);

            cmd.Parameters.AddWithValue("@CustomerName", txtName.Text.Trim());
            cmd.Parameters.AddWithValue("@InvoiceDate", dtpInvoiceDate.Value);

            con.Open();
            cmd.ExecuteNonQuery();
            LoadData();
            con.Close();

            lblMsg.Text = "Customer Name & Date Inserted Successfully";
        }

        private void frmInvoiceEntry_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM  Invoice", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
            SqlCommand cmd = new SqlCommand("DELETE FROM Invoice WHERE InvoiceID=@InvoiceID", con);
            cmd.Parameters.AddWithValue("@InvoiceID", id);
            con.Open();
            cmd.ExecuteNonQuery();
            LoadData();
            con.Close();

            lblMsg.Text = "Customer Name & Date Delete Successfully";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            
            txtName.Clear();
            dtpInvoiceDate.Value = DateTime.Now;
            lblMsg.Text = "";
            txtName.Focus();

        }
    }
}
