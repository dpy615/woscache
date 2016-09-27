using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Searcher;
using Core.Entity;

namespace Core {
    public class SearcherTool {

        private WosSearcher wosSearcher;
        private DBSearcher dbSearcher;

        public SearcherTool() {
            wosSearcher = new WosSearcher();
            dbSearcher = new DBSearcher();
        }
        public WosData Search(string title) {
            WosData wosData = dbSearcher.Search(title);
            if (wosData != null) {
                return wosData;
            }
            wosData = wosSearcher.Search(title);
            if (wosData != null) {
                double matchTmp = 0;
                double.TryParse(wosData.getDataArray()[62], out matchTmp);
                if (matchTmp > 8) {
                    DBConnector.MySqlCon.SaveMatchData(wosData, title);
                    DBConnector.MySqlCon.SaveWosData(wosData);
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
