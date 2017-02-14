using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace NichTest
{
    public class RxOTable : DataTable
    {
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
            this.Columns.Add("LosA_OMA", typeof(double));
            this.Columns.Add("LosD_OMA", typeof(double));
            this.Columns.Add("Sensitivity", typeof(double));
            this.Columns.Add("Sensitivity_OMA", typeof(double));
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
