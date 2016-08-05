using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace NichTest
{
    public partial class MineClearanceForm : Form
    {

        //定义mPanel变量
        private minePanel mPanel;
        //定义mAreaManage变量
        private MineAreaManage mAreaManage;
        //雷区的行数
        private int rowMines;
        //雷区的列数
        private int colMines;
        private int minesCount;
        private int mineCell;
        private int clientAreaWidth;
        private int clientAreaHeight;

        private const int borderWidth = 16;

        private const int borderHeight = 70;

        public MineClearanceForm()
        {
            //设置双缓冲
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            rowMines = 9;
            colMines = 9;
            minesCount = 10;
            mineCell = 20;
            clientAreaWidth = colMines * mineCell + borderWidth;
            clientAreaHeight = rowMines * mineCell + borderHeight;

            //layOutAll();
            /**********************************************************************************/
            //程序设计器，由程序自动生成
            InitializeComponent();
        }

        private void layOutAll()
        {
            /**********************************************************************************/
            //判定对象是否存在
            if (this.mPanel == null)
            {
                //创建minePanel对象
                this.mPanel = new minePanel(clientAreaWidth, clientAreaHeight);
            }
            else
            {
                this.mPanel.Dispose();
                //创建minePanel对象
                this.mPanel = new minePanel(clientAreaWidth, clientAreaHeight);
            }
            //
            //mPanel,总的雷区布局，状态区域加扫雷区域
            //
            //this.mPanel.BackColor = System.Drawing.Color.Silver;
            //设置mPanel控件，并以四边全停靠的方式，停靠在父控件上
            //this.mPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mPanel.Location = new System.Drawing.Point(0, SystemInformation.MenuHeight-10);
            this.mPanel.Name = "mPanel";
            //this.mPanel.Size = new System.Drawing.Size(600, 400);

            //在主窗体内添加mPanel控件
            this.Controls.Add(this.mPanel);

            /**********************************************************************************/
            //创建mAreaMange对象
            if (mAreaManage == null)
            {
                //创建mAreaManage对象
                mAreaManage = new MineAreaManage(rowMines, colMines, minesCount);
            }
            else
            {
                mAreaManage.Dispose();
                //创建mAreaManage对象
                mAreaManage = new MineAreaManage(rowMines, colMines, minesCount);
            }
            //mAreaManage.BackColor = Color.FromArgb(20,213,224,238);
            //mAreaManage.BackgroundImage = Image.FromFile("..\\..\\png\\beijing1.jpeg");
            //mAreaManage.BackgroundImage = Properties.Resources.beijing2;
            //指定控件的父窗体或控件
            mAreaManage.Parent = this.mPanel;
            //将minePanel对象传递给mAreaManage类
            mAreaManage.MPanel = this.mPanel;
            //mAreaManage控件布局
            this.mAreaManage.Location = new System.Drawing.Point(ClientRectangle.Left + 5 + 3, ClientRectangle.Top + 10 + 3 + 36);
            this.mAreaManage.Name = "mAreaManage";
            //mAreaManage.Dock = DockStyle.Fill;

            ////将mineAreaManage对象传递给mPanel类
            //this.mPanel.MAreaManage = mAreaManage;
            /**********************************************************************************/
            //
            //设置主窗体客户区域大小
            //
            this.ClientSize = new System.Drawing.Size(clientAreaWidth, clientAreaHeight);
        }

        private void mainWin_Load(object sender, EventArgs e)
        {

           //创建minePanel对象
           this.mPanel = new minePanel(clientAreaWidth, clientAreaHeight);

            //
            //mPanel,总的雷区布局，状态区域加扫雷区域
            //
            //this.mPanel.BackColor = System.Drawing.Color.Silver;
            //设置mPanel控件，并以四边全停靠的方式，停靠在父控件上
            //this.mPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mPanel.Location = new System.Drawing.Point(0, SystemInformation.MenuHeight-10);
            this.mPanel.Name = "mPanel";
            //this.mPanel.Size = new System.Drawing.Size(600, 400);

            //在主窗体内添加mPanel控件
            this.Controls.Add(this.mPanel);

            /**********************************************************************************/
            //创建mAreaMange对象
            mAreaManage = new MineAreaManage(rowMines, colMines, minesCount);

            //mAreaManage.BackColor = Color.FromArgb(20,213,224,238);
           // mAreaManage.BackgroundImage = Image.FromFile("..\\..\\png\\beijing1.jpeg");
            //mAreaManage.BackgroundImage = Properties.Resources.beijing2;
            mAreaManage.Parent = this.mPanel;
            //将minePanel对象传递给mAreaManage类
            mAreaManage.MPanel = this.mPanel;
            //mAreaManage控件布局
            this.mAreaManage.Location = new System.Drawing.Point(ClientRectangle.Left + 5 + 3, ClientRectangle.Top + 10 + 3 + 36);
            this.mAreaManage.Name = "mAreaManage";

            /**********************************************************************************/
            //选中初级菜单项
            this.primaryMenuItem.Checked = true;
            this.intermediateMenuItem.Checked = false;
            this.seniorMenuItem.Checked = false;
            this.selfDefMenuItem.Checked = false;

            /**********************************************************************************/
            //
            //设置主窗体客户区域大小
            //
            this.ClientSize = new System.Drawing.Size(clientAreaWidth, clientAreaHeight);

            //表情状态按钮点击事件
            mPanel.StusButton.Click += new EventHandler(mAreaManage.StusButton_Click);
        }

        private void 开局ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mAreaManage.TStatus = TotalStatus.haveStart;
            //开启定时器
            mPanel.TmCount.Start();
            mAreaManage.StusButton_Click(sender,e);
            
        }

        private void 初级ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.primaryMenuItem.Checked = true;
            this.intermediateMenuItem.Checked = false;
            this.seniorMenuItem.Checked = false;
            this.selfDefMenuItem.Checked = false;

            rowMines = 9;
            colMines = 9;
            minesCount = 10;
            mineCell = 20;
            clientAreaWidth = colMines * mineCell + borderWidth;
            clientAreaHeight = rowMines * mineCell + borderHeight;

            //重新布局
            layOutAll();
            //设置游戏开始状态
            mAreaManage.TStatus = TotalStatus.haveStart;
            //开启定时器
            mPanel.TmCount.Start();
            //设置雷数
            mPanel.LftMinePanel.Number = minesCount;

            //表情状态按钮点击事件
            mPanel.StusButton.Click += new EventHandler(mAreaManage.StusButton_Click);
        }

        private void 中级ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.primaryMenuItem.Checked = false;
            this.intermediateMenuItem.Checked = true;
            this.seniorMenuItem.Checked = false;
            this.selfDefMenuItem.Checked = false;

            rowMines = 16;
            colMines = 16;
            minesCount = 40;
            mineCell = 20;
            clientAreaWidth = colMines * mineCell + borderWidth;
            clientAreaHeight = rowMines * mineCell + borderHeight;

            //重新布局
            layOutAll();
            //设置游戏开始状态
            mAreaManage.TStatus = TotalStatus.haveStart;
            //开启定时器
            mPanel.TmCount.Start();
            //设置雷数
            mPanel.LftMinePanel.Number = minesCount;

            //表情状态按钮点击事件
            mPanel.StusButton.Click += new EventHandler(mAreaManage.StusButton_Click);
        }

        private void 高级ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.primaryMenuItem.Checked = false;
            this.intermediateMenuItem.Checked = false;
            this.seniorMenuItem.Checked = true;
            this.selfDefMenuItem.Checked = false;

            rowMines = 16;
            colMines = 30;
            minesCount = 100;
            mineCell = 20;
            clientAreaWidth = colMines * mineCell + borderWidth;
            clientAreaHeight = rowMines * mineCell + borderHeight;

            //重新布局
            layOutAll();
            //设置游戏开始状态
            mAreaManage.TStatus = TotalStatus.haveStart;
            //开启定时器
            mPanel.TmCount.Start();
            //设置雷数
            mPanel.LftMinePanel.Number = minesCount;

            //表情状态按钮点击事件
            mPanel.StusButton.Click += new EventHandler(mAreaManage.StusButton_Click);
        }

        private void 自定义ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.primaryMenuItem.Checked = false;
            this.intermediateMenuItem.Checked = false;
            this.seniorMenuItem.Checked = false;
            this.selfDefMenuItem.Checked = true;

            CustomizeMineForm selDefMine = new CustomizeMineForm();
            selDefMine.ShowDialog();

            if(selDefMine.UpdateFlag)
            {
                rowMines = Convert.ToInt32(selDefMine.GHeight);
                colMines =  Convert.ToInt32(selDefMine.GWidth);
                minesCount = Convert.ToInt32(selDefMine.GMineCounts);
                if(rowMines<9)
                {
                    rowMines = 9;
                }
                if (colMines < 9)
                {
                    colMines = 9;
                }
                if(rowMines>30)
                {
                    rowMines = 30;
                }
                if(colMines>40)
                {
                    colMines = 40;
                }
                if(minesCount>rowMines*colMines)
                {
                    minesCount = rowMines * colMines;
                }
                mineCell = 20;
                clientAreaWidth = colMines * mineCell + borderWidth;
                clientAreaHeight = rowMines * mineCell + borderHeight;

                //重新布局
                layOutAll();
                //设置游戏开始状态
                mAreaManage.TStatus = TotalStatus.haveStart;
                //开启定时器
                mPanel.TmCount.Start();
                //设置雷数
                mPanel.LftMinePanel.Number = minesCount;
            }

            //表情状态按钮点击事件
            mPanel.StusButton.Click += new EventHandler(mAreaManage.StusButton_Click);
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();            
        }

        private void mainWin_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }
    }
}
