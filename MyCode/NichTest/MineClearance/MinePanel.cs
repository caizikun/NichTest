using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace NichTest
{
    /// <summary>
    /// LedPanel计时器与剩余雷数显示器
    /// </summary>
    public class time_LftMCount_Panel:Panel
    {

        private const int ledWidth = 13;
        private const int ledHeight = 23;
        private int ledCount = 3;

        public time_LftMCount_Panel()
        {
            //设置双缓冲
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            InitComponent();
            ClientSize = new Size(ledWidth * ledCount, ledHeight);
        }

        protected override void Dispose(bool disposing)
        {

            base.Dispose(disposing);
        }

        private void InitComponent()
        {
            Name = "time_LftMCount_Panel";
        }

        //定义led图像列表
        private ImageList imageList;
        public ImageList ImageList
        {
            get { return imageList; }
            set { imageList = value; }
        }

        //定义计时数
        private int number;
        public int Number
        {
            get { return number; }
            set 
            { 
                if (value >= 0)
				{
                    int maxValue = 999;
					if (value > maxValue)
						value = maxValue;
				}
				else 
				{
                    int minValue = -99;
                    if(value < minValue)
                    {
                        value = -99;
                    }
				}
				if (number != value)
				{
					number = value;
                    //重绘LedPanel控件
					Invalidate();
				}
			}

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle rect = ClientRectangle;
            Graphics g = e.Graphics;
            int reversedNum = 0;

            if (number >= 0)
            {
                //输出三位字符串，若小于三位，则往左用‘0’补齐
                string num = number.ToString().PadLeft(ledCount, '0');
                for (int i = 0; i < ledCount; i++)
                {
                    int j = Convert.ToInt32(num[i]) - 48;
                    imageList.Draw(g, rect.Left + ledWidth * i, rect.Top, j);
                }
            }
            else
            {            
                //如果为负数，则进行反转
                reversedNum = -number;
                //输出两位字符串，若小于两位，则往左用‘0’补齐
                string num = reversedNum.ToString().PadLeft(ledCount - 1, '0');

                //绘制负号
                imageList.Draw(g, rect.Left, rect.Top, 10);

                for (int i = 0; i < ledCount-1; i++)
                {
                    int j = Convert.ToInt32(num[i]) - 48;
                    imageList.Draw(g, rect.Left + ledWidth * (i+1), rect.Top, j);
                }
            }
        }
    }

    /// <summary>
    /// StatusButton状态显示按钮
    /// </summary>
    public class statusButton : Button
    {
        //判断按钮是否被按下
        private bool statusFaceFlag;
        public bool StatusFaceFlag
        {
            get { return statusFaceFlag; }
            set { statusFaceFlag = value; }
        }

        public statusButton()
        {
            //设置双缓冲
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            InitComponent();
            SetStyle(ControlStyles.Selectable, false);
            //BackColor = SystemColors.Control;
            BackColor  = SystemColors.Control;
            ifPressed = false;
            statusFaceFlag = false;
            Width = 26;
            Height = 26;
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                grayBursh.Dispose();
            }

            base.Dispose(disposing);
        }

        private Brush grayBursh;
        private void InitComponent()
        {
            Name = "statusButton";
            grayBursh = new SolidBrush(Color.LightGray);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            Rectangle rect = ClientRectangle;
            Graphics g = pevent.Graphics;

            g.FillRectangle(grayBursh, rect);
            if (Image != null)
            {
                int offset;
                if (ifPressed)
                    offset = 1;
                else
                    offset = 0;

                g.DrawImage(Image,rect.Left+2+offset,rect.Top+2+offset);
            }
        }

        private bool ifPressed;
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);


            if(mevent.Button == MouseButtons.Left)
            {
                ifPressed = true;
                Invalidate();
            }

        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);

            if(mevent.Button == MouseButtons.Left)
            {
                //statusFaceFlag = true;
                ifPressed = false;
                Invalidate();
            }
            
        }
    }

    /// <summary>
    /// minePanel游戏状态显示区域
    /// </summary>
    public class minePanel:Panel
    {
        private statusButton stusButton;
        public statusButton StusButton
        {
            get { return stusButton; }
            set { stusButton = value; }
        }

        private ImageList imgList;

        private time_LftMCount_Panel timePanel;
        public time_LftMCount_Panel TimePanel
        {
            get { return timePanel; }
            set { timePanel = value; }
        }


        private time_LftMCount_Panel lftMinePanel;
        public time_LftMCount_Panel LftMinePanel
        {
            get { return lftMinePanel; }
            set { lftMinePanel = value; }
        }
      
        //定义定时器
        private Timer tmCount;
        public Timer TmCount
        {
            get { return tmCount; }
            set { tmCount = value; }
        }

        public minePanel(int gWidth,int gHeight)
        {
            //设置双缓冲
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            InitComponent();

            SetStyle(ControlStyles.Selectable, false);
            //BackColor = Color.Silver;
            BackColor = SystemColors.Control;
            Width = gWidth;
            Height = gHeight;
            //状态表情
            StatusFace(1);
            //布局子控件
            LayoutChildControls();
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                stusButton.Dispose();
                imgList.Dispose();
                timePanel.Dispose();
                lftMinePanel.Dispose();
                tmCount.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitComponent()
        {
            Name = "minePanel";

            stusButton = new statusButton();
            stusButton.Name = "stusButton";
            stusButton.Text = "";
            stusButton.Parent = this;

            //添加ImageList图片资源
            //imgList = new ImageList();
            //imgList.ImageSize = new Size(13, 23);
            //string FileName = "0123456789";
            //for (int i = 0; i < FileName.Length; i++)
            //{
            //    string filePath = "..\\..\\png\\" + FileName[i] + ".png";
            //    imgList.Images.Add(Image.FromFile(filePath));
            //}

            //添加ImageList图片资源
            imgList = new ImageList();
            imgList.ImageSize = new Size(13, 23);
            imgList.Images.Add(Properties.Resources._0);
            imgList.Images.Add(Properties.Resources._1);
            imgList.Images.Add(Properties.Resources._2);
            imgList.Images.Add(Properties.Resources._3);
            imgList.Images.Add(Properties.Resources._4);
            imgList.Images.Add(Properties.Resources._5);
            imgList.Images.Add(Properties.Resources._6);
            imgList.Images.Add(Properties.Resources._7);
            imgList.Images.Add(Properties.Resources._8);
            imgList.Images.Add(Properties.Resources._9);
            imgList.Images.Add(Properties.Resources._);

            //创建计时器容器
            timePanel = new time_LftMCount_Panel();
            timePanel.Number =0;
            timePanel.Name = "timePanel";
            timePanel.Parent = this;
            timePanel.ImageList = imgList;

            //创建剩余雷数容器
            lftMinePanel = new time_LftMCount_Panel();
            lftMinePanel.Number = 10;
            lftMinePanel.Name = "lftMinePanel";
            lftMinePanel.Parent = this;
            lftMinePanel.ImageList = imgList;

            //创建定时器
            tmCount = new Timer();
            tmCount.Interval = 1000;
            tmCount.Tick += new EventHandler(tmCount_Tick);
            tmCount.Start();
           // tmCount.Stop();
        }

        private void tmCount_Tick(object sender, EventArgs e)
        {
            timePanel.Number = timePanel.Number + 1;
        }

        //设定边框宽度
        private int frameWidth = 20;
        public int FrameWidth
        {
            get { return frameWidth; }
            set { frameWidth = value; }
        }

        /// <summary>
        /// 控制表情状态方法
        /// </summary>
        /// <param name="faceID"></param>
        public void StatusFace(int faceID)
        {
            if (stusButton.Image != null)
            {
                stusButton.Image.Dispose();
                stusButton.Image = null;
            }
            //string facePath = "..\\..\\png\\face" + faceID.ToString() + ".png";
            //string facePath = Properties.Resources.Face1; 
            //stusButton.Image = Image.FromFile(facePath);

            switch (faceID)
            { 
                case 1:
                    stusButton.Image = Properties.Resources.Face1;
                    break;
                case 2:
                    stusButton.Image = Properties.Resources.face2;
                    break;
                case 3:
                    stusButton.Image = Properties.Resources.face3;
                    break;
                case 4:
                    stusButton.Image = Properties.Resources.face4;
                    break;
                default:
                    break;

            }
        }


        /// <summary>
        /// 布局子控件
        /// </summary>
        public void LayoutChildControls()
        {
            Rectangle rect = ClientRectangle;

            //布局剩余雷数显示区域
            lftMinePanel.Left = rect.Left+frameWidth+8;
            lftMinePanel.Top = rect.Top+frameWidth;

            //布局计时器显示区域
            timePanel.Left = rect.Right-frameWidth-13*3-8;
            timePanel.Top = rect.Top+frameWidth;

            //布局状态按钮
            stusButton.Left = rect.Width/2-13;
            stusButton.Top = 5+14;
        }
    }
}
