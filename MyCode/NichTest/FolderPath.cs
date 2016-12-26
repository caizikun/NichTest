using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace NichTest
{
    public struct FolderPath
    {
        public static string OpticalEyeDiagram;
        public static string ElecEyeDiagram;
        public static string PlariltyEyeDiagram;

        public static void SetValue(string[] folderPath)
        {
            foreach (string path in folderPath)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }                
            }
            OpticalEyeDiagram = folderPath[0] + "\\";
            ElecEyeDiagram = folderPath[1] + "\\";
            PlariltyEyeDiagram = folderPath[2] + "\\";
        }
    }

    public struct FilePath
    {
        public static string ConfigXml;
        public static string TestDataXml;
        public static string LogFile;

        public static void SaveTableToExcel(DataTable table, string fileName)
        {
            DirectoryInfo directoryInfo = Directory.GetParent(fileName);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(fileStream, Encoding.Default);

            StringBuilder title = new StringBuilder();
            for (int i = 0; i < table.Columns.Count; i++)//栏位：自动跳到下一单元格
            {
                title.Append(table.Columns[i].ColumnName);
                title.Append("\t");
            }
            writer.WriteLine(title);

            StringBuilder content = new StringBuilder();
            foreach (DataRow row in table.Rows)//内容：自动跳到下一单元格
            {
                for (int i = 0; i < table.Columns.Count; i++)
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
