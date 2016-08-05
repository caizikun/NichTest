using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace NichTest
{
    #region  Local Database

    public class LocalDatabase : DataIO
    {
        public OleDbConnection conn;
        private string strConnection = "";
        private DataTable DT = new DataTable();

        public LocalDatabase(string Path)
        {
            strConnection = "Provider = Microsoft.ACE.OLEDB.12.0;" + @"Data Source=" + Path;
            conn = new OleDbConnection(strConnection);
        }
        public override bool OpenDatabase(bool Swith)
        {
            if (Swith)
            {

                if (conn.State.ToString().ToUpper() != "OPEN")
                {
                    conn.Open();
                    Thread.Sleep(200);
                }
            }
            else
            {
                if (conn != null && conn.State.ToString().ToUpper() == "OPEN")
                {
                    conn.Close();
                    Thread.Sleep(200);
                }
            }
            return true;
        }
        // override public bool WriterLog(int ConditionID, int SNID, string StrInf,  string StartTime, string EndTime, out  int strid)
        public override bool WriterLog(int ConditionID, int SNID, string StrInf, string StartTime, string EndTime, float Temp, float Voltage, byte Channel, bool Resultlflag, out int logid)
        {



            string strSelectconditions = "Select *from TopoLogRecord order by ID";

            string StrTableName = "TopoLogRecord";
            OleDbDataAdapter da = new OleDbDataAdapter(strSelectconditions, conn);

            OleDbCommandBuilder cb = new OleDbCommandBuilder(da);
            da.UpdateCommand = cb.GetUpdateCommand();
            DataSet ds = new DataSet(StrTableName);

            da.Fill(ds, StrTableName);

            int i = ds.Tables[StrTableName].Rows.Count;
            //var SS= ds.Tables[]
            DataRow drx = ds.Tables[StrTableName].NewRow();

            drx["PID"] = ConditionID;

            drx["RunRecordID"] = SNID;
            drx["StartTime"] = StartTime;
            drx["EndTime"] = EndTime;

            drx["TestLog"] = StrInf;
            drx["Channel"] = Channel;
            ds.Tables[StrTableName].Rows.Add(drx);
            da.Update(ds, StrTableName);
            string StrgetId = "Select * from TopoLogRecord  order by id desc";
            da = new OleDbDataAdapter(StrgetId, conn);

            cb = new OleDbCommandBuilder(da);
            da.UpdateCommand = cb.GetUpdateCommand();

            ds.Clear();

            //ds.EndInit

            //ds = new DataSet(StrTableName);
            // ds.Tables[]

            da.Fill(ds, StrTableName);
            logid = Convert.ToInt32(ds.Tables[StrTableName].Rows[0]["ID"]);

            return true;
        }
        override public bool WriteResult(int StrInterfaceLogid, DataTable ResultDataTabel)
        {
            string strSelectconditions = "Select *from TopoTestData order by ID";

            string StrTableName = "TopoTestData";
            OleDbDataAdapter da = new OleDbDataAdapter(strSelectconditions, conn);

            OleDbCommandBuilder cb = new OleDbCommandBuilder(da);
            da.UpdateCommand = cb.GetUpdateCommand();
            DataSet ds = new DataSet(StrTableName);

            da.Fill(ds, StrTableName);


            int i = ds.Tables[StrTableName].Rows.Count;

            int J = ResultDataTabel.Rows.Count;
            foreach (DataRow dr in ResultDataTabel.Rows)
            {
                if (dr["DataRecord"].ToString().Trim() == "1")
                {

                    DataRow pdr = ds.Tables[StrTableName].NewRow();
                    pdr["PID"] = StrInterfaceLogid;
                    pdr["ItemName"] = dr["ItemName"].ToString();
                    pdr["ItemValue"] = dr["Value"].ToString();
                    pdr["Result"] = dr["Result"];

                    ds.Tables[StrTableName].Rows.Add(pdr);
                    da.Update(ds, StrTableName);
                }
            }

            return true;
        }
        override public DataTable GetDataTable(string StrSelectconditions, string StrTabelName)
        {
            DataTable dt = new DataTable();

            if (OpenDatabase(true))
            {
                OleDbDataAdapter da = new OleDbDataAdapter(StrSelectconditions, conn);

                OleDbCommandBuilder cb = new OleDbCommandBuilder(da);

                //  da.UpdateCommand = cb.GetUpdateCommand();

                DataSet ds = new DataSet(StrTabelName);

                da.Fill(ds, StrTabelName);

                dt = ds.Tables[StrTabelName];
            }
            OpenDatabase(false);
            conn.Close();
            return dt;
        }// 获得当前的Datatable
        override public bool WriterSN(int TestPlanID, string StrSN,string StrFwRev,string Strip ,string STRlightSource,string StrRemark,out int Id)
        {
            string StrTableName = "TopoRunRecordTable";
            string strSelectconditions = "Select * from " + StrTableName + " order by ID";
            Id = -1;
            if (OpenDatabase(true))
            {
                OleDbDataAdapter da = new OleDbDataAdapter(strSelectconditions, conn);

                OleDbCommandBuilder cb = new OleDbCommandBuilder(da);

                //  da.UpdateCommand = cb.GetUpdateCommand();

                DataSet ds = new DataSet(StrTableName);

                da.Fill(ds, StrTableName);

                DT = ds.Tables[StrTableName];

                DataRow pdr = DT.NewRow();
                pdr["PID"] = TestPlanID;
                pdr["SN"] = StrSN;
                pdr["FWRev"] = StrFwRev;
                pdr["ip"] = Strip;
               // pdr["StartTime"] = GetCurrTime().ToString("yyyy/MM/dd HH:mm:ss");
                pdr["StartTime"] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                ds.Tables[StrTableName].Rows.Add(pdr);
                da.Update(ds, StrTableName);
                //----------------------------------

                strSelectconditions = "Select id from " + StrTableName + " order by ID  desc";
                da = new OleDbDataAdapter(strSelectconditions, conn);
                cb = new OleDbCommandBuilder(da);

                da.UpdateCommand = cb.GetUpdateCommand();
                ds = new DataSet(StrTableName);

                da.Fill(ds, StrTableName);

                conn.Close();

                Id = Convert.ToInt32(ds.Tables[StrTableName].Rows[0]["ID"]);
                return true;
            }
            else
            {
                return false;
            }
            //string StrTableName = "TopoRunRecordTable";
            //string strSelectconditions = "Select * from "+StrTableName+ " order by ID";
            //OleDbDataAdapter da = new OleDbDataAdapter(strSelectconditions, conn);
            //OleDbCommandBuilder cb = new OleDbCommandBuilder(da);

            //da.UpdateCommand = cb.GetUpdateCommand();
            //ds = new DataSet(StrTableName);

            //da.Fill(ds, StrTableName);

            //int i = ds.Tables[StrTableName].Rows.Count;

            //DT = ds.Tables[StrTableName];

        }
        public override bool UpdateDataTable(string SQLCmd, DataTable NewChangeDT)
        {

            bool result = false;

            if (conn == null) conn.Open();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            OleDbTransaction tr;
            tr = conn.BeginTransaction(IsolationLevel.RepeatableRead);

            try
            {
                OleDbCommand cm = new OleDbCommand(SQLCmd, conn);
                OleDbDataAdapter da = new OleDbDataAdapter(cm);
                OleDbCommandBuilder cb = new OleDbCommandBuilder(da);
                DataTable mydt = new DataTable();
                da.SelectCommand.Transaction = tr;


                da.Update(NewChangeDT);
                tr.Commit();

                result = true;

                return result;
            }
            catch (Exception ex)
            {
                try
                {
                    MessageBox.Show(ex.ToString());
                    tr.Rollback();
                    if (conn.State.ToString().ToUpper() == "OPEN") conn.Close();
                    return result;
                }
                catch (System.Exception TransactionEx)
                {
                    //Handle Exception
                    MessageBox.Show(TransactionEx.ToString());
                    if (conn.State.ToString().ToUpper() == "OPEN") conn.Close();
                    return result;
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

        }


    }
    #endregion 
}
