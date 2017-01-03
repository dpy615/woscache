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

namespace Core
{
    public class SearcherTool
    {

        /// <summary>
        /// 匹配值阈值
        /// </summary>
        public static double matchOk = 0.8;
        /// <summary>
        /// 连接字符串
        /// </summary>
        public static string ConnectString = string.Empty;
        /// <summary>
        /// 当前执行到的ID
        /// </summary>
        public static int nowIndex = 0;
        /// <summary>
        /// 数据库中的最大记录的ID，会一直不停地更新
        /// </summary>
        public static int maxIndex = 0;
        /// <summary>
        /// 计时器间隔，控制访问Oracle获取最大的ID值,单位为分钟
        /// </summary>
        private static int timeInterval = 10;
        /// <summary>
        /// 访问wos的最大线程数
        /// </summary>
        private int threadCount = 10;
        /// <summary>
        /// 每次访问wos的间隔，randomTime*随机值（0-1）
        /// </summary>
        private int randomTime = 10;

        /// <summary>
        /// 所有的起始时间
        /// </summary>
        private List<DateTime> startTimes = new List<DateTime>();
        /// <summary>
        /// 所有的结束时间
        /// </summary>
        private List<DateTime> endTimes = new List<DateTime>();

        private System.Timers.Timer timer;
        private WosSearcher wosSearcher = new WosSearcher();
        private DBSearcher dbSearcher = new DBSearcher();
        private Thread searchThread;

        public bool bRunFlg = false;
        public List<WosSearcher> searchers = new List<WosSearcher>();


        public SearcherTool()
        {

            ReadConfig();
            for (int i = 0; i < threadCount; i++)
            {
                WosSearcher searcher = new WosSearcher();
                searchers.Add(searcher);
            }
            maxIndex = dbSearcher.GetMaxTiTleIndex();

            timer = new System.Timers.Timer();
            timer.Interval = 1000 * 60 * timeInterval;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Enabled = false;
            try
            {
                int tmpIndex = dbSearcher.GetMaxTiTleIndex();
                if (tmpIndex > 0)
                {
                    maxIndex = tmpIndex;
                    Console.WriteLine("GetMaxIndex:" + maxIndex);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLog(string.Format("{0}\r\n{1}", "GetMaxIndexError", ex.Message));
            }
            finally
            {
                timer.Enabled = true;
            }
        }

        private void ReadConfig()
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            ConnectString = System.Configuration.ConfigurationManager.AppSettings["ConnectString"];
            double.TryParse(System.Configuration.ConfigurationManager.AppSettings["MatchOk"], out matchOk);
            int.TryParse(ConfigurationManager.AppSettings["ThreadCount"], out threadCount);
            int.TryParse(ConfigurationManager.AppSettings["TimeInterval"], out timeInterval);
            int.TryParse(ConfigurationManager.AppSettings["RandomTime"], out randomTime);
            int.TryParse(File.ReadAllText(path + "titleIndex.cfg").Trim(), out nowIndex);

            SplitTime(ConfigurationManager.AppSettings["SearchTime1"].ToString().Trim());
            SplitTime(ConfigurationManager.AppSettings["SearchTime2"].ToString().Trim());
            SplitTime(ConfigurationManager.AppSettings["SearchTime3"].ToString().Trim());

            string initmsg = "INIT\r\n";
            initmsg += "ThreadCount:" + threadCount + "\r\n";
            initmsg += "TimeInterval:" + timeInterval + "\r\n";
            initmsg += "TitleIndex:" + nowIndex + "\r\n";
            initmsg += "RandomTime:" + randomTime + "\r\n";
            Logs.WriteLog(initmsg);
            Console.WriteLine(initmsg);
            // MySqlCon.CheckTables();
        }

        private void SplitTime(string times)
        {
            if (string.IsNullOrEmpty(times) || !times.Contains("-"))
            {
                return;
            }
            string start = times.Split('-')[0];
            string end = times.Split('-')[1];
            DateTime startTm;
            DateTime endTm;
            if (DateTime.TryParse(start, out startTm) && DateTime.TryParse(end, out endTm))
            {
                startTimes.Add(startTm);
                endTimes.Add(endTm);
            }
            else
            {
                Logs.WriteLog("时间范围设置不正确：" + times);
            }
        }

        private void SetTitleIndex(string index)
        {
            lock (searchers)
            {
                if (int.Parse(index) >= nowIndex)
                {
                    nowIndex = int.Parse(index);
                    File.WriteAllText("titleIndex.cfg", index);
                }
            }
        }

        public void start()
        {
            bRunFlg = true;
            searchThread = new Thread(new ThreadStart(StartThread));
            searchThread.Start();
        }

        public void stop()
        {
            bRunFlg = false;
        }

        private void StartThread()
        {
            while (bRunFlg)
            {
                if (!CheckTimeIsOK())
                {
                    Thread.Sleep(1000 * 10);
                    continue;
                }

                int start = nowIndex + 1;
                int end = nowIndex + 500;
                if (start >= maxIndex)
                {
                    Thread.Sleep(1000 * 60);
                    continue;
                }
                else if (end > maxIndex + 1)
                {
                    end = maxIndex + 1;
                }
                SearchDataToOracle(start, end);
                Thread.Sleep(1000 * 10);
            }
        }

        /// <summary>
        /// 检查当前时间是否在设置的允许抓取数据的时间范围内
        /// </summary>
        /// <returns></returns>
        private bool CheckTimeIsOK()
        {
            if (startTimes.Count < 1)
            {
                return true;
            }
            DateTime dtNow = DateTime.Now;
            for (int i = 0; i < startTimes.Count; i++) 
            {
                var dtStart = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day,startTimes[i].Hour, startTimes[i].Minute,startTimes[i].Second);
                var dtEnd = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, endTimes[i].Hour, endTimes[i].Minute, endTimes[i].Second);
                if(dtNow >= dtStart && dtNow <= dtEnd)
                {
                    return true;
                }
            }
            return false;
            
        }

        public delegate void DealTitle(string title, string id, WosSearcher searcher);

        /// <summary>
        /// 检索数据并保存到oracle库中
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void SearchDataToOracle(int start, int end)
        {
            DataTable dt_articles = OracleCon.SelectArticles(start, end);
            if (dt_articles.Rows.Count < 1)
            {
                SetTitleIndex(end.ToString());
                return;
            }
            try
            {
                foreach (DataRow row in dt_articles.Rows)
                {
                    string title = row["CITATION_TITLE"].ToString();
                    string id = row["INTERNAL_ID"].ToString();
                    int.TryParse(id, out nowIndex);
                    if (string.IsNullOrEmpty(title))
                    {
                        SetTitleIndex(id);
                        OracleCon.SaveMatchDataError(title, id, "title is null");
                        continue;
                    }

                    if (dbSearcher.SearchOracle(title, id))
                    {
                        SetTitleIndex(id);
                        continue;
                    }

                    bool GetSearcher = false;
                    WosSearcher tmpSearcher = null;
                    while (!GetSearcher)
                    {
                        for (int i = 0; i < searchers.Count; i++)
                        {
                            if (!searchers[i].isbusy)
                            {
                                tmpSearcher = searchers[i];
                                GetSearcher = true;
                                tmpSearcher.isbusy = true;
                                break;
                            }
                        }
                    }
                    new DealTitle(M_DealTitle).BeginInvoke(title, id, tmpSearcher, null, null);
                }
            }
            catch (Exception e)
            {
                Logs.WriteLog(string.Format("{0}\r\n{1}", "SearchDataToOracle:" + nowIndex, e.Message));
                return;
            }

        }

        private void M_DealTitle(string title, string id, WosSearcher tmpSearcher)
        {
            bool b = true;
        start:
            try
            {
                WosData wosData = tmpSearcher.SearchNoInit(title, randomTime);
                if (wosData != null)
                {
                    double matchTmp = 0;
                    string[] rdatas = wosData.getDataArray();
                    double.TryParse(rdatas[rdatas.Length - 1], out matchTmp);
                    if (matchTmp >= matchOk)
                    {
                        try
                        {
                            DBConnector.OracleCon.SaveWosDataLong(wosData);
                        }
                        catch (Exception e)
                        {
                            OracleCon.SaveMatchDataError(title, id, e.Message);
                        }
                        try
                        {
                            DBConnector.OracleCon.SaveMatchData(wosData, title, id);
                        }
                        catch (Exception e)
                        {
                            OracleCon.SaveMatchDataError(title, id, e.Message);
                        }
                    }
                    else
                    {
                        OracleCon.SaveMatchDataError(title, id, "no data match");
                    }
                }
                b = true;
            }
            catch (Exception e)
            {
                if (e.Message == "wos数据下载超时")
                {
                    b = false;
                    Logs.WriteLog(string.Format("{0}\r\n{1}\r\n{2}", title, e.Message, "请检查网络连接是否正常"));
                    goto start;
                }
                b = true;
                OracleCon.SaveMatchDataError(title, id, e.Message);
            }
            finally
            {
                if (b)
                {
                    tmpSearcher.isbusy = false;
                }
            }
            SetTitleIndex(id);
        }


        /// <summary>
        /// 对一个数据库中的数据进行检索，并入库
        /// </summary>
        /// <param name="titles"></param>
        public void SearchDataToDb(string[] titles)
        {
            try
            {
                wosSearcher.InitHttp();
                foreach (string title in titles)
                {
                    WosData wosData = wosSearcher.Search(title);
                    if (wosData != null)
                    {
                        double matchTmp = 0;
                        string[] rdatas = wosData.getDataArray();
                        double.TryParse(rdatas[rdatas.Length - 1], out matchTmp);
                        if (matchTmp >= matchOk)
                        {
                            try
                            {
                                DBConnector.MySqlCon.SaveWosData(wosData);
                            }
                            catch (Exception)
                            {
                                DBConnector.MySqlCon.SaveMatchData(wosData, title);
                            }
                        }
                        else
                        {
                            wosData = null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logs.WriteLog("SearchDataToDb Error:" + e.Message);
            }

        }

        /// <summary>
        /// 只检索数据库
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public WosData SearchDB(string title)
        {
            WosData wosData = dbSearcher.Search(title);
            return wosData;
        }

        /// <summary>
        /// 只检索Wos
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public WosData SearchWos(string title)
        {
            WosData wosData = wosSearcher.Search(title);
            if (wosData != null)
            {
                double matchTmp = 0;
                string[] rdatas = wosData.getDataArray();
                double.TryParse(rdatas[rdatas.Length - 1], out matchTmp);
                if (matchTmp >= matchOk)
                {
                    try
                    {
                        DBConnector.MySqlCon.SaveWosData(wosData);
                    }
                    catch (Exception)
                    {
                        DBConnector.MySqlCon.SaveMatchData(wosData, title);
                    }
                }
                else
                {
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
        public WosData SearchWos(string title, string year)
        {
            WosData wosData = wosSearcher.Search(title, year);
            if (wosData != null)
            {
                double matchTmp = 0;
                string[] rdatas = wosData.getDataArray();
                double.TryParse(rdatas[rdatas.Length - 1], out matchTmp);
                if (matchTmp >= matchOk)
                {
                    try
                    {
                        DBConnector.MySqlCon.SaveWosData(wosData);
                    }
                    catch (Exception)
                    {
                        DBConnector.MySqlCon.SaveMatchData(wosData, title);
                    }
                }
                else
                {
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
        public WosData Search(string title)
        {
            WosData wosData = dbSearcher.Search(title);
            if (wosData != null)
            {
                return wosData;
            }
            wosData = wosSearcher.Search(title);
            if (wosData != null)
            {
                double matchTmp = 0;
                string[] rdatas = wosData.getDataArray();
                double.TryParse(rdatas[rdatas.Length - 1], out matchTmp);
                if (matchTmp >= matchOk)
                {
                    try
                    {
                        DBConnector.MySqlCon.SaveWosData(wosData);
                    }
                    catch (Exception)
                    {
                        DBConnector.MySqlCon.SaveMatchData(wosData, title);
                    }
                }
                else
                {
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
        public WosData Search(string title, string year)
        {
            WosData wosData = dbSearcher.Search(title);
            if (wosData != null)
            {
                return wosData;
            }
            wosData = wosSearcher.Search(title, year);
            if (wosData != null)
            {
                double matchTmp = 0;
                string[] rdatas = wosData.getDataArray();
                double.TryParse(rdatas[rdatas.Length - 1], out matchTmp);
                if (matchTmp >= matchOk)
                {
                    try
                    {
                        DBConnector.MySqlCon.SaveWosData(wosData);
                    }
                    catch (Exception)
                    {
                        DBConnector.MySqlCon.SaveMatchData(wosData, title);
                    }
                }
                else
                {
                    wosData = null;
                }
            }
            return wosData;
        }



    }
}
