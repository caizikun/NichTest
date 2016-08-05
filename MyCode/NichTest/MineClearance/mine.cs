using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace NichTest
{
    //定义块标记
    public enum Mark
    {
        //正常标记
        NormalMark,
        //旗帜标记,并且标志正确
        FlagMarkRight,
        //旗帜标记,并且标志错误
        FlagMarkWrong,
        //疑问标记
        DoubtMark
    };

    //定义块状态
    public enum MineStatus
    {
        //正常状态
        NormalStatus,
        //已被展开状态
        DeloyedStatus,
        //疑问标记状态
        DoubtMarkStatus,
        //旗帜标记状态
        FlagMarkStatus
    };

    /// <summary>
    /// 定义单个块数据结构
    /// </summary>
    class Mine
    {
        public Mine()
        { 
        
        }

        public Mine(bool pIsMine,bool pIsOpened,bool pIsMarked)
        {
            isMine = pIsMine;
            isOpened = pIsOpened;
            isMarked = pIsMarked;
        }

         //判定是否有雷
        private bool isMine;
        public bool IsMine
        {
          get { return isMine; }
          set { isMine = value; }
        }

        //判定是否被点开
        private bool isOpened;
        public bool IsOpened
        {
          get { return isOpened; }
          set { isOpened = value; }
        }

        //判定是否被标记
        private bool isMarked;
        public bool IsMarked
        {
          get { return isMarked; }
          set { isMarked = value; }
        }

        //相邻块中有雷的个数
        private int mineAdjoinCount;
        public int MineAdjoinCount
        {
          get { return mineAdjoinCount; }
          set { mineAdjoinCount = value; }
        }

        private Mark mark;
        //获取标记
        public Mark getMark()
        {
            return mark;
        }
        //设置标记
        public void setMark(Mark pMark)
        {
            mark = pMark;
        }

        private MineStatus mineStatus;
        //获取块状态
        public MineStatus getMineStatus()
        {
            return mineStatus;
        }
        //设置块状态
        public void setMineStatus(MineStatus pMineStatus)
        {
            mineStatus = pMineStatus;
        }

    }

    /// <summary>
    /// 创建雷区域
    /// </summary>
    class minesAreas 
    {
        private int areasCols;
        private int areasRows;
        //地雷数
        private int minesCount;

        //定义块数组
        private Mine[,] mines;
        internal Mine[,] Mines
        {
            get { return mines; }
            set { mines = value; }
        }

        //地雷索引
        private ArrayList lstMineIndex;

        public minesAreas(int rows,int cols,int pMinesCount)
        {
            areasCols = cols;
            areasRows = rows;
            minesCount = pMinesCount;

            //创建块数组
            mines = new Mine[rows, cols];
            //创建地雷索引数组
            lstMineIndex = new ArrayList();
            
            //创建雷区
            createMinesAreas();

            //随机布雷
            randomDistributeMines();

            //设置当前块的相邻块中地雷总数
            setMineAdjoinCount();
        }

        /// <summary>
        /// 创建雷区
        /// </summary>
        private void createMinesAreas() 
        {
            for (int r = 0; r < areasRows; r++)
            {
                for (int c = 0; c < areasCols; c++)
                {
                    //创建 mines[r, c]对象
                    mines[r, c] = new Mine();
                    mines[r, c].IsMine = false;
                    mines[r, c].IsOpened = false;
                    mines[r, c].IsMarked = false;
                    mines[r, c].setMark(Mark.NormalMark);
                    mines[r, c].setMineStatus(MineStatus.NormalStatus);
                }
            }
        }

        /// <summary>
        /// 随机布雷
        /// </summary>
        private void randomDistributeMines()
        {
            //系统自动选取当前时间作为随机种子
            Random ro = new Random();
            int up = areasRows * areasCols;

            //清除地雷链表
            lstMineIndex.Clear();

            //如果链表内存放的雷数不等于minesCount时
            while (lstMineIndex.Count != minesCount)
            {
                int result;

                if (lstMineIndex.Count > minesCount)
                {
                    Debug.WriteLine("布雷失败！");
                    return;
                }

                result = ro.Next(up);
                //若链表中不包含该随机数，则加入链表
                if(!lstMineIndex.Contains(result))
                {
                    lstMineIndex.Add(result);
                }
            }

            //取得随机数
            if (lstMineIndex.Count == minesCount)
            {
                int index;
                for (int i = 0; i < lstMineIndex.Count;i++ )
                {

                    index = (int)lstMineIndex[i];
                    mines[index / areasCols, index % areasCols].IsMine = true;
                }
            }
        }

        /// <summary>
        /// 设置当前块的相邻块中地雷总数
        /// </summary>
        private void setMineAdjoinCount()
        {
            for(int r = 0; r<areasRows;r++)
            {
                for (int c = 0; c < areasCols; c++)
                {
                    int count=0;
                    //若该块为有雷块
                    if(!mines[r,c].IsMine)
                    {
                        if(hasMine(r - 1,c - 1))
                        {
                            count++;
                        }
                        if (hasMine(r - 1, c))
                        {
                            count++;
                        }
                        if (hasMine(r - 1, c + 1))
                        {
                            count++;
                        }
                        if (hasMine(r, c - 1))
                        {
                            count++;
                        }
                        if (hasMine(r, c + 1))
                        {
                            count++;
                        }
                        if (hasMine(r + 1, c - 1))
                        {
                            count++;
                        }
                        if (hasMine(r + 1, c))
                        {
                            count++;
                        }
                        if (hasMine(r + 1, c + 1))
                        {
                            count++;
                        }
                    }

                    //设置有雷块数
                    mines[r, c].MineAdjoinCount = count;
                }
            }
        }

        private bool hasMine(int row,int col)
        {
            if(indexIsValidate(row,col))
            {
                if (mines[row, col].IsMine)
                    return true;

                else
                    return false;
            }
            else
                return false;
        }

        private bool indexIsValidate(int row, int col)
        {
            if (row>=0 && row < areasRows && col >= 0 && col < areasCols)
                return true;
            else
                return false;
        }
    }
}
