using Core.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Core.Searcher {
    class DBSearcher {
        public WosData Search(string title) {
            title = title.Replace("'", "\\'").Replace("\"", "\\\"");
            DataTable dt = DBConnector.MySqlCon.ExecSql("select * from titlematch where title = '" + title + "'");
            if (dt.Rows.Count < 1) {
                return null;
            } else {
                string ut = dt.Rows[0]["UT"].ToString();
                DataTable dt1 = DBConnector.MySqlCon.ExecSql("select * from wosdata where UT='" + ut + "'");
                object[] datas = dt1.Rows[0].ItemArray;
                WosData wosData = new WosData();
                wosData.setDataArray(datas);
                return wosData;
            }
        }
    }
}
