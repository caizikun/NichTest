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
    public partial class CustomizeMineForm : Form
    {
        private string gHeight;
        public string GHeight
        {
            get { return gHeight; }
            set { gHeight = value; }
        }


        private string gWidth;
        public string GWidth
        {
            get { return gWidth; }
            set { gWidth = value; }
        }

        private string gMineCounts;
        public string GMineCounts
        {
            get { return gMineCounts; }
            set { gMineCounts = value; }
        }

        private bool updateFlag;
        public bool UpdateFlag
        {
            get { return updateFlag; }
            set { updateFlag = value; }
        }

        public CustomizeMineForm()
        {
            InitializeComponent();

            this.height.Text = "9";
            this.width.Text = "9";
            this.mineCounts.Text = "10";
            updateFlag = false;

            this.confirm.Click += new EventHandler(confirm_Click);
            this.exit.Click += new EventHandler(exit_Click);
        }

        //当按下确定按钮
        void confirm_Click(object sender, EventArgs e)
        {
            if (!this.height.Text.Equals("") && !this.width.Text.Equals("") && !this.mineCounts.Text.Equals(""))
            {
                gHeight = this.height.Text;
                gWidth = this.width.Text;
                gMineCounts = this.mineCounts.Text;
                updateFlag = true;
            }
            this.Close();
        }

        void exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void sefDefMineWnd_Load(object sender, EventArgs e)
        {
           
        }
    }
}
