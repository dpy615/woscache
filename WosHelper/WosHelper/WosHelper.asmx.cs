using Core;
using Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WosHelper {
    /// <summary>
    /// Summary description for WosHelper
    /// </summary>
    [WebService(Namespace = "WosHelper")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WosHelper : System.Web.Services.WebService {
        SearcherTool searcher = new SearcherTool();
        [WebMethod(Description = "通过标题来检索")]
        public WosData SearchByTitle(string title) {
            WosData wosData = searcher.Search(title);
            return wosData;
        }

        [WebMethod(Description = "通过标题和出版年来检索")]
        public WosData SearchByTitleAndYear(string title, string year) {
            WosData wosData = null;
            if (string.IsNullOrEmpty(year)) {
                wosData = searcher.Search(title);
            } else {
                wosData = searcher.Search(title, year);
            }
            return wosData;
        }


        [WebMethod(Description = "获取当前系统的数据库连接字符串")]
        public string GetConnectString() {
            return SearcherTool.ConnectString;
        }

        [WebMethod(Description = "通过标题来检索（仅检索数据库中内容）")]
        public WosData SearchByTitleDB(string title) {
            WosData wosData = searcher.Search(title);
            return wosData;
        }

        [WebMethod(Description = "通过标题来检索（不检索数据库）")]
        public WosData SearchByTitleNoDB(string title) {
            WosData wosData = searcher.Search(title);
            return wosData;
        }

        [WebMethod(Description = "通过标题和出版年来检索（不检索数据库）")]
        public WosData SearchByTitleAndYearNoDB(string title, string year) {
            WosData wosData = null;
            if (string.IsNullOrEmpty(year)) {
                wosData = searcher.Search(title);
            } else {
                wosData = searcher.Search(title, year);
            }
            return wosData;
        }
    }
}
