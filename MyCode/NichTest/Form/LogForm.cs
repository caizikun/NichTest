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
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
        }

        public void ChangeText()
        {
            try
            {
                this.textBoxLog.Clear();
                this.textBoxLog.Text = Log.ReadLogFromTxt();

                //让文本框获取焦点
                this.textBoxLog.Focus();
                //设置光标的位置到文本尾
                this.textBoxLog.SelectionStart = (textBoxLog.Text.Length - 1) < 0 ? 0 : (textBoxLog.Text.Length - 1);
                //滚动到控件光标处
                this.textBoxLog.ScrollToCaret();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
