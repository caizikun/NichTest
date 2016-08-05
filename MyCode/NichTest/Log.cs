using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NichTest
{
    public class Log
    {
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
            }
            catch
            {
                return;
            }         
        }
    }
}
