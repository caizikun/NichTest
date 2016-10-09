using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace NichTest
{
    public class DUTCoeffControlByPN: DataTable
    {
        public struct CoeffInfo
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public int Channel { get; set; }
            public byte Page { get; set; }
            public int StartAddress { get; set; }
            public int Length { get; set; }
            public byte Format { get; set; }
            public double Amplify { get; set; }
        }

        public DUTCoeffControlByPN(DataTable table)
        {
            string[] columns = { "ItemTYPE", "ItemName", "Channel", "Page", "StartAddress", "Length", "Format", "AmplifyCoeff" };

            DataColumn dc = this.Columns.Add("ID", typeof(int));
            dc.AllowDBNull = false;
            dc.Unique = true;
            dc.AutoIncrement = true;

            foreach(string column in columns)
            {
                this.Columns.Add(column, typeof(string));
            }

            for (int row = 0; row < table.Rows.Count; row++)
            {
                DataRow dr = this.NewRow();
                foreach (string column in columns)
                {
                    dr[column] = table.Rows[row][column].ToString().Trim().ToUpper();
                }
                this.Rows.Add(dr);
            }
        }

        public CoeffInfo GetOneInfoFromTable(string itemName, int channel)
        {            
            string filter = "ItemName = " + "'" + itemName + "'";
            DataRow[] foundRows = this.Select(filter);

            for (int row = 0; row < foundRows.Length; row++)
            {
                if (Convert.ToInt32(foundRows[row]["Channel"]) == channel)
                {
                    CoeffInfo coeffInfo = new CoeffInfo();
                    coeffInfo.Type = foundRows[row]["ItemTYPE"].ToString();
                    coeffInfo.Name = itemName;
                    coeffInfo.Channel = channel;
                    coeffInfo.Page = Convert.ToByte(foundRows[row]["Page"]);
                    coeffInfo.StartAddress = Convert.ToInt32(foundRows[row]["StartAddress"]);
                    coeffInfo.Length = Convert.ToInt32(foundRows[row]["Length"]);
                    string buff = foundRows[row]["Format"].ToString();
                    switch (buff)
                    {                        
                        case "IEEE754":
                            coeffInfo.Format = 1;
                            break;
                        case "U16":
                            coeffInfo.Format = 2;
                            break;
                        case "U32":
                            coeffInfo.Format = 3;
                            break;

                    }
                    //1 ieee754;2 UInt16;3 UInt32
                    coeffInfo.Amplify = Convert.ToDouble(foundRows[row]["AmplifyCoeff"]);

                    return coeffInfo;
                }
            }

            throw new IndexOutOfRangeException("No find " + itemName + "information, please check module table config");            
        }
    }
}
