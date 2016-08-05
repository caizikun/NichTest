using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace NichTest
{
    public class DataIO:IDisposable //基类 虚方法!
    {
        public DataIO()
        {
        }

        #region Leo
        public virtual bool OpenDatabase(bool Swith) { return true; }
        public virtual bool WriterLog(int ConditionID, int SNID, string StrInf, string StartTime, string EndTime, float Temp, float Voltage, byte Channel, bool Resultlflag, out int logid) { logid = -1; return true; }
        public virtual bool WriteResult(int StrInterfaceLogid, DataTable ResultDataTabel) { return true; }
        public virtual DataTable GetDataTable(string sqlCmd, string StrTableName)
        {
            return null;
        }
      //  public virtual bool WriterSN(int TestPlanID, string StrSN,string StrFwRev,string StrIPaddress,string strLightSourceMessage, out int snid) { snid = -1; return true; }
        public virtual bool WriterSN(int TestPlanID, string StrSN, string StrFwRev, string StrIPaddress, string strLightSourceMessage,string StrTestRemark, out int snid) { snid = -1; return true; }

 #region  Dispose()

        // Public implementation of Dispose pattern callable by consumers.
        bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }
        #endregion
        
        #endregion

        public virtual long GetLastInsertData(string tableName)
        {
            return -1;
        }

        public virtual bool UpdateDT()
        {
            return true;
        }

        public virtual bool UpdateDataTable(string sqlCmd, DataTable dt) //140722重新启用 //140612_1
        {
            return true;
        }

        public virtual long GetPID(string sqlCmd)
        {
            return -1;
        }

        public virtual bool BlnISExistTable(string tableName)
        {
            return true;
        }

        public virtual string[] GetCurrTablesName(string serverName, string dbName, string userName, string pwd)
        {
            return null;
        }
        public virtual string[] GetCurrTablesName(string Accesspath)
        {
            return null;
        }

        public virtual bool UpdateDataTable(string sqlCmd, DataTable dt, bool IsAddNewData)
        {
            return true;
        }

        public virtual DateTime GetCurrTime()
        {
            return DateTime.Parse("2000/1/1 12:00:00");
        }
    }


    //完全的公共部分开始!++++++++++++++++++++++++++++++++++++++  //140722
    public class ServerDatabaseIO : DataIO	//SQL子类 2个公共的方法!
    {
        protected SqlConnection conn;

        /// <summary>
        /// 进行加密和解密操作!!!
        /// </summary>
        /// <param name="str">待加密或解密的字符串!</param>
        /// <param name="decode">解密? true =解密|false=加密</param>
        /// <param name="Codelength">1位密码的位元长度</param>
        /// <returns>返回结果 string</returns>
        protected  string SetPWDCode(string str, bool decode, int Codelength)  //140918_0 增加加密和解密部分!
        {
            string resultStr = "";
            try
            {
                if (decode)
                {
                    int leftStr = str.Length % Codelength;
                    int arrayNum = str.Length / Codelength;
                    if (str.Length % Codelength != 0)
                    {
                        arrayNum++;
                        str = str.PadLeft(arrayNum * Codelength, '0');
                    }

                    string[] myCode = new string[arrayNum];
                    String[] pduCodeArray = new String[arrayNum]; //此字符串数组，分隔好的字符串

                    for (int i = 0; i < arrayNum; i++)
                    {
                        pduCodeArray[i] = str.Substring(0, Codelength);
                        myCode[i] = ((Convert.ToInt32(pduCodeArray[i], 16) + 3 + i) / 0x0f).ToString();// Convert.ToString(pduCodeArray[i], 16);
                        str = str.Substring(Codelength);
                    }

                    string tempStr = "";
                    for (int i = 0; i < myCode.Length; i++)
                    {
                        tempStr += Convert.ToString((char)(int.Parse(myCode[i])));

                    }
                    resultStr = tempStr;
                }
                else
                {
                    int arrayNum = str.Length;
                    string[] myCode = new string[arrayNum];
                    for (int i = 0; i < arrayNum; i++)
                    {
                        int charCode = char.Parse(str.Substring(0, 1));
                        if (charCode > 126 | charCode < 32)
                        {
                            MessageBox.Show("当前的密码字符串中存在汉字,可能导致解密失败,请重新输入密码!!!");
                            return null;
                        }
                        myCode[i] = Convert.ToString(char.Parse(str.Substring(0, 1)) * 0x0f - 3 - i, 16).PadLeft(Codelength, '0');
                        str = str.Substring(1);
                    }
                    string tempStr = "";
                    for (int i = 0; i < myCode.Length; i++)
                    {
                        tempStr += myCode[i];
                    }
                    resultStr = tempStr;
                }
                return resultStr;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return resultStr;
            }

        }

        public ServerDatabaseIO(string serverName) //读取XML后配置此部分
        {
            string pserverName = serverName;
            string pdbName = "ATSHOME";
            string puser = "ATSUser";
            string ppwd = "ats#inno2014";

            //140917_0 Add
            //string pdbName = "EDVTHOME";  
            //string puser = "EDVTUser";
            //string ppwd = "edvt#inno2014";

            string strConn = "Server=" + pserverName + ";Initial Catalog=" + pdbName + ";" + "user id=" + puser + ";password=" + ppwd + ";";
            conn = new SqlConnection(strConn);
        }

        public ServerDatabaseIO(string serverName, string dbName, string user, string pwd) 
        {
            string user_id = SetPWDCode(user, true, 4);
            string password = SetPWDCode(pwd, true, 4);
            string strConn = "Server=" + serverName + ";Initial Catalog=" + dbName + ";" + "user id=" + user_id + ";password=" + password + ";";
            conn = new SqlConnection(strConn);
        }

        public override bool OpenDatabase(bool state)
        {
            bool result = false;
            try
            {
                if (state)
                {
                    if (conn == null) conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                }
                else
                {
                    if (conn != null && conn.State != ConnectionState.Closed) //140625_0
                    {
                        conn.Close();
                    }
                }

                result = true;
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Initialize connection database failed!Pls check the connection setting...\n" + ex.Message);
                return result;
            }
        }

        public override DataTable GetDataTable(string sqlCmd, string StrTableName)
        {
            SqlCommand cm = new SqlCommand(sqlCmd, conn);
            SqlDataAdapter da = new SqlDataAdapter(cm);
            DataTable mydt = new DataTable(StrTableName);
            try
            {

                if (conn == null) conn.Open();
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                da.Fill(mydt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Get data failed！" + "\n" + ex.Message + "\n" + ex.Source);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return mydt;
        }

        public override DateTime GetCurrTime()
        {
            DateTime servertime = DateTime.Now; //默认为本机时间,若获取服务器时间成功则为服务器时间!
            try
            {
                OpenDatabase(true);
                //执行存储过程 
                System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("getcurrservertime ", conn);
                //说明命令要执行的是存储过程     
                command.CommandType = CommandType.StoredProcedure;
                //向存储过程中传递参数   

                command.Parameters.Add(new System.Data.SqlClient.SqlParameter("@currtime", SqlDbType.DateTime));

                command.Parameters["@currtime"].Direction = ParameterDirection.Output;
                command.UpdatedRowSource = UpdateRowSource.None;
                command.ExecuteNonQuery();
                servertime = DateTime.Parse(command.Parameters["@currtime"].Value.ToString());
                OpenDatabase(false);
                return servertime;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return servertime;
            }

        }
    }

    public class LocalDatabaseIO : DataIO //本地数据库子类 2个公共的方法!
    {
        protected OleDbConnection conn;

        public LocalDatabaseIO(string AccessFilePath)
        {
            string strConn = "";
            if (AccessFilePath.ToUpper().Contains(".accdb".ToUpper()))
            {
                strConn = "Provider = Microsoft.ACE.OLEDB.12.0;" + @"Data Source=" + AccessFilePath;
            }
            else
            {
                strConn = "Provider=Microsoft.Jet.OleDb.4.0;" + @"Data Source=" + AccessFilePath;
            }
            conn = new OleDbConnection(strConn);
        }

        public override bool OpenDatabase(bool state)
        {
            bool result = false;
            try
            {
                if (state)
                {
                    if (conn == null) conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                }
                else
                {
                    if (conn != null && conn.State != ConnectionState.Closed)   //140625_0
                    {
                        conn.Close();
                    }
                }
                result = true;
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Initialize connection database failed!Pls check the connection setting...\n" + ex.Message);
                return result;
            }
        }

        public override DataTable GetDataTable(string sqlCmd, string StrTableName)
        {
            OleDbDataAdapter da = new OleDbDataAdapter(sqlCmd, conn);
            DataTable mydt = new DataTable(StrTableName);
            try
            {

                if (conn == null) conn.Open();
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                da.Fill(mydt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Get data failed！" + "\n" + ex.Message + "\n" + ex.Source);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return mydt;
        }

        public override DateTime GetCurrTime()
        {
            return DateTime.Now; //默认为本机时间,若获取服务器时间成功则为服务器时间!
        }
    }

    public static class TimeSpanUse
    {
        public static string DateDiff(TimeSpan tsBegin, Log pLog, string StrMessage, out TimeSpan tsStartTime)
        {
            TimeSpan tsEnd = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan ts = tsEnd.Subtract(tsBegin).Duration();//时间差的绝对值 ，测试你的代码运行了多长时间。
            string Str = (ts.TotalMilliseconds / 1000).ToString();
            string Str1 = "TimeSpan:" + StrMessage + "Run Time " + Str;
            Log.SaveLogToTxt(Str1);
            tsStartTime = new TimeSpan(DateTime.Now.Ticks);
            return Str;
        }
    }
}

