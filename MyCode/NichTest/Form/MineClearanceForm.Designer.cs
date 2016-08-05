namespace NichTest
{
    partial class MineClearanceForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// 由程序自动生成
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MineClearanceForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.游戏ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.开局ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.primaryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.intermediateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.seniorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selfDefMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Gainsboro;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.游戏ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(220, 27);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 游戏ToolStripMenuItem
            // 
            this.游戏ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.开局ToolStripMenuItem,
            this.toolStripSeparator1,
            this.primaryMenuItem,
            this.intermediateMenuItem,
            this.seniorMenuItem,
            this.selfDefMenuItem,
            this.toolStripSeparator2,
            this.退出ToolStripMenuItem});
            this.游戏ToolStripMenuItem.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.游戏ToolStripMenuItem.Name = "游戏ToolStripMenuItem";
            this.游戏ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.游戏ToolStripMenuItem.Text = "游戏";
            // 
            // 开局ToolStripMenuItem
            // 
            this.开局ToolStripMenuItem.Name = "开局ToolStripMenuItem";
            this.开局ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.开局ToolStripMenuItem.Text = "开局";
            this.开局ToolStripMenuItem.Click += new System.EventHandler(this.开局ToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // primaryMenuItem
            // 
            this.primaryMenuItem.AutoToolTip = true;
            this.primaryMenuItem.CheckOnClick = true;
            this.primaryMenuItem.MergeIndex = 1;
            this.primaryMenuItem.Name = "primaryMenuItem";
            this.primaryMenuItem.Size = new System.Drawing.Size(152, 22);
            this.primaryMenuItem.Text = "初级";
            this.primaryMenuItem.Click += new System.EventHandler(this.初级ToolStripMenuItem_Click);
            // 
            // intermediateMenuItem
            // 
            this.intermediateMenuItem.AutoToolTip = true;
            this.intermediateMenuItem.CheckOnClick = true;
            this.intermediateMenuItem.MergeIndex = 1;
            this.intermediateMenuItem.Name = "intermediateMenuItem";
            this.intermediateMenuItem.Size = new System.Drawing.Size(152, 22);
            this.intermediateMenuItem.Text = "中级";
            this.intermediateMenuItem.Click += new System.EventHandler(this.中级ToolStripMenuItem_Click);
            // 
            // seniorMenuItem
            // 
            this.seniorMenuItem.AutoToolTip = true;
            this.seniorMenuItem.CheckOnClick = true;
            this.seniorMenuItem.MergeIndex = 1;
            this.seniorMenuItem.Name = "seniorMenuItem";
            this.seniorMenuItem.Size = new System.Drawing.Size(152, 22);
            this.seniorMenuItem.Text = "高级";
            this.seniorMenuItem.Click += new System.EventHandler(this.高级ToolStripMenuItem_Click);
            // 
            // selfDefMenuItem
            // 
            this.selfDefMenuItem.AutoToolTip = true;
            this.selfDefMenuItem.CheckOnClick = true;
            this.selfDefMenuItem.MergeIndex = 1;
            this.selfDefMenuItem.Name = "selfDefMenuItem";
            this.selfDefMenuItem.Size = new System.Drawing.Size(152, 22);
            this.selfDefMenuItem.Text = "自定义...";
            this.selfDefMenuItem.Click += new System.EventHandler(this.自定义ToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.退出ToolStripMenuItem.Text = "退出";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // MineClearanceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(220, 261);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "MineClearanceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "扫雷";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.mainWin_FormClosed);
            this.Load += new System.EventHandler(this.mainWin_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 游戏ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 开局ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem primaryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem intermediateMenuItem;
        private System.Windows.Forms.ToolStripMenuItem seniorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selfDefMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
    }
}

