using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Threading;

namespace NichTest
{
    //雷区总状态
    public enum TotalStatus
    {
        haveStart,
        notHaveStart,
        notHaveFinished,
        haveFinished,
        exploding
    }

    class MineAreaManage:Control
    {
        private int mineCell = 20;
        private int areaRow;
        private int areaCol;
        private int mineCount;
        private minesAreas mAreas;
        //定义块数组
        private Mine[,] mines;
        //用于存放鼠标点击的位置
        private int rMouse, cMouse;
        //鼠标左键按下状态
        private bool leftMousePressStat;
        //鼠标右键按下状态
        private bool rightMousePressStat;
        //鼠标左右键同时按下状态
        private bool leftAndRightMousePressStat;
        //存放爆炸点的坐标位置
        private int rExploding, cExploding;
        //鼠标悬浮标志
        private bool mouseHoverFlag;

        //存放mPanel对象
        private minePanel mPanel;
        public minePanel MPanel
        {
            get { return mPanel; }
            set { mPanel = value; }
        }

        //雷区总状态
        private TotalStatus tStatus;
        public TotalStatus TStatus
        {
            get { return tStatus; }
            set { tStatus = value; }
        }

        public MineAreaManage(int gRow,int gCol,int gMineCount)
        {
            //设置双缓冲
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint |ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
            //支持设置透明度
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            leftMousePressStat = false;
            areaRow = gRow;
            areaCol = gCol;
            mineCount = gMineCount;

            //控件高度与宽度
            Width = gCol * mineCell;
            Height = gRow * mineCell;

            mouseHoverFlag = false;
            leftMousePressStat = false;
            rightMousePressStat = false;
            leftAndRightMousePressStat = false;

            //将游戏状态设置为开始
            tStatus = TotalStatus.haveStart;
            //初始化雷区
            initMines(gRow, gCol, gMineCount);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        //表情状态按钮点击事件
        public void StusButton_Click(object sender, EventArgs e)
        {
            //初始化雷区
            initMines(areaRow,areaCol, mineCount);
            tStatus = TotalStatus.haveStart;
            mPanel.TmCount.Start();
            mPanel.TimePanel.Number = 0;
            mPanel.LftMinePanel.Number = mineCount;
            mPanel.StatusFace(1);
           // mPanel.TmCount
            Invalidate();
        }

        //初始化雷区
        private void initMines(int gRow,int gCol,int gMineCount)
        {
            if (mAreas != null)
            {
                mAreas = null;

                //创建雷区对象
                mAreas = new minesAreas(gRow, gCol, gMineCount);
                //获取存放雷块的二维数组
                mines = mAreas.Mines;
            }
            else
            {
                //创建雷区对象
                mAreas = new minesAreas(gRow, gCol, gMineCount);
                //获取存放雷块的二维数组
                mines = mAreas.Mines;
            }
        }

        /********************************************************************************************/
        //htjiao/2016-7-26
        //重绘
        /********************************************************************************************/
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle rect = ClientRectangle;
            Graphics g = e.Graphics;

            //游戏失败
            if(tStatus==TotalStatus.exploding)
            {
                //设置表情状态按钮
                mPanel.StatusFace(3);

                //关闭定时器
                mPanel.TmCount.Stop();

                for (int i = 0; i < areaRow;i++ )
                {
                    for (int j = 0; j < areaCol;j++ )
                    {
                        //若该块为地雷块，且未被标记
                        if(mines[i,j].IsMine&&!mines[i,j].IsMarked)
                        {
                            if(!(i == rExploding && j == cExploding))
                                 //g.DrawImage(Image.FromFile("..\\..\\png\\notFound.png"), new Rectangle(rect.Left + j * mineCell+1, rect.Top + i * mineCell+1, mineCell, mineCell));
                                g.DrawImage(Properties.Resources.notFound, new Rectangle(rect.Left + j * mineCell + 1, rect.Top + i * mineCell + 1, mineCell, mineCell));
                        }
                        //标记错误的块
                        if (mines[i, j].getMark() == Mark.FlagMarkWrong)
                        {
                            if (!(i == rExploding && j == cExploding))
                                //g.DrawImage(Image.FromFile("..\\..\\png\\markWrong.png"), new Rectangle(rect.Left + j * mineCell+1, rect.Top + i * mineCell+1, mineCell, mineCell));
                                g.DrawImage(Properties.Resources.markWrong, new Rectangle(rect.Left + j * mineCell + 1, rect.Top + i * mineCell + 1, mineCell, mineCell));
                        }
                        if (i == rExploding && j == cExploding)
                        {
                            //当前爆炸点雷块
                            //g.DrawImage(Image.FromFile("..\\..\\png\\exploding.png"), new Rectangle(rect.Left + j * mineCell+1, rect.Top + i * mineCell+1, mineCell, mineCell));
                            g.DrawImage(Properties.Resources.exploding, new Rectangle(rect.Left + j * mineCell + 1, rect.Top + i * mineCell + 1, mineCell, mineCell));
                        }
                     
                        //若该雷块被挖开，同时也设置了打开标志
                        if (mines[i, j].getMineStatus() == MineStatus.DeloyedStatus && mines[i, j].IsOpened)
                        {
                            g.DrawString(mines[i, j].MineAdjoinCount.ToString(), new Font("宋体", 15, FontStyle.Bold), new SolidBrush(Color.Red), new Rectangle(rect.Left + j * mineCell + 2, rect.Top + i * mineCell, mineCell, mineCell));
                        }
                        //若该雷块被挖开，但未设置打开标志（在雷块周围地雷数为0时，会出现这种情况）,同时该块不是地雷块
                        else if (mines[i, j].getMineStatus() == MineStatus.DeloyedStatus && !mines[i, j].IsOpened && !mines[i, j].IsMine)
                        {
                            g.FillRectangle(new SolidBrush(Color.LightGray), new Rectangle(rect.Left + j * mineCell, rect.Top + i * mineCell, mineCell, mineCell));
                        }
                        //当前雷块为疑问标记状态时
                        if (mines[i, j].getMineStatus() == MineStatus.DoubtMarkStatus&&!mines[i,j].IsMine)
                        {
                            g.DrawString("?", new Font("宋体", 15, FontStyle.Bold), new SolidBrush(Color.Red), new Rectangle(rect.Left + j * mineCell + 2, rect.Top + i * mineCell, mineCell, mineCell));
                        }
                        //当前雷块为旗帜标记状态时,且标记正确
                        if (mines[i, j].getMineStatus() == MineStatus.FlagMarkStatus&&mines[i,j].getMark()==Mark.FlagMarkRight)
                        {
                            //g.DrawImage(Image.FromFile("..\\..\\png\\flag.png"), new Rectangle(rect.Left + j * mineCell+1, rect.Top + i * mineCell+1, mineCell, mineCell));
                            g.DrawImage(Properties.Resources.flag, new Rectangle(rect.Left + j * mineCell + 1, rect.Top + i * mineCell + 1, mineCell, mineCell));
                        }
                        drawMine(g, rect, i, j);
                    }
                }
            }
            //游戏未结束
            else if(tStatus == TotalStatus.notHaveFinished||tStatus==TotalStatus.notHaveStart||tStatus==TotalStatus.haveStart)
            {
                //增加鼠标悬浮效果
                if (mouseHoverFlag&&!mines[rMouse,cMouse].IsOpened)
                {
                    g.FillRectangle(new SolidBrush(Color.SandyBrown), new Rectangle(rect.Left + cMouse * mineCell, rect.Top + rMouse * mineCell, mineCell, mineCell));
                }

                //当左键按下时，当前块呈现下陷效果
                if (leftMousePressStat && !leftAndRightMousePressStat)
                {
                    g.FillRectangle(new SolidBrush(Color.LightGray), new Rectangle(rect.Left + cMouse * mineCell, rect.Top + rMouse * mineCell, mineCell, mineCell));
                }

                //当左右键同时按下某个雷块时，九宫格呈现下陷效果（除周围地雷数为0且该块不为地雷的块不用实现该效果）
                if (leftAndRightMousePressStat &&
                    !((mines[rMouse, cMouse].getMineStatus() == MineStatus.DeloyedStatus) && (!mines[rMouse, cMouse].IsOpened)))//已挖开，但打开标志未设置，表明该块为周围地雷数为0的块
                {
                    if ((rMouse - 1) >= 0 && (cMouse - 1) >= 0)
                        g.FillRectangle(new SolidBrush(Color.LightGray), new Rectangle(rect.Left + (cMouse - 1) * mineCell, rect.Top + (rMouse - 1) * mineCell, mineCell, mineCell));
                    if ((rMouse - 1) >= 0 && cMouse >= 0)
                        g.FillRectangle(new SolidBrush(Color.LightGray), new Rectangle(rect.Left + cMouse * mineCell, rect.Top + (rMouse - 1) * mineCell, mineCell, mineCell));
                    if ((rMouse - 1) >= 0 && (cMouse + 1) < areaCol)
                        g.FillRectangle(new SolidBrush(Color.LightGray), new Rectangle(rect.Left + (cMouse + 1) * mineCell, rect.Top + (rMouse - 1) * mineCell, mineCell, mineCell));
                    if (rMouse >= 0 && (cMouse - 1) >= 0)
                        g.FillRectangle(new SolidBrush(Color.LightGray), new Rectangle(rect.Left + (cMouse - 1) * mineCell, rect.Top + rMouse * mineCell, mineCell, mineCell));
                    if (rMouse >= 0 && cMouse >= 0)
                        g.FillRectangle(new SolidBrush(Color.LightGray), new Rectangle(rect.Left + cMouse * mineCell, rect.Top + rMouse * mineCell, mineCell, mineCell));
                    if (rMouse >= 0 && (cMouse + 1) < areaCol)
                        g.FillRectangle(new SolidBrush(Color.LightGray), new Rectangle(rect.Left + (cMouse + 1) * mineCell, rect.Top + rMouse * mineCell, mineCell, mineCell));
                    if ((rMouse + 1) < areaRow && (cMouse - 1) >= 0)
                        g.FillRectangle(new SolidBrush(Color.LightGray), new Rectangle(rect.Left + (cMouse - 1) * mineCell, rect.Top + (rMouse + 1) * mineCell, mineCell, mineCell));
                    if ((rMouse + 1) < areaRow && cMouse >= 0)
                        g.FillRectangle(new SolidBrush(Color.LightGray), new Rectangle(rect.Left + cMouse * mineCell, rect.Top + (rMouse + 1) * mineCell, mineCell, mineCell));
                    if ((rMouse + 1) < areaRow && (cMouse + 1) < areaCol)
                        g.FillRectangle(new SolidBrush(Color.LightGray), new Rectangle(rect.Left + (cMouse + 1) * mineCell, rect.Top + (rMouse + 1) * mineCell, mineCell, mineCell));
                }

                for (int r = 0; r < areaRow; r++)
                {
                    for (int c = 0; c < areaCol; c++)
                    {
                        //if (mines[r, c].IsMine)
                        //{
                        //    g.DrawString("A", new Font("宋体", 15, FontStyle.Bold), new SolidBrush(Color.Red), new Rectangle(rect.Left + c * mineCell + 2, rect.Top + r * mineCell, mineCell, mineCell));
                        //}
                        //若该雷块被挖开，同时也设置了打开标志
                        if (mines[r, c].getMineStatus() == MineStatus.DeloyedStatus && mines[r, c].IsOpened)
                        {
                            g.DrawString(mines[r, c].MineAdjoinCount.ToString(), new Font("宋体", 15, FontStyle.Bold), new SolidBrush(Color.Red), new Rectangle(rect.Left + c * mineCell + 2, rect.Top + r * mineCell, mineCell, mineCell));
                        }
                        //若该雷块被挖开，但未设置打开标志（在雷块周围地雷数为0时，会出现这种情况）,同时该块不是地雷块
                        else if (mines[r, c].getMineStatus() == MineStatus.DeloyedStatus && !mines[r, c].IsOpened && !mines[r, c].IsMine)
                        {
                            g.FillRectangle(new SolidBrush(Color.LightGray), new Rectangle(rect.Left + c * mineCell, rect.Top + r * mineCell, mineCell, mineCell));
                        }
                        //当前雷块为疑问标记状态时
                        if (mines[r, c].getMineStatus() == MineStatus.DoubtMarkStatus)
                        {
                            g.DrawString("?", new Font("宋体", 15, FontStyle.Bold), new SolidBrush(Color.Red), new Rectangle(rect.Left + c * mineCell + 2, rect.Top + r * mineCell, mineCell, mineCell));
                        }
                        //当前雷块为旗帜标记状态时
                        if (mines[r, c].getMineStatus() == MineStatus.FlagMarkStatus)
                        {
                           // g.DrawImage(Image.FromFile("..\\..\\png\\flag.png"), new Rectangle(rect.Left + c * mineCell+1, rect.Top + r * mineCell+1, mineCell, mineCell));
                            g.DrawImage(Properties.Resources.flag, new Rectangle(rect.Left + c * mineCell + 1, rect.Top + r * mineCell + 1, mineCell, mineCell));
                        }
                        //当前雷块为正常标记状态时
                        if (mines[r, c].getMineStatus() == MineStatus.NormalStatus)
                        {
                            g.DrawString("", new Font("宋体", 15, FontStyle.Bold), new SolidBrush(Color.Red), new Rectangle(rect.Left + c * mineCell + 2, rect.Top + r * mineCell, mineCell, mineCell));
                        }
                        drawMine(g, rect, r, c);
                    }
                }
            }
            //游戏成功
            else if(tStatus == TotalStatus.haveFinished)
            {
                //设置表情状态按钮
                mPanel.StatusFace(4);

                //关闭定时器
                mPanel.TmCount.Stop();
            }
        }

        //绘制单块边框
        private void drawMine(Graphics g, Rectangle rect,int row,int col)
        {

            //绘制块边框
            g.DrawRectangle(new Pen(Color.Gray), new Rectangle(rect.Left+col*mineCell, rect.Top+row*mineCell, mineCell, mineCell));
            g.DrawRectangle(new Pen(Color.LightGray), new Rectangle(rect.Left + col * mineCell + 1, rect.Top + row * mineCell + 1, mineCell-2, mineCell-2));
            g.DrawRectangle(new Pen(Color.LightGray), new Rectangle(rect.Left + col * mineCell + 2, rect.Top + row * mineCell + 2, mineCell - 4, mineCell - 4));
        }


        /********************************************************************************************/
        //htjiao/2016-7-26
        //递归判断
        /********************************************************************************************/
        //递归判断相邻雷块
        private void recurJudge(int r, int c)
        {
            if ((r - 1) >= 0 && (c - 1) >= 0)
            {
                //如果该块未被挖开，同时打开标志未设置
                if(!mines[r-1,c-1].IsOpened&&!(mines[r-1,c-1].getMineStatus()==MineStatus.DeloyedStatus))
                {
                     //设置该雷块被挖开，但打开标志IsOpen未设置（IsOpened未设置）
                    mines[r - 1, c - 1].setMineStatus(MineStatus.DeloyedStatus);
                    //如果不展开，则继续递归判断
                    if (ifRecursion(r - 1, c - 1))
                        recurJudge(r - 1, c - 1);
                }

            }
            if ((r - 1) >= 0 && c >= 0)
            {
                if (!mines[r - 1, c].IsOpened && !(mines[r - 1, c].getMineStatus() == MineStatus.DeloyedStatus))
                {
                    //设置该雷块被挖开，但打开标志未设置（IsOpened未设置）
                    mines[r - 1, c].setMineStatus(MineStatus.DeloyedStatus);
                    if (ifRecursion(r - 1, c))
                        recurJudge(r - 1, c);
                }
            }
            if ((r - 1) >= 0 && (c + 1) < areaCol)
            {
                if (!mines[r - 1, c+1].IsOpened && !(mines[r - 1, c+1].getMineStatus() == MineStatus.DeloyedStatus))
                {
                    //设置该雷块被挖开，但打开标志未设置（IsOpened未设置）
                    mines[r - 1, c+1].setMineStatus(MineStatus.DeloyedStatus);
                    if (ifRecursion(r - 1, c + 1))
                        recurJudge(r - 1, c + 1);
                }
            }
            if (r >= 0 && (c - 1) >= 0)
            {
                if (!mines[r, c - 1].IsOpened && !(mines[r, c - 1].getMineStatus() == MineStatus.DeloyedStatus))
                {
                    //设置该雷块被挖开，但打开标志未设置（IsOpened未设置）
                    mines[r, c - 1].setMineStatus(MineStatus.DeloyedStatus);
                    if (ifRecursion(r, c - 1))
                        recurJudge(r, c - 1);
                }
            }
            if (r >= 0 && (c + 1) < areaCol)
            {
                if (!mines[r, c + 1].IsOpened && !(mines[r, c + 1].getMineStatus() == MineStatus.DeloyedStatus))
                {
                    //设置该雷块被挖开，但打开标志未设置（IsOpened未设置）
                    mines[r, c + 1].setMineStatus(MineStatus.DeloyedStatus);
                    if (ifRecursion(r, c + 1))
                        recurJudge(r, c + 1);
                }
            }
            if ((r + 1) < areaRow && (c - 1) >= 0)
            {
                if (!mines[r+1, c - 1].IsOpened && !(mines[r+1, c - 1].getMineStatus() == MineStatus.DeloyedStatus))
                {
                    //设置该雷块被挖开，但打开标志未设置（IsOpened未设置）
                    mines[r+1, c - 1].setMineStatus(MineStatus.DeloyedStatus);
                    if (ifRecursion(r + 1, c - 1))
                        recurJudge(r + 1, c - 1);
                }
            }
            if ((r + 1) < areaRow && c >= 0)
            {
                if (!mines[r + 1, c].IsOpened && !(mines[r + 1, c].getMineStatus() == MineStatus.DeloyedStatus))
                {
                    //设置该雷块被挖开，但打开标志未设置（IsOpened未设置）
                    mines[r + 1, c ].setMineStatus(MineStatus.DeloyedStatus);
                    if (ifRecursion(r + 1, c))
                        recurJudge(r + 1, c);
                }
            }
            if ((r + 1) < areaRow && (c + 1) < areaCol)
            {
                if (!mines[r + 1, c + 1].IsOpened && !(mines[r + 1, c + 1].getMineStatus() == MineStatus.DeloyedStatus))
                {
                    //设置该雷块被挖开，但打开标志未设置（IsOpened未设置）
                    mines[r + 1, c + 1].setMineStatus(MineStatus.DeloyedStatus);
                    if (ifRecursion(r + 1, c + 1))
                        recurJudge(r + 1, c + 1);
                }
            }
        }

        //判断是否要展开
        private bool ifRecursion(int r, int c)
        {
            if(!mines[r,c].IsOpened)
            {
                if (!mines[r, c].IsMarked)
                {
                    if (mines[r, c].MineAdjoinCount > 0)
                    {
                        //如果未被打开，且未被标记，同时周围有地雷，则直接展开
                        mines[r, c].setMineStatus(MineStatus.DeloyedStatus);
                        //设置该块已被打开
                        mines[r, c].IsOpened = true;
                        return false;
                    }
                    else  //当当前块周围的地雷数为0时，返回true，表示继续递归
                    {
                        return true;
                    }
                }
                else
                {
                    //若该块被标记，则设置其相应状态位
                    mines[r, c].setMineStatus(MineStatus.FlagMarkStatus);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /********************************************************************************************/
        //htjiao/2016-7-26
        //鼠标悬浮事件
        /********************************************************************************************/
        private int prevRow;
        private int prevCol;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            ////获取该控件在屏幕坐标系内的坐标
            //Point p = new Point(0, 0);     //   0,0   是屏幕左上角坐标 
            //p = this.PointToScreen(p);
            //p = this.PointToClient(p);

            Point p = new Point(0, 0);
            //获取鼠标在屏幕坐标系内的坐标
            p = Control.MousePosition;
            //将屏幕坐标系内的坐标转换为控件坐标系内的坐标
            p = this.PointToClient(p);

            //获取客户区矩阵
            Rectangle rect = ClientRectangle;
            int r = p.Y / mineCell;
            int c = p.X / mineCell;


            if (r != prevRow || c != prevCol)
            {
                mouseHoverFlag = true;
                rMouse = r;
                cMouse = c;
                Invalidate();
            }

            prevRow = r;
            prevCol = c;

            base.OnMouseMove(e);
        }
        /********************************************************************************************/
        //htjiao/2016-7-26
        //鼠标点击事件
        /********************************************************************************************/
        //用于控制左右键同时发生事件
        private MouseButtons oldButton;
        private long oldTicks;                  // 1个Ticks = 100毫微秒
        private long TicksDelay = 5000000L;  // 5000000 Ticks = 500毫秒

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
         
            Rectangle rect = ClientRectangle;
            rMouse = e.Y / mineCell;
            cMouse = e.X / mineCell;

            mouseHoverFlag = false;
            /*****************************************************************************************/
            //htjiao/2016-7-26
            //实现了同时按下左键与右键的操作，主要通过标志位来实现
            //当rightMousePressStat=true&&leftAndRightMousePressStat=true
            //或者leftAndRightMousePressStat=true&&leftAndRightMousePressStat=true时，表明左右键同时按下了
            //只有当游戏按下开始键后才开始
            if (tStatus != TotalStatus.notHaveStart && tStatus != TotalStatus.haveFinished && tStatus != TotalStatus.exploding)
            {
                mPanel.StatusFace(2);

                if (e.Button == (MouseButtons.Left | MouseButtons.Right))
                {
                    leftAndRightMousePressStat = true;
                }
                else if (e.Button == MouseButtons.Left)
                {
                    //表明同时按下了左右键
                    if ((DateTime.Now.Ticks - oldTicks) < TicksDelay && oldButton == MouseButtons.Right)
                    {
                        leftAndRightMousePressStat = true;
                    }
                    //表明只按下了左键
                    else
                    {
                        //该块未被打开
                        if (!mines[rMouse, cMouse].IsOpened)
                            leftMousePressStat = true;
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    //表明同时按下了左右键
                    if ((DateTime.Now.Ticks - oldTicks) < TicksDelay && oldButton == MouseButtons.Left)
                    {
                        leftAndRightMousePressStat = true;
                    }
                    //表明只按下了右键
                    else
                    {
                        //该块未被打开
                        if (!mines[rMouse, cMouse].IsOpened)
                            rightMousePressStat = true;
                    }
                }

                oldButton = e.Button;
                oldTicks = DateTime.Now.Ticks;

                //重绘，用于实现点击后的下陷效果
                Invalidate();
            }      
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            //只有当游戏按下开始键后才开始
            if (tStatus != TotalStatus.notHaveStart&&tStatus!=TotalStatus.haveFinished&&tStatus!=TotalStatus.exploding)
            {
                mPanel.StatusFace(1);

                //左键弹开
                //if (e.Button == MouseButtons.Left)
                if(leftMousePressStat&&!leftAndRightMousePressStat)
                {
                    leftMousePressStat = false;

                    //判定雷块未被打开
                    if (!mines[rMouse, cMouse].IsOpened)
                    {
                        //判定雷块未被标记（带问号的块或正常块）
                        if (!mines[rMouse, cMouse].IsMarked)
                        {
                            //若该雷块有雷，则游戏失败
                            if (mines[rMouse, cMouse].IsMine)
                            {
                                rExploding = rMouse;
                                cExploding = cMouse;
                                tStatus = TotalStatus.exploding;
                            }
                            //若无雷
                            else
                            {
                                //若相邻有雷块数大于0
                                if (mines[rMouse, cMouse].MineAdjoinCount > 0)
                                {
                                    //设置 mines[r, c]为能被展开
                                    mines[rMouse, cMouse].setMineStatus(MineStatus.DeloyedStatus);
                                    mines[rMouse, cMouse].IsOpened = true;
                                }
                                //若相邻有雷块数为0
                                else if (mines[rMouse, cMouse].MineAdjoinCount == 0)
                                {
                                    //设置当前块的状态为被挖开状态，但打开标志IsOpen未设置（在雷块周围地雷数为0时，会出现这种情况）
                                    mines[rMouse, cMouse].setMineStatus(MineStatus.DeloyedStatus);

                                    //递归判断相邻雷块
                                    recurJudge(rMouse, cMouse);
                                }
                            }
                        }
                    }
                }
                //右键弹开
               // else if (e.Button == MouseButtons.Right)
                else if(rightMousePressStat&&!leftAndRightMousePressStat)
                {
                    if (!mines[rMouse, cMouse].IsOpened)
                    {
                        //如果已被标记
                        if (mines[rMouse, cMouse].IsMarked)
                        {
                            mines[rMouse, cMouse].setMark(Mark.DoubtMark);
                            mines[rMouse, cMouse].setMineStatus(MineStatus.DoubtMarkStatus);
                            mines[rMouse, cMouse].IsMarked = false;
                            mPanel.LftMinePanel.Number++;
                        }
                        else
                        {
                            //如果是疑问标记
                            if (mines[rMouse, cMouse].getMark() == Mark.DoubtMark)
                            {
                                mines[rMouse, cMouse].setMark(Mark.NormalMark);
                                mines[rMouse, cMouse].setMineStatus(MineStatus.NormalStatus);
                            }
                            else //如果是正常情况
                            {
                                //已被挖开或打开标志已设置的不用进行标记
                                if(!(mines[rMouse,cMouse].getMineStatus()==MineStatus.DeloyedStatus))
                                {
                                    //若该块有地雷，则表示标记正确
                                    if (mines[rMouse, cMouse].IsMine)
                                    {
                                        mines[rMouse, cMouse].setMark(Mark.FlagMarkRight);
                                        mines[rMouse, cMouse].setMineStatus(MineStatus.FlagMarkStatus);
                                        mines[rMouse, cMouse].IsMarked = true;

                                        mPanel.LftMinePanel.Number--;
                                    }
                                    else
                                    {
                                        mines[rMouse, cMouse].setMark(Mark.FlagMarkWrong);
                                        mines[rMouse, cMouse].setMineStatus(MineStatus.FlagMarkStatus);
                                        mines[rMouse, cMouse].IsMarked = true;

                                        mPanel.LftMinePanel.Number--;
                                    }
                                }
                            }
                        }
                    }
                    rightMousePressStat = false;
                }
                //同时弹开
                //else if (e.Button == (MouseButtons.Left| MouseButtons.Right))
                else if(leftAndRightMousePressStat)
                {
                    //该雷块已打开
                    if (mines[rMouse, cMouse].IsOpened)
                    {
                        //求取块周围已标记的块数
                        int markedCounts = haveMarkedCount(rMouse, cMouse);
                        //如果周围的已标记块数与周围的雷数相同
                        if(markedCounts==mines[rMouse,cMouse].MineAdjoinCount)
                        {
                            //如果有标记错误的块
                            if (ifHaveMarkedWrongMine(rMouse, cMouse))
                            {
                                tStatus = TotalStatus.exploding;
                            }
                            else//没有标记错误的块，则递归展开
                            {
                                //递归判断周围雷块
                                recurJudge(rMouse,cMouse);
                            }
                        }
                    }

                    leftMousePressStat = false;
                    rightMousePressStat = false;
                    leftAndRightMousePressStat = false;
                }

                //检测所有的雷块，并设置全局状态
                checkAllMines();
                Invalidate();
            }
        }

        //检测所有雷块，并设置全局状态
        private void checkAllMines()
        {
            int count=0;
            //当前状态不为爆炸状态
            if(tStatus!=TotalStatus.exploding)
            {
                for (int r = 0; r < areaRow; r++)
                {
                    for (int c = 0; c < areaCol; c++)
                    {
                        //如果仍存在normalStatus,doubtStatus和标记错误的块，则表示游戏仍未结束
                        if (mines[r, c].getMineStatus() == MineStatus.NormalStatus || mines[r, c].getMineStatus() == MineStatus.DoubtMarkStatus || mines[r, c].getMark() == Mark.FlagMarkWrong)
                        {
                            tStatus = TotalStatus.notHaveFinished;
                            return;
                        }
                        count++;
                    }
                }
                if (count == areaCol * areaRow)
                {
                    //游戏成功
                    tStatus = TotalStatus.haveFinished;
                }
            }
        }

        //判断周围是否有标记错误的块
        private bool ifHaveMarkedWrongMine(int r,int c)
        {
            if ((r - 1) >= 0 && (c - 1) >= 0)
            {
                if (mines[r - 1, c - 1].IsMarked && mines[r - 1, c - 1].getMark() == Mark.FlagMarkWrong)
                {
                    rExploding = r - 1;
                    cExploding = c - 1;
                    return true;
                }
            }
            if ((r - 1) >= 0 && c >= 0)
            {
                rExploding = r - 1;
                cExploding = c ;
                if (mines[r - 1, c].IsMarked && mines[r - 1, c].getMark() == Mark.FlagMarkWrong)
                    return true;
            }
            if ((r - 1) >= 0 && (c + 1) < areaCol)
            {
                rExploding = r - 1;
                cExploding = c + 1;
                if (mines[r - 1, c + 1].IsMarked && mines[r - 1, c + 1].getMark() == Mark.FlagMarkWrong)
                    return true;   
            }
            if (r >= 0 && (c - 1) >= 0)
            {
                rExploding = r ;
                cExploding = c - 1;
                if (mines[r, c - 1].IsMarked && mines[r, c - 1].getMark() == Mark.FlagMarkWrong)
                    return true;
            }
            if (r >= 0 && (c + 1) < areaCol)
            {
                rExploding = r ;
                cExploding = c + 1;
                if (mines[r, c + 1].IsMarked && mines[r, c + 1].getMark() == Mark.FlagMarkWrong)
                    return true;
            }
            if ((r + 1) < areaRow && (c - 1) >= 0)
            {
                rExploding = r + 1;
                cExploding = c - 1;
                if (mines[r + 1, c - 1].IsMarked && mines[r + 1, c - 1].getMark() == Mark.FlagMarkWrong)
                    return true;
            }
            if ((r + 1) < areaRow && c >= 0)
            {
                rExploding = r + 1;
                cExploding = c ;
                if (mines[r + 1, c].IsMarked && mines[r + 1, c].getMark() == Mark.FlagMarkWrong)
                    return true;
            }
            if ((r + 1) < areaRow && (c + 1) < areaCol)
            {
                rExploding = r + 1;
                cExploding = c + 1;
                if (mines[r + 1, c + 1].IsMarked && mines[r + 1, c + 1].getMark() == Mark.FlagMarkWrong)
                    return true;
            }

            return false;
        }

        //求取周围已被标记的块数
        private int haveMarkedCount(int r,int c)
        {
            int count = 0;
            if ((r - 1) >= 0 && (c - 1) >= 0)
            {
                if (mines[r - 1, c - 1].IsMarked)
                    count++;
            }
            if ((r - 1) >= 0 && c >= 0)
            {
                if (mines[r - 1, c].IsMarked)
                    count++;
            }
            if ((r - 1) >= 0 && (c + 1) < areaCol)
            {
                if (mines[r - 1, c + 1].IsMarked)
                    count++;
            }
            if (r >= 0 && (c - 1) >= 0)
            {
                if (mines[r , c - 1].IsMarked)
                    count++;
            }
            if (r >= 0 && (c + 1) < areaCol)
            {
                if (mines[r , c + 1].IsMarked)
                    count++;
            }
            if ((r + 1) < areaRow && (c - 1) >= 0)
            {
                if (mines[r + 1, c - 1].IsMarked)
                    count++;
            }
            if ((r + 1) < areaRow && c >= 0)
            {
                if (mines[r + 1, c].IsMarked)
                    count++;
            }
            if ((r + 1) < areaRow && (c + 1) < areaCol)
            {
                if (mines[r + 1, c + 1].IsMarked)
                    count++;
            }

            return count;
        }
    }
}
