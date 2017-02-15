namespace NichTest
{
    partial class SearchForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txtListSN = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonFullData = new System.Windows.Forms.RadioButton();
            this.radioButtonLatestData = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonByGroup = new System.Windows.Forms.RadioButton();
            this.radioButtonBySN = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonQuickCheck = new System.Windows.Forms.RadioButton();
            this.radioButtonRxO = new System.Windows.Forms.RadioButton();
            this.radioButtonTxO = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(62)))), ((int)(((byte)(91)))));
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(20);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10);
            this.panel1.Size = new System.Drawing.Size(493, 487);
            this.panel1.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(10, 84);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtListSN);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(5);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(5);
            this.splitContainer1.Size = new System.Drawing.Size(473, 393);
            this.splitContainer1.SplitterDistance = 232;
            this.splitContainer1.TabIndex = 3;
            // 
            // txtListSN
            // 
            this.txtListSN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtListSN.Location = new System.Drawing.Point(5, 5);
            this.txtListSN.Multiline = true;
            this.txtListSN.Name = "txtListSN";
            this.txtListSN.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtListSN.Size = new System.Drawing.Size(222, 383);
            this.txtListSN.TabIndex = 4;
            this.txtListSN.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtListSN_KeyPress);
            this.txtListSN.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtListSN_PreviewKeyDown);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.White;
            this.groupBox3.Controls.Add(this.radioButtonFullData);
            this.groupBox3.Controls.Add(this.radioButtonLatestData);
            this.groupBox3.Location = new System.Drawing.Point(8, 285);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(221, 100);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            // 
            // radioButtonFullData
            // 
            this.radioButtonFullData.AutoSize = true;
            this.radioButtonFullData.Location = new System.Drawing.Point(44, 59);
            this.radioButtonFullData.Name = "radioButtonFullData";
            this.radioButtonFullData.Size = new System.Drawing.Size(92, 25);
            this.radioButtonFullData.TabIndex = 6;
            this.radioButtonFullData.Text = "所有数据";
            this.radioButtonFullData.UseVisualStyleBackColor = true;
            // 
            // radioButtonLatestData
            // 
            this.radioButtonLatestData.AutoSize = true;
            this.radioButtonLatestData.Checked = true;
            this.radioButtonLatestData.Location = new System.Drawing.Point(44, 28);
            this.radioButtonLatestData.Name = "radioButtonLatestData";
            this.radioButtonLatestData.Size = new System.Drawing.Size(92, 25);
            this.radioButtonLatestData.TabIndex = 5;
            this.radioButtonLatestData.TabStop = true;
            this.radioButtonLatestData.Text = "最新数据";
            this.radioButtonLatestData.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.White;
            this.groupBox2.Controls.Add(this.radioButtonByGroup);
            this.groupBox2.Controls.Add(this.radioButtonBySN);
            this.groupBox2.Location = new System.Drawing.Point(8, 162);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(221, 117);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // radioButtonByGroup
            // 
            this.radioButtonByGroup.AutoSize = true;
            this.radioButtonByGroup.Checked = true;
            this.radioButtonByGroup.Location = new System.Drawing.Point(44, 70);
            this.radioButtonByGroup.Name = "radioButtonByGroup";
            this.radioButtonByGroup.Size = new System.Drawing.Size(92, 25);
            this.radioButtonByGroup.TabIndex = 8;
            this.radioButtonByGroup.TabStop = true;
            this.radioButtonByGroup.Text = "批量查询";
            this.radioButtonByGroup.UseVisualStyleBackColor = true;
            // 
            // radioButtonBySN
            // 
            this.radioButtonBySN.AutoSize = true;
            this.radioButtonBySN.Location = new System.Drawing.Point(44, 28);
            this.radioButtonBySN.Name = "radioButtonBySN";
            this.radioButtonBySN.Size = new System.Drawing.Size(82, 25);
            this.radioButtonBySN.TabIndex = 7;
            this.radioButtonBySN.Text = "SN查询";
            this.radioButtonBySN.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.radioButtonQuickCheck);
            this.groupBox1.Controls.Add(this.radioButtonRxO);
            this.groupBox1.Controls.Add(this.radioButtonTxO);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(221, 148);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // radioButtonQuickCheck
            // 
            this.radioButtonQuickCheck.AutoSize = true;
            this.radioButtonQuickCheck.Checked = true;
            this.radioButtonQuickCheck.Location = new System.Drawing.Point(44, 106);
            this.radioButtonQuickCheck.Name = "radioButtonQuickCheck";
            this.radioButtonQuickCheck.Size = new System.Drawing.Size(119, 25);
            this.radioButtonQuickCheck.TabIndex = 2;
            this.radioButtonQuickCheck.TabStop = true;
            this.radioButtonQuickCheck.Text = "QuickCheck";
            this.radioButtonQuickCheck.UseVisualStyleBackColor = true;
            // 
            // radioButtonRxO
            // 
            this.radioButtonRxO.AutoSize = true;
            this.radioButtonRxO.Location = new System.Drawing.Point(43, 66);
            this.radioButtonRxO.Name = "radioButtonRxO";
            this.radioButtonRxO.Size = new System.Drawing.Size(59, 25);
            this.radioButtonRxO.TabIndex = 1;
            this.radioButtonRxO.Text = "RxO";
            this.radioButtonRxO.UseVisualStyleBackColor = true;
            // 
            // radioButtonTxO
            // 
            this.radioButtonTxO.AutoSize = true;
            this.radioButtonTxO.Location = new System.Drawing.Point(44, 19);
            this.radioButtonTxO.Name = "radioButtonTxO";
            this.radioButtonTxO.Size = new System.Drawing.Size(58, 25);
            this.radioButtonTxO.TabIndex = 0;
            this.radioButtonTxO.Text = "TxO";
            this.radioButtonTxO.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.btnHelp);
            this.panel2.Controls.Add(this.btnClearAll);
            this.panel2.Controls.Add(this.btnSearch);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(10, 10);
            this.panel2.Margin = new System.Windows.Forms.Padding(5);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(10);
            this.panel2.Size = new System.Drawing.Size(473, 74);
            this.panel2.TabIndex = 2;
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(342, 13);
            this.btnHelp.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(107, 51);
            this.btnHelp.TabIndex = 1;
            this.btnHelp.Text = "帮助";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnClearAll
            // 
            this.btnClearAll.Location = new System.Drawing.Point(192, 13);
            this.btnClearAll.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(107, 51);
            this.btnClearAll.TabIndex = 1;
            this.btnClearAll.Text = "清空";
            this.btnClearAll.UseVisualStyleBackColor = true;
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(30, 13);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Padding = new System.Windows.Forms.Padding(5);
            this.btnSearch.Size = new System.Drawing.Size(107, 51);
            this.btnSearch.TabIndex = 0;
            this.btnSearch.Text = "查询";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 487);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.Name = "SearchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "查找数据";
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtListSN;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioButtonFullData;
        private System.Windows.Forms.RadioButton radioButtonLatestData;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonByGroup;
        private System.Windows.Forms.RadioButton radioButtonBySN;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonQuickCheck;
        private System.Windows.Forms.RadioButton radioButtonRxO;
        private System.Windows.Forms.RadioButton radioButtonTxO;
    }
}