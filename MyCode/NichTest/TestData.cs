using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace NichTest
{
    class TestData: DataTable
    {
        public TestData()
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

            this.Columns.Add("TxDarkADC", typeof(int));
            this.Columns.Add("TxPower", typeof(string));
            this.Columns.Add("RxDarkADC", typeof(int));
            this.Columns.Add("RxRes", typeof(string));

            this.Columns.Add("DeltaTxDarkADC", typeof(string));
            this.Columns.Add("DeltaTxPower", typeof(string));
            this.Columns.Add("DeltaRxDarkADC", typeof(string));
            this.Columns.Add("DeltaRxRes", typeof(string));

            this.Columns.Add("Result", typeof(int));            
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
