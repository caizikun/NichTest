namespace NichTest
{
    partial class DCA_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBoxItemModel = new System.Windows.Forms.ComboBox();
            this.btnAutoScale = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnRunStop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.numericUpDownAddress = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAddress)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxItemModel
            // 
            this.comboBoxItemModel.Cursor = System.Windows.Forms.Cursors.Default;
            this.comboBoxItemModel.FormattingEnabled = true;
            this.comboBoxItemModel.Items.AddRange(new object[] {
            "86100"});
            this.comboBoxItemModel.Location = new System.Drawing.Point(76, 16);
            this.comboBoxItemModel.Name = "comboBoxItemModel";
            this.comboBoxItemModel.Size = new System.Drawing.Size(75, 21);
            this.comboBoxItemModel.TabIndex = 0;
            // 
            // btnAutoScale
            // 
            this.btnAutoScale.Location = new System.Drawing.Point(15, 112);
            this.btnAutoScale.Name = "btnAutoScale";
            this.btnAutoScale.Size = new System.Drawing.Size(75, 48);
            this.btnAutoScale.TabIndex = 1;
            this.btnAutoScale.Text = "AutoScale";
            this.btnAutoScale.UseVisualStyleBackColor = true;
            this.btnAutoScale.Click += new System.EventHandler(this.btnAutoScale_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(105, 112);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 48);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnRunStop
            // 
            this.btnRunStop.Location = new System.Drawing.Point(197, 112);
            this.btnRunStop.Name = "btnRunStop";
            this.btnRunStop.Size = new System.Drawing.Size(75, 48);
            this.btnRunStop.TabIndex = 1;
            this.btnRunStop.Text = "Run/Stop";
            this.btnRunStop.UseVisualStyleBackColor = true;
            this.btnRunStop.Click += new System.EventHandler(this.btnRunStop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "model";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "address";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(184, 24);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 48);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // numericUpDownAddress
            // 
            this.numericUpDownAddress.Location = new System.Drawing.Point(76, 57);
            this.numericUpDownAddress.Name = "numericUpDownAddress";
            this.numericUpDownAddress.Size = new System.Drawing.Size(75, 20);
            this.numericUpDownAddress.TabIndex = 4;
            this.numericUpDownAddress.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // DCA_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 193);
            this.Controls.Add(this.numericUpDownAddress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnRunStop);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnAutoScale);
            this.Controls.Add(this.comboBoxItemModel);
            this.MaximizeBox = false;
            this.Name = "DCA_Form";
            this.Text = "DCA";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAddress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxItemModel;
        private System.Windows.Forms.Button btnAutoScale;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnRunStop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.NumericUpDown numericUpDownAddress;
    }
}