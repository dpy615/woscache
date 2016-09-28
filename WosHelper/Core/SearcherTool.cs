using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Searcher;
using Core.Entity;

namespace Core {
    public class SearcherTool {

        public static double matchOk = 0.8;
        public static string ConnectString = string.Empty;
        private WosSearcher wosSearcher;
        private DBSearcher dbSearcher;

        public SearcherTool() {
            wosSearcher = new WosSearcher();
            dbSearcher = new DBSearcher();
            ReadConfig();
        }

        private void ReadConfig() {
            ConnectString = System.Configuration.ConfigurationManager.AppSettings["ConnectString"];
            double.TryParse(System.Configuration.ConfigurationManager.AppSettings["matchok"], out matchOk);
        }

        public WosData Search(string title) {
            WosData wosData = dbSearcher.Search(title);
            if (wosData != null) {
                return wosData;
            }
            wosData = wosSearcher.Search(title);
            if (wosData != null) {
                double matchTmp = 0;
                string[] rdatas = wosData.getDataArray();
                double.TryParse(rdatas[rdatas.Length - 1], out matchTmp);
                if (matchTmp >= matchOk) {
                    try {
                        DBConnector.MySqlCon.SaveWosData(wosData);
                    } catch (Exception) {
                        DBConnector.MySqlCon.SaveMatchData(wosData, title);
                    }
                } else {
                    wosData = null;
                }
            }
            return wosData;
        }

        public WosData Search(string title, string year) {
            return Search(title);
        }
    }
}
