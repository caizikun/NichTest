using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NichTest
{
    public delegate void Report();

    public class Log
    {
        public static event Report ReportRecord;

        private Log() { }

        public static void SaveLogToTxt(object content)
        {
            try
            {
                string fileName = FilePath.LogFile;
                DirectoryInfo directoryInfo = Directory.GetParent(fileName);

                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                FileStream fileStream = null;
                StreamWriter writer = null;
                FileInfo fileInfo = new FileInfo(fileName);
                if (!fileInfo.Exists)
                {
                    fileStream = fileInfo.Create();
                }
                else
                {
                    fileStream = fileInfo.Open(FileMode.Append, FileAccess.Write);
                }
                writer = new StreamWriter(fileStream);

                string text = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + content.ToString();
                writer.WriteLine(text);

                writer.Close();
                writer.Dispose();
                fileStream.Close();
                fileStream.Dispose();
                ReportRecord();
            }
            catch
            {
                return;
            }         
        }

        public static string ReadLogFromTxt()
        {
            try
            {
                string fileName = FilePath.LogFile;
                DirectoryInfo directoryInfo = Directory.GetParent(fileName);

                if (!directoryInfo.Exists)
                {
                    return null;
                }

                FileStream fileStream = null;
                StreamReader reader = null;
                FileInfo fileInfo = new FileInfo(fileName);
                if (!fileInfo.Exists)
                {
                    return null;
                }
                else
                {
                    fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read);
                }
                reader = new StreamReader(fileStream);
                string content = reader.ReadToEnd();

                reader.Close();
                reader.Dispose();
                fileStream.Close();
                fileStream.Dispose();

                return content;
            }
            catch
            {
                return null;
            }
        }
    }
}
