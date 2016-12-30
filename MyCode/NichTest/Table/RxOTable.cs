using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace NichTest
{
    public class RxOTable : DataTable
    {
        private static volatile RxOTable instance = null;
        private static object syncRoot = new Object();

        public RxOTable()
        {
            DataColumn dc = this.Columns.Add("ID", typeof(int));
            dc.AllowDBNull = false;
            dc.Unique = true;
            dc.AutoIncrement = true;

            this.Columns.Add("Family", typeof(string));
            this.Columns.Add("PartNumber", typeof(string));
            this.Columns.Add("SerialNumber", typeof(string));
            this.Columns.Add("Channel", typeof(int));
            this.Columns.Add("Current", typeof(string));
            this.Columns.Add("Temp", typeof(double));
            this.Columns.Add("Station", typeof(string));
            this.Columns.Add("Time", typeof(string));
            this.Columns.Add("DmiVccErr", typeof(double));
            this.Columns.Add("DmiTempErr", typeof(double));
            this.Columns.Add("DmiRxPWRErr", typeof(double));
            this.Columns.Add("DmiRxNOptical", typeof(double));
            this.Columns.Add("LosA", typeof(double));
            this.Columns.Add("LosD", typeof(double));
            this.Columns.Add("LosH", typeof(double));
            this.Columns.Add("LOSA_OMA", typeof(double));
            this.Columns.Add("LOSD_OMA", typeof(double));
            this.Columns.Add("Sensitivity", typeof(double));
            this.Columns.Add("Sensitivity_OMA", typeof(double));
            this.Columns.Add("Result", typeof(int));
        }

        public static RxOTable Get_RxOTable()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new RxOTable();
                    }
                }
            }
            return instance;
        }

        public void ReadXml(string name, string xmlFilePath)
        {
            this.TableName = name;
            this.ReadXml(xmlFilePath);
        }

        public void WriteXml(string name, string xmlFilePath)
        {
            this.TableName = name;
            this.WriteXml(xmlFilePath);
        }
    }
}
