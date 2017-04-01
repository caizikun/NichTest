using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NichTest
{
    public partial class DCA_Form : Form
    {
        private Scope dca;

        public DCA_Form()
        {
            InitializeComponent();
        }

        private void btnAutoScale_Click(object sender, EventArgs e)
        {
            if (dca == null)
            {
                return;
            }
            dca.AutoScale(1);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (this.comboBoxItemModel.Text.Contains("86100"))
            {
                dca = new Flex86100();
            }
            else
            {
                return;
            }
            dca.Address = this.numericUpDownAddress.Value.ToString();
            bool connected = dca.Connect();
            this.Text = this.comboBoxItemModel.Text;
            string message;
            if (connected == true)
            {
                message = "successfully";
            }
            else
            {
                message = "failed";
                dca = null;
            }
            MessageBox.Show("Connected " + message, "message", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (dca == null)
            {
                return;
            }
            dca.ClearDisplay();
        }

        private void btnRunStop_Click(object sender, EventArgs e)
        {
            if (dca == null)
            {
                return;
            }

            if(this.btnRunStop.Text== "Run/Stop"|| this.btnRunStop.Text == "Run")
            {
                dca.RunStop(true, 1);
                this.btnRunStop.Text = "Stop";
            }
            else
            {
                dca.RunStop(false, 1);
                this.btnRunStop.Text = "Run";
            }            
        }
    }
}
