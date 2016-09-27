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
        [WebMethod]
        public WosData Searcher(string title) {
            WosData wosData = searcher.Search(title);
            return wosData;
        }
    }
}
