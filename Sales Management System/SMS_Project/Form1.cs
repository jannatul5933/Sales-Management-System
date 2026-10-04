using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMS_Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void insertDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmInvoiceEntry frm = new frmInvoiceEntry();
            frm.ShowDialog();
            frm.MdiParent=this;
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMasterDetails frm=new frmMasterDetails();
            frm.ShowDialog();
            frm.MdiParent=this;
        }

        private void reportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmcrystelreport frm=new frmcrystelreport();
            frm.Show();
        }
    }
}
