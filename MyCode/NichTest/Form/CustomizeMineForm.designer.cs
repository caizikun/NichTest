namespace NichTest
{
    partial class CustomizeMineForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.height = new System.Windows.Forms.TextBox();
            this.width = new System.Windows.Forms.TextBox();
            this.mineCounts = new System.Windows.Forms.TextBox();
            this.exit = new System.Windows.Forms.Button();
            this.confirm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "高度(H)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "宽度(W)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "雷数(M)";
            // 
            // height
            // 
            this.height.CausesValidation = false;
            this.height.Location = new System.Drawing.Point(105, 14);
            this.height.Name = "height";
            this.height.Size = new System.Drawing.Size(77, 21);
            this.height.TabIndex = 3;
            // 
            // width
            // 
            this.width.Location = new System.Drawing.Point(105, 51);
            this.width.Name = "width";
            this.width.Size = new System.Drawing.Size(77, 21);
            this.width.TabIndex = 4;
            // 
            // mineCounts
            // 
            this.mineCounts.Location = new System.Drawing.Point(105, 86);
            this.mineCounts.Name = "mineCounts";
            this.mineCounts.Size = new System.Drawing.Size(77, 21);
            this.mineCounts.TabIndex = 5;
            // 
            // exit
            // 
            this.exit.Location = new System.Drawing.Point(21, 130);
            this.exit.Name = "exit";
            this.exit.Size = new System.Drawing.Size(63, 24);
            this.exit.TabIndex = 6;
            this.exit.Text = "取消";
            this.exit.UseVisualStyleBackColor = true;
            // 
            // confirm
            // 
            this.confirm.Location = new System.Drawing.Point(126, 130);
            this.confirm.Name = "confirm";
            this.confirm.Size = new System.Drawing.Size(64, 23);
            this.confirm.TabIndex = 7;
            this.confirm.Text = "确定";
            this.confirm.UseVisualStyleBackColor = true;
            // 
            // sefDefMineWnd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(213, 170);
            this.Controls.Add(this.confirm);
            this.Controls.Add(this.exit);
            this.Controls.Add(this.mineCounts);
            this.Controls.Add(this.width);
            this.Controls.Add(this.height);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "sefDefMineWnd";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "自定义雷区";
            this.Load += new System.EventHandler(this.sefDefMineWnd_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox height;
        private System.Windows.Forms.TextBox width;
        private System.Windows.Forms.TextBox mineCounts;
        private System.Windows.Forms.Button exit;
        private System.Windows.Forms.Button confirm;
    }
}