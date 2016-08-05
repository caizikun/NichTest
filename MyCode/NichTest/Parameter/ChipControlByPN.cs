using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace NichTest
{
    public class ChipControlByPN : DataTable
    {
        public ChipControlByPN(DataTable table)
        {
            string[] columns = {"ItemName", "ModuleLine", "ChipLine", "DriveType", "RegisterAddress", "Length", "Endianness", "StartBit", "EndBit"};

            DataColumn dc = this.Columns.Add("ID", typeof(int));
            dc.AllowDBNull = false;
            dc.Unique = true;
            dc.AutoIncrement = true;

            
            foreach (string column in columns)
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
    }
}
