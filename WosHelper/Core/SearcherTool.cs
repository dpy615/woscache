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


        /// <summary>
        /// 只检索数据库
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public WosData SearchDB(string title) {
            WosData wosData = dbSearcher.Search(title);
            return wosData;
        }

        /// <summary>
        /// 只检索Wos
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public WosData SearchWos(string title) {
            WosData wosData = wosSearcher.Search(title);
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

        /// <summary>
        /// 只检索Wos(标题和出版年)
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public WosData SearchWos(string title, string year) {
            WosData wosData = wosSearcher.Search(title, year);
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


        /// <summary>
        /// 先检索数据库，再检索wos
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 通过标题和出半年检索
        /// </summary>
        /// <param name="title"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public WosData Search(string title, string year) {
            WosData wosData = dbSearcher.Search(title);
            if (wosData != null) {
                return wosData;
            }
            wosData = wosSearcher.Search(title, year);
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

    }
}
