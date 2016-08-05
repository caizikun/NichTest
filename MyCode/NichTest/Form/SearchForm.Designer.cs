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
            this.btnSearch = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txtListSN = new System.Windows.Forms.TextBox();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.radioButtonBySN = new System.Windows.Forms.RadioButton();
            this.radioButtonByGroup = new System.Windows.Forms.RadioButton();
            this.radioButtonLatestData = new System.Windows.Forms.RadioButton();
            this.radioButtonFullData = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnHelp);
            this.panel1.Controls.Add(this.btnClearAll);
            this.panel1.Controls.Add(this.btnSearch);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(15, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(388, 82);
            this.panel1.TabIndex = 0;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(15, 24);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(107, 40);
            this.btnSearch.TabIndex = 0;
            this.btnSearch.Text = "查询";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(15, 82);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtListSN);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(388, 245);
            this.splitContainer1.SplitterDistance = 223;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 1;
            // 
            // txtListSN
            // 
            this.txtListSN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtListSN.Location = new System.Drawing.Point(0, 0);
            this.txtListSN.Multiline = true;
            this.txtListSN.Name = "txtListSN";
            this.txtListSN.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtListSN.Size = new System.Drawing.Size(223, 245);
            this.txtListSN.TabIndex = 0;
            this.txtListSN.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtListSN_KeyPress);
            this.txtListSN.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtListSN_PreviewKeyDown);
            // 
            // btnClearAll
            // 
            this.btnClearAll.Location = new System.Drawing.Point(144, 24);
            this.btnClearAll.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(107, 40);
            this.btnClearAll.TabIndex = 1;
            this.btnClearAll.Text = "清空";
            this.btnClearAll.UseVisualStyleBackColor = true;
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(261, 24);
            this.btnHelp.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(107, 40);
            this.btnHelp.TabIndex = 1;
            this.btnHelp.Text = "帮助";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.radioButtonBySN);
            this.splitContainer2.Panel1.Controls.Add(this.radioButtonByGroup);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.radioButtonFullData);
            this.splitContainer2.Panel2.Controls.Add(this.radioButtonLatestData);
            this.splitContainer2.Size = new System.Drawing.Size(160, 245);
            this.splitContainer2.SplitterDistance = 125;
            this.splitContainer2.TabIndex = 0;
            // 
            // radioButtonBySN
            // 
            this.radioButtonBySN.AutoSize = true;
            this.radioButtonBySN.Location = new System.Drawing.Point(33, 18);
            this.radioButtonBySN.Name = "radioButtonBySN";
            this.radioButtonBySN.Size = new System.Drawing.Size(82, 25);
            this.radioButtonBySN.TabIndex = 2;
            this.radioButtonBySN.Text = "SN查询";
            this.radioButtonBySN.UseVisualStyleBackColor = true;
            // 
            // radioButtonByGroup
            // 
            this.radioButtonByGroup.AutoSize = true;
            this.radioButtonByGroup.Checked = true;
            this.radioButtonByGroup.Location = new System.Drawing.Point(33, 67);
            this.radioButtonByGroup.Name = "radioButtonByGroup";
            this.radioButtonByGroup.Size = new System.Drawing.Size(92, 25);
            this.radioButtonByGroup.TabIndex = 3;
            this.radioButtonByGroup.TabStop = true;
            this.radioButtonByGroup.Text = "批量查询";
            this.radioButtonByGroup.UseVisualStyleBackColor = true;
            // 
            // radioButtonLatestData
            // 
            this.radioButtonLatestData.AutoSize = true;
            this.radioButtonLatestData.Checked = true;
            this.radioButtonLatestData.Location = new System.Drawing.Point(33, 21);
            this.radioButtonLatestData.Name = "radioButtonLatestData";
            this.radioButtonLatestData.Size = new System.Drawing.Size(92, 25);
            this.radioButtonLatestData.TabIndex = 0;
            this.radioButtonLatestData.TabStop = true;
            this.radioButtonLatestData.Text = "最新数据";
            this.radioButtonLatestData.UseVisualStyleBackColor = true;
            // 
            // radioButtonFullData
            // 
            this.radioButtonFullData.AutoSize = true;
            this.radioButtonFullData.Location = new System.Drawing.Point(33, 70);
            this.radioButtonFullData.Name = "radioButtonFullData";
            this.radioButtonFullData.Size = new System.Drawing.Size(92, 25);
            this.radioButtonFullData.TabIndex = 1;
            this.radioButtonFullData.Text = "所有数据";
            this.radioButtonFullData.UseVisualStyleBackColor = true;
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 342);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "SearchForm";
            this.Padding = new System.Windows.Forms.Padding(15, 0, 0, 15);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "查找数据";
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.TextBox txtListSN;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.RadioButton radioButtonBySN;
        private System.Windows.Forms.RadioButton radioButtonByGroup;
        private System.Windows.Forms.RadioButton radioButtonFullData;
        private System.Windows.Forms.RadioButton radioButtonLatestData;
    }
}