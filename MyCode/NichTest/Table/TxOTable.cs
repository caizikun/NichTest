using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace NichTest
{
    public class TxOTable : DataTable
    {
        private static volatile TxOTable instance = null;
        private static object syncRoot = new Object();

        private TxOTable()
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
            this.Columns.Add("DmiIBias", typeof(double));
            this.Columns.Add("DmiTxPowerErr", typeof(double));
            this.Columns.Add("TxDisablePower", typeof(double));
            this.Columns.Add("IBias", typeof(double));
            this.Columns.Add("IMod", typeof(double));
            this.Columns.Add("AP", typeof(double));
            this.Columns.Add("ER", typeof(double));
            this.Columns.Add("OMA", typeof(double));
            this.Columns.Add("MaskMargin", typeof(double));
            this.Columns.Add("XMASKMARGIN2", typeof(double));
            this.Columns.Add("JitterPP", typeof(double));
            this.Columns.Add("JitterRMS", typeof(double));
            this.Columns.Add("Crossing", typeof(double));
            this.Columns.Add("RiseTime", typeof(double));
            this.Columns.Add("FallTime", typeof(double));
            this.Columns.Add("EyeHeight", typeof(double));
            this.Columns.Add("AMP", typeof(double));
            this.Columns.Add("Wavelength", typeof(double));
            this.Columns.Add("Result", typeof(int));
        }        

        public static TxOTable Get_TxOTable()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new TxOTable();
                    }
                }
            }
            return instance;
        }

        public void ReadXml(string name, string xmlFilePath)
        {
            lock (syncRoot)
            {
                this.TableName = name;
                this.ReadXml(xmlFilePath);
            }
        }

        public void WriteXml(string name, string xmlFilePath)
        {
            lock (syncRoot)
            {
                this.TableName = name;
                this.WriteXml(xmlFilePath);
            }
        }
    }
}
