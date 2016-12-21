using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Searcher;
using Core.Entity;
using Core.DBConnector;
using System.Threading;
using System.Data;
using System.Configuration;
using System.IO;

namespace Core {
    public class SearcherTool {

        /// <summary>
        /// 匹配值阈值
        /// </summary>
        public static double matchOk = 0.8;
        /// <summary>
        /// 连接字符串
        /// </summary>
        public static string ConnectString = string.Empty;
        public static int nowIndex = 0;
        private WosSearcher wosSearcher = new WosSearcher();
        private DBSearcher dbSearcher;
        private Thread searchThread;
        private int threadCount = 10;
        public bool bRunFlg = false;
        public List<WosSearcher> searchers = new List<WosSearcher>();


        public SearcherTool() {
            ReadConfig();
            for (int i = 0; i < threadCount; i++) {
                WosSearcher searcher = new WosSearcher();
                searchers.Add(searcher);
            }
            dbSearcher = new DBSearcher();
            
        }

        private void ReadConfig() {
            ConnectString = System.Configuration.ConfigurationManager.AppSettings["ConnectString"];
            double.TryParse(System.Configuration.ConfigurationManager.AppSettings["matchok"], out matchOk);
            int.TryParse(ConfigurationManager.AppSettings["ThreadCount"], out threadCount);
            int.TryParse(File.ReadAllText("titleIndex.cfg").Trim(), out nowIndex);
            // MySqlCon.CheckTables();
        }

        private void SetTitleIndex(string index) {
            lock (searchers) {
                if (int.Parse(index) >= nowIndex) {
                    nowIndex = int.Parse(index);
                    File.WriteAllText("titleIndex.cfg", index);
                }
            }
        }

        public void start() {
            bRunFlg = true;
            searchThread = new Thread(new ThreadStart(StartThread));
            searchThread.Start();
        }

        public void stop() {
            bRunFlg = false;
        }

        private void StartThread() {
            while (bRunFlg) {
                int start = nowIndex + 1;
                int end = nowIndex + 500;
                SearchDataToOracle(start, end);
                Thread.Sleep(1000 * 10);
            }
        }

        public delegate void DealTitle(string title, string id, WosSearcher searcher);

        /// <summary>
        /// 检索数据并保存到oracle库中
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void SearchDataToOracle(int start, int end) {
            DataTable dt_articles = OracleCon.SelectArticles(start, end);
            if (dt_articles.Rows.Count < 1) {
                SetTitleIndex(end.ToString());
                return;
            }

            try {
                foreach (DataRow row in dt_articles.Rows) {
                    string title = row["CITATION_TITLE"].ToString();
                    string id = row["INTERNAL_ID"].ToString();
                    int.TryParse(id, out nowIndex);
                    if (string.IsNullOrEmpty(title)) {
                        SetTitleIndex(id);
                        OracleCon.SaveMatchDataError(title, id, "title is null");
                        continue;
                    }
                    bool GetSearcher = false;
                    WosSearcher tmpSearcher = null;
                    while (!GetSearcher) {
                        for (int i = 0; i < searchers.Count; i++) {
                            if (!searchers[i].isbusy) {
                                tmpSearcher = searchers[i];
                                GetSearcher = true;
                                tmpSearcher.isbusy = true;
                                break;
                            }
                        }
                    }
                    new DealTitle(M_DealTitle).BeginInvoke(title, id, tmpSearcher, null, null);
                }
            } catch (Exception) {
                return;
            }

        }

        private void M_DealTitle(string title, string id, WosSearcher tmpSearcher) {
            try {
                WosData wosData = tmpSearcher.SearchNoInit(title);
                if (wosData != null) {
                    double matchTmp = 0;
                    string[] rdatas = wosData.getDataArray();
                    double.TryParse(rdatas[rdatas.Length - 1], out matchTmp);
                    if (matchTmp >= matchOk) {
                        try {
                            DBConnector.OracleCon.SaveWosDataLong(wosData);
                        } catch (Exception e) {
                            OracleCon.SaveMatchDataError(title, id, e.Message);
                        }
                        try {
                            DBConnector.OracleCon.SaveMatchData(wosData, title, id);
                        } catch (Exception e) {
                            OracleCon.SaveMatchDataError(title, id, e.Message);
                        }
                    } else {
                        OracleCon.SaveMatchDataError(title, id, "no data match");
                    }
                }
            } catch (Exception e) {
                OracleCon.SaveMatchDataError(title, id, e.Message);
            } finally {
                tmpSearcher.isbusy = false;
            }
            SetTitleIndex(id);
        }


        /// <summary>
        /// 对一个数据库中的数据进行检索，并入库
        /// </summary>
        /// <param name="titles"></param>
        public void SearchDataToDb(string[] titles) {
            try {
                wosSearcher.InitHttp();
                foreach (string title in titles) {
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
                }
            } catch (Exception e) {
                Logs.WriteLog("SearchDataToDb Error:" + e.Message);
            }

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
