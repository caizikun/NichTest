using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace NichTest
{
    public class TxOTable : DataTable
    {
        private static object syncRoot = new Object();

        public TxOTable()
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
            this.Columns.Add("AP_dBm", typeof(double));
            this.Columns.Add("ER_dB", typeof(double));
            this.Columns.Add("OEOMA", typeof(double));
            this.Columns.Add("TxOMA_dBm", typeof(double));
            this.Columns.Add("MaskMargin", typeof(double));
            this.Columns.Add("XMaskMargin2", typeof(double));
            this.Columns.Add("JitterPP_ps", typeof(double));
            this.Columns.Add("JitterRMS_ps", typeof(double));
            this.Columns.Add("Crossing", typeof(double));
            this.Columns.Add("RiseTime_ps", typeof(double));
            this.Columns.Add("FallTime_ps", typeof(double));
            this.Columns.Add("EyeHeight_mW", typeof(double));
            this.Columns.Add("AMP", typeof(double));
            this.Columns.Add("Wavelength", typeof(double));
            this.Columns.Add("Status", typeof(int));
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

        public void SaveTableToExcel(string fileName)
        {
            lock (syncRoot)
            {
                DirectoryInfo directoryInfo = Directory.GetParent(fileName);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate);
                StreamWriter writer = new StreamWriter(fileStream, Encoding.Default);

                StringBuilder title = new StringBuilder();
                for (int i = 0; i < this.Columns.Count; i++)//栏位：自动跳到下一单元格
                {
                    title.Append(this.Columns[i].ColumnName);
                    title.Append("\t");
                }
                writer.WriteLine(title);

                StringBuilder content = new StringBuilder();
                foreach (DataRow row in this.Rows)//内容：自动跳到下一单元格
                {
                    for (int i = 0; i < this.Columns.Count; i++)
                    {
                        content.Append(row[i]);
                        content.Append("\t");
                    }
                    content.Append("\n");
                }
                writer.Write(content);

                writer.Close();
                writer.Dispose();
                fileStream.Close();
                fileStream.Dispose();
            }
        }
    }
}
