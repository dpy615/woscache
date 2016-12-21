using Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Searcher {

    public class WosSearcher {
        string jessionID = "";
        string sID = "";
        HttpHelper http = null;
        HttpResult result;
        string cookie;

        int searchCount = -1;
        public bool isbusy = false;

        public string[] Search(string title, bool isCn) {
            http = new HttpHelper();
            this.InitHttp();
            return DownLoad(http, ref result, cookie, title, isCn);
        }

        public string[] Search(string title, string year, bool isCn) {
            http = new HttpHelper();
            this.InitHttp();
            return DownLoadWithYear(http, ref result, cookie, title, isCn, year);
        }

        public WosData Search(string title) {
            WosData wosData = new WosData();
            string[] datas = Search(title, false);
            if (datas == null) {
                return null;
            }
            wosData.setDataArray(datas);
            return wosData;
        }

        public WosData Search(string title, string year) {
            WosData wosData = new WosData();
            string[] datas = Search(title, year, false);
            if (datas == null) {
                return null;
            }
            wosData.setDataArray(datas);
            return wosData;
        }
        /// <summary>
        /// 不进行初始化读取数据，该方法用于同一个会话进行多个操作，在调用该方法之前必须调用initHttp
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public WosData SearchNoInit(string title) {
            if (searchCount == -1) {
                this.InitHttp();
            }
            searchCount++;
            if (searchCount > 30) {
                this.InitHttp();
                searchCount = 0;
            }
            WosData wosData = new WosData();
            try {
                string[] datas = DownLoad(http, ref result, cookie, title, false);
                if (datas == null) {
                    return null;
                }
                wosData.setDataArray(datas);
            } catch (Exception e) {
                Logs.WriteLog(e.ToString());
                throw e;
            }
            
            return wosData;
        }


        public void InitHttp() {
            if (http == null) {
                http = new HttpHelper();
            }
            HttpItem item = new HttpItem() {
                URL = "http://apps.webofknowledge.com/",//URL     必需项    
                Method = "get",//URL     可选项 默认为Get   
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                Cookie = "",//字符串Cookie     可选项   
                Referer = "",//来源URL     可选项   
                Postdata = "",//Post数据     可选项GET时不需要写   
                Timeout = 100000,//连接超时时间     可选项默认为100000    
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000   
                //UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                ContentType = "text/html",//返回类型    可选项有默认值   
                Allowautoredirect = false,//是否根据301跳转     可选项   
                //CerPath = "d:\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数   
                //Connectionlimit = 1024,//最大连接数     可选项 默认为1024    
                ProxyIp = "",//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数 
                //ProxyPwd = "123456",//代理服务器密码     可选项    
                //ProxyUserName = "administrator",//代理服务器账户名     可选项   
                ResultType = ResultType.String
            };
            result = http.GetHtml(item);
            string html = result.Html;
            cookie = result.Cookie;

            MoveTmp(http, ref result, "");
            cookie = "SID=\"" + sID + "\"; CUSTOMER=\"CAS National Sciences Library of Chinese Academy of Sciences\"; E_GROUP_NAME=\"CAS Library of Beijing Headquarter\"; JSESSIONID=" + jessionID;
        }

        public static string reg_title = @"doc=\d{0,3}""><value lang_id="""">.*?</value>";
        public static string reg_rurl = "<input type=\"hidden\" id=\"rurl\" name=\"rurl\" value=\".*?\" />";
        public static string reg_qid = @"<input type=""hidden"" name=""qid"" value=""\d*""/>";

        /// <summary>
        /// 获取wos网站中导出的文件
        /// </summary>
        /// <param name="http"></param>
        /// <param name="result"></param>
        /// <param name="cookie"></param>
        /// <param name="title"></param>
        /// <param name="isCn"></param>
        /// <returns></returns>
        private string[] DownLoad(HttpHelper http, ref HttpResult result, string cookie, string title, bool isCn) {
            string[] sReturn = null;
            HttpItem item = new HttpItem() {
                URL = "http://apps.webofknowledge.com/WOS_GeneralSearch.do",//URL     必需项    
                Method = "POST",//URL     可选项 默认为Get   
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                Cookie = cookie,//字符串Cookie     可选项   
                Referer = "",//来源URL     可选项   

                Postdata = "fieldCount=1&action=search&product=WOS&search_mode=GeneralSearch&SID=" + sID + "&max_field_count=50&max_field_notice=%E6%B3%A8%E6%84%8F%3A+%E6%97%A0%E6%B3%95%E6%B7%BB%E5%8A%A0%E5%8F%A6%E4%B8%80%E5%AD%97%E6%AE%B5%E3%80%82&input_invalid_notice=%E6%A3%80%E7%B4%A2%E9%94%99%E8%AF%AF%3A+%E8%AF%B7%E8%BE%93%E5%85%A5%E6%A3%80%E7%B4%A2%E8%AF%8D%E3%80%82&exp_notice=%E6%A3%80%E7%B4%A2%E9%94%99%E8%AF%AF%3A+%E4%B8%93%E5%88%A9%E6%A3%80%E7%B4%A2%E8%AF%8D%E5%8F%AF%E5%9C%A8%E5%A4%9A%E4%B8%AA%E5%AE%B6%E6%97%8F%E4%B8%AD%E6%89%BE%E5%88%B0+%28&input_invalid_notice_limits=+%3Cbr%2F%3E%E6%B3%A8%3A+%E6%BB%9A%E5%8A%A8%E6%A1%86%E4%B8%AD%E6%98%BE%E7%A4%BA%E7%9A%84%E5%AD%97%E6%AE%B5%E5%BF%85%E9%A1%BB%E8%87%B3%E5%B0%91%E4%B8%8E%E4%B8%80%E4%B8%AA%E5%85%B6%E4%BB%96%E6%A3%80%E7%B4%A2%E5%AD%97%E6%AE%B5%E7%9B%B8%E7%BB%84%E9%85%8D%E3%80%82&sa_params=WOS%7C%7C" + sID + "%7Chttp%3A%2F%2Fapps.webofknowledge.com%7C%27&formUpdated=true&value%28input1%29=" + title + "&value%28select1%29=TI&x=912&y=298&value%28hidInput1%29=&limitStatus=collapsed&ss_lemmatization=On&ss_spellchecking=Suggest&SinceLastVisit_UTC=&SinceLastVisit_DATE=&period=Range+Selection&range=ALL&startYear=1900&endYear=2016&editions=SCI&editions=SSCI&editions=ISTP&editions=CCR&editions=IC&update_back2search_link_param=yes&ssStatus=display%3Anone&ss_showsuggestions=ON&ss_numDefaultGeneralSearchFields=1&ss_query_language=&rs_sort_by=PY.D%3BLD.D%3BSO.A%3BVL.D%3BPG.A%3BAU.A",
                Timeout = 100000,//连接超时时间     可选项默认为100000    
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000   
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                Allowautoredirect = false,//是否根据301跳转     可选项   
                ResultType = ResultType.String,
                Host = "apps.webofknowledge.com",
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8"
            };

            item.Header.Add("Origin", "http://apps.webofknowledge.com");

            result = http.GetHtml(item);
            MoveTmp(http, ref result, cookie);

            string url = item.URL;
            var titles = Regex.Matches(result.Html.Replace("\r", "").Replace("\n", ""), reg_title);
            if (titles.Count < 1) {
                return null;
            }

            var tmpUrl = Regex.Match(result.Html, reg_rurl).ToString();
            string rurl = tmpUrl.Substring(tmpUrl.IndexOf("http"), tmpUrl.LastIndexOf("\"") - tmpUrl.IndexOf("http"));
            var tqid = Regex.Match(result.Html, reg_qid).ToString();
            tqid = tqid.Substring(tqid.IndexOf("value"));
            tqid = tqid.Substring(tqid.IndexOf("\"") + 1);
            tqid = tqid.Substring(0, tqid.IndexOf("\""));


            double matchValue = 0;
            string matchTitle = "";
            string matchIndex = "";
            for (int i = 0; i < titles.Count; i++) {
                string str = titles[i].ToString();
                str = str.Substring(str.IndexOf("doc"));
                string index = str.Substring(str.IndexOf("=") + 1);
                index = index.Substring(0, index.IndexOf("\""));
                string tit = str.Substring(str.IndexOf("<value lang_")).Replace("<value lang_id=\"\">", "").Replace("<span class=\"hitHilite\">", "").Replace(@"</span>", "").Replace(@"</value>", "");

                double tmpvalue = Utils.MatchValue(title, tit, isCn);
                if (tmpvalue > matchValue) {
                    matchValue = tmpvalue;
                    matchTitle = tit;
                    matchIndex = index;
                }
            }


            item = new HttpItem() {
                URL = "http://apps.webofknowledge.com/OutboundService.do?action=go&&",//URL     必需项    
                Method = "POST",//URL     可选项 默认为Get   
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                Cookie = cookie,//字符串Cookie     可选项   
                Referer = url,//来源URL     可选项   
                Postdata = "selectedIds=&displayCitedRefs=true&displayTimesCited=true&displayUsageInfo=true&viewType=summary&product=WOS&rurl=" + rurl + "&mark_id=WOS&colName=WOS&search_mode=GeneralSearch&locale=zh_CN&view_name=WOS-summary&sortBy=PY.D%3BLD.D%3BSO.A%3BVL.D%3BPG.A%3BAU.A&mode=OpenOutputService&qid=" + tqid + "&SID=" + sID + "&format=saveToFile&filters=PMID+USAGEIND+AUTHORSIDENTIFIERS+ACCESSION_NUM+FUNDING+SUBJECT_CATEGORY+JCR_CATEGORY+LANG+IDS+PAGEC+SABBR+CITREFC+ISSN+PUBINFO+KEYWORDS+CITTIMES+ADDRS+CONFERENCE_SPONSORS+DOCTYPE+CITREF+ABSTRACT+CONFERENCE_INFO+SOURCE+TITLE+AUTHORS++&mark_to=" + matchIndex + "&mark_from=" + matchIndex + "&queryNatural=%3Cb%3E%E4%B8%BB%E9%A2%98%3A%3C%2Fb%3E+%28" + title + "%29&count_new_items_marked=0&use_two_ets=false&IncitesEntitled=yes&value%28record_select_type%29=range&markFrom=" + matchIndex + "&markTo=" + matchIndex + "&fields_selection=PMID+USAGEIND+AUTHORSIDENTIFIERS+ACCESSION_NUM+FUNDING+SUBJECT_CATEGORY+JCR_CATEGORY+LANG+IDS+PAGEC+SABBR+CITREFC+ISSN+PUBINFO+KEYWORDS+CITTIMES+ADDRS+CONFERENCE_SPONSORS+DOCTYPE+CITREF+ABSTRACT+CONFERENCE_INFO+SOURCE+TITLE+AUTHORS++&save_options=tabWinUTF8",
                Timeout = 100000,//连接超时时间     可选项默认为100000    
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000   
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                //UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                Allowautoredirect = false,//是否根据301跳转     可选项   
                //CerPath = "d:\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数   
                //Connectionlimit = 1024,//最大连接数     可选项 默认为1024    
                ProxyIp = "",//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数 
                //ProxyPwd = "123456",//代理服务器密码     可选项    
                //ProxyUserName = "administrator",//代理服务器账户名     可选项   
                ResultType = ResultType.String,
                Host = "apps.webofknowledge.com",
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8"
            };

            item.Header.Add("Origin", "http://apps.webofknowledge.com");

            result = http.GetHtml(item);
            MoveTmp(http, ref result, cookie);

            string res = result.Html;
            string txt = res.Replace("\n", "").Split('\r')[1];
            string[] resArr = res.Replace("\n", "").Split('\r')[1].Split('\t');
            sReturn = new string[resArr.Length + 3];
            Array.Copy(resArr, sReturn, resArr.Length);
            sReturn[resArr.Length] = txt;
            sReturn[resArr.Length + 1] = matchTitle;
            sReturn[resArr.Length + 2] = matchValue.ToString();
            return sReturn;
        }



        /// <summary>
        /// 获取wos网站中导出的文件 通过标题和年份检索
        /// </summary>
        /// <param name="http"></param>
        /// <param name="result"></param>
        /// <param name="cookie"></param>
        /// <param name="title"></param>
        /// <param name="isCn"></param>
        /// <returns></returns>
        private string[] DownLoadWithYear(HttpHelper http, ref HttpResult result, string cookie, string title, bool isCn, string year) {
            string[] sReturn = null;
            HttpItem item = new HttpItem() {
                URL = "http://apps.webofknowledge.com/WOS_GeneralSearch.do",//URL     必需项    
                Method = "POST",//URL     可选项 默认为Get   
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                Cookie = cookie,//字符串Cookie     可选项   
                Referer = "",//来源URL     可选项   

                Postdata = "fieldCount=2&action=search&product=WOS&search_mode=GeneralSearch&SID=" + sID + "&max_field_count=50&max_field_notice=%E6%B3%A8%E6%84%8F%3A+%E6%97%A0%E6%B3%95%E6%B7%BB%E5%8A%A0%E5%8F%A6%E4%B8%80%E5%AD%97%E6%AE%B5%E3%80%82&input_invalid_notice=%E6%A3%80%E7%B4%A2%E9%94%99%E8%AF%AF%3A+%E8%AF%B7%E8%BE%93%E5%85%A5%E6%A3%80%E7%B4%A2%E8%AF%8D%E3%80%82&exp_notice=%E6%A3%80%E7%B4%A2%E9%94%99%E8%AF%AF%3A+%E4%B8%93%E5%88%A9%E6%A3%80%E7%B4%A2%E8%AF%8D%E5%8F%AF%E5%9C%A8%E5%A4%9A%E4%B8%AA%E5%AE%B6%E6%97%8F%E4%B8%AD%E6%89%BE%E5%88%B0+%28&input_invalid_notice_limits=+%3Cbr%2F%3E%E6%B3%A8%3A+%E6%BB%9A%E5%8A%A8%E6%A1%86%E4%B8%AD%E6%98%BE%E7%A4%BA%E7%9A%84%E5%AD%97%E6%AE%B5%E5%BF%85%E9%A1%BB%E8%87%B3%E5%B0%91%E4%B8%8E%E4%B8%80%E4%B8%AA%E5%85%B6%E4%BB%96%E6%A3%80%E7%B4%A2%E5%AD%97%E6%AE%B5%E7%9B%B8%E7%BB%84%E9%85%8D%E3%80%82&sa_params=WOS%7C%7C" + sID + "%7Chttp%3A%2F%2Fapps.webofknowledge.com%7C%27&formUpdated=true&value%28input1%29=" + title + "&value%28select1%29=TI&value%28hidInput1%29=&value%28bool_1_2%29=AND&value%28input2%29=" + year + "&value%28select2%29=PY&x=917&y=350&value%28hidInput1%29=&limitStatus=collapsed&ss_lemmatization=On&ss_spellchecking=Suggest&SinceLastVisit_UTC=&SinceLastVisit_DATE=&period=Range+Selection&range=ALL&startYear=1900&endYear=2016&editions=SCI&editions=SSCI&editions=ISTP&editions=CCR&editions=IC&update_back2search_link_param=yes&ssStatus=display%3Anone&ss_showsuggestions=ON&ss_numDefaultGeneralSearchFields=1&ss_query_language=&rs_sort_by=PY.D%3BLD.D%3BSO.A%3BVL.D%3BPG.A%3BAU.A",
                Timeout = 100000,//连接超时时间     可选项默认为100000    
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000   
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                Allowautoredirect = false,//是否根据301跳转     可选项   
                ResultType = ResultType.String,
                Host = "apps.webofknowledge.com",
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8"
            };

            item.Header.Add("Origin", "http://apps.webofknowledge.com");

            result = http.GetHtml(item);
            MoveTmp(http, ref result, cookie);

            string url = item.URL;
            var titles = Regex.Matches(result.Html.Replace("\r", "").Replace("\n", ""), reg_title);
            if (titles.Count < 1) {
                return null;
            }

            var tmpUrl = Regex.Match(result.Html, reg_rurl).ToString();
            string rurl = tmpUrl.Substring(tmpUrl.IndexOf("http"), tmpUrl.LastIndexOf("\"") - tmpUrl.IndexOf("http"));
            var tqid = Regex.Match(result.Html, reg_qid).ToString();
            tqid = tqid.Substring(tqid.IndexOf("value"));
            tqid = tqid.Substring(tqid.IndexOf("\"") + 1);
            tqid = tqid.Substring(0, tqid.IndexOf("\""));


            double matchValue = 0;
            string matchTitle = "";
            string matchIndex = "";
            for (int i = 0; i < titles.Count; i++) {
                string str = titles[i].ToString();
                str = str.Substring(str.IndexOf("doc"));
                string index = str.Substring(str.IndexOf("=") + 1);
                index = index.Substring(0, index.IndexOf("\""));
                string tit = str.Substring(str.IndexOf("<value lang_")).Replace("<value lang_id=\"\">", "").Replace("<span class=\"hitHilite\">", "").Replace(@"</span>", "").Replace(@"</value>", "");

                double tmpvalue = Utils.MatchValue(title, tit, isCn);
                if (tmpvalue > matchValue) {
                    matchValue = tmpvalue;
                    matchTitle = tit;
                    matchIndex = index;
                }
            }


            item = new HttpItem() {
                URL = "http://apps.webofknowledge.com/OutboundService.do?action=go&&",//URL     必需项    
                Method = "POST",//URL     可选项 默认为Get   
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                Cookie = cookie,//字符串Cookie     可选项   
                Referer = url,//来源URL     可选项   
                Postdata = "selectedIds=&displayCitedRefs=true&displayTimesCited=true&displayUsageInfo=true&viewType=summary&product=WOS&rurl=" + rurl + "&mark_id=WOS&colName=WOS&search_mode=GeneralSearch&locale=zh_CN&view_name=WOS-summary&sortBy=PY.D%3BLD.D%3BSO.A%3BVL.D%3BPG.A%3BAU.A&mode=OpenOutputService&qid=" + tqid + "&SID=" + sID + "&format=saveToFile&filters=PMID+USAGEIND+AUTHORSIDENTIFIERS+ACCESSION_NUM+FUNDING+SUBJECT_CATEGORY+JCR_CATEGORY+LANG+IDS+PAGEC+SABBR+CITREFC+ISSN+PUBINFO+KEYWORDS+CITTIMES+ADDRS+CONFERENCE_SPONSORS+DOCTYPE+CITREF+ABSTRACT+CONFERENCE_INFO+SOURCE+TITLE+AUTHORS++&mark_to=" + matchIndex + "&mark_from=" + matchIndex + "&queryNatural=%3Cb%3E%E4%B8%BB%E9%A2%98%3A%3C%2Fb%3E+%28" + title + "%29&count_new_items_marked=0&use_two_ets=false&IncitesEntitled=yes&value%28record_select_type%29=range&markFrom=" + matchIndex + "&markTo=" + matchIndex + "&fields_selection=PMID+USAGEIND+AUTHORSIDENTIFIERS+ACCESSION_NUM+FUNDING+SUBJECT_CATEGORY+JCR_CATEGORY+LANG+IDS+PAGEC+SABBR+CITREFC+ISSN+PUBINFO+KEYWORDS+CITTIMES+ADDRS+CONFERENCE_SPONSORS+DOCTYPE+CITREF+ABSTRACT+CONFERENCE_INFO+SOURCE+TITLE+AUTHORS++&save_options=tabWinUTF8",
                Timeout = 100000,//连接超时时间     可选项默认为100000    
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000   
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                //UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
                Allowautoredirect = false,//是否根据301跳转     可选项   
                //CerPath = "d:\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数   
                //Connectionlimit = 1024,//最大连接数     可选项 默认为1024    
                ProxyIp = "",//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数 
                //ProxyPwd = "123456",//代理服务器密码     可选项    
                //ProxyUserName = "administrator",//代理服务器账户名     可选项   
                ResultType = ResultType.String,
                Host = "apps.webofknowledge.com",
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8"
            };

            item.Header.Add("Origin", "http://apps.webofknowledge.com");

            result = http.GetHtml(item);
            MoveTmp(http, ref result, cookie);

            string res = result.Html;
            string txt = res.Replace("\n", "").Split('\r')[1];
            string[] resArr = res.Replace("\n", "").Split('\r')[1].Split('\t');
            sReturn = new string[resArr.Length + 3];
            Array.Copy(resArr, sReturn, resArr.Length);
            sReturn[resArr.Length] = txt;
            sReturn[resArr.Length + 1] = matchTitle;
            sReturn[resArr.Length + 2] = matchValue.ToString();
            return sReturn;
        }

        /// <summary>
        /// move
        /// </summary>
        /// <param name="http"></param>
        /// <param name="result"></param>
        /// <param name="cookie"></param>
        private void MoveTmp(HttpHelper http, ref HttpResult result, string cookie) {
            HttpItem item;
            string c = "";
            if (string.IsNullOrEmpty(cookie)) {
                c = result.Cookie;
            } else {
                c = cookie;
            }
            while (result.StatusDescription == "Moved Temporarily") {
                item = new HttpItem() {
                    URL = result.Header.GetValues("Location")[0],//URL     必需项    
                    Method = "get",//URL     可选项 默认为Get   
                    IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                    Cookie = c,//字符串Cookie     可选项   
                    Referer = "",//来源URL     可选项   
                    Postdata = "",//Post数据     可选项GET时不需要写   
                    Timeout = 100000,//连接超时时间     可选项默认为100000    
                    ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000   
                    //UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                    ContentType = "text/html",//返回类型    可选项有默认值   
                    Allowautoredirect = false,//是否根据301跳转     可选项   
                    //CerPath = "d:\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数   
                    //Connectionlimit = 1024,//最大连接数     可选项 默认为1024    
                    ProxyIp = "",//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数 
                    //ProxyPwd = "123456",//代理服务器密码     可选项    
                    //ProxyUserName = "administrator",//代理服务器账户名     可选项   
                    ResultType = ResultType.String
                };
                result = http.GetHtml(item);
                if (!string.IsNullOrEmpty(result.Cookie)) {
                    if (result.Cookie.Contains("JSESSIONID=")) {
                        string str = result.Cookie;
                        jessionID = str.Substring(str.IndexOf("JSESSIONID="));
                        jessionID = jessionID.Substring(jessionID.IndexOf("=") + 1);
                        jessionID = jessionID.Substring(0, 32);
                    }
                    if (result.Cookie.Contains("SID=")) {
                        string str = result.Cookie;
                        sID = str.Substring(str.IndexOf("SID="));
                        sID = sID.Substring(sID.IndexOf("\"") + 1);
                        sID = sID.Substring(0, sID.IndexOf("\""));
                    }
                }

            }
        }
    }
}
