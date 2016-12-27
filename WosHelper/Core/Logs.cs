using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Core {
    public class Logs {
        public static object _logLocker = new object();
        public static string path = System.AppDomain.CurrentDomain.BaseDirectory+"logs";
        public static void WriteLog(string str) {
            string FileName = DateTime.Now.ToString("yyyy-MM-dd");
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            FileName = path+"\\" + FileName + ".txt";
            lock (_logLocker) {
                using (StreamWriter sw = new StreamWriter(FileName, true)) {
                    sw.WriteLine(string.Format("{0}\r\n{1}\r\n", DateTime.Now.ToString("HH:mm:ss"), str));
                    sw.Flush();
                }
            }
        }
    }
}
