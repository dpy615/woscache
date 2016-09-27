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
            WosData wosData = wosSearcher.Search(title);
            return wosData;
        }
    }
}
