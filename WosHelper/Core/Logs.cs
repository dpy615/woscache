using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Core {
    class Logs {
        public static object _logLocker = new object();
        public static void WriteLog(string str) {
            string FileName = DateTime.Now.ToString("yyyy-MM-dd");
            if (!Directory.Exists("logs")) {
                Directory.CreateDirectory("logs");
            }
            FileName = "logs\\" + FileName + ".txt";
            lock (_logLocker) {
                using (StreamWriter sw = new StreamWriter(FileName, true)) {
                    sw.WriteLine(string.Format("{0}\t{1}", DateTime.Now.ToString("HH:mm:ss"), str));
                    sw.Flush();
                }
            }
        }
    }
}
