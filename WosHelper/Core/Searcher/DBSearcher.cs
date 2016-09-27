using Core.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Core.Searcher {
    class DBSearcher {
        public WosData Search(string title) {
            DataTable dt = DBConnector.MySqlCon.ExecSql("select * from titlematch where title = '" + title + "'");
            if (dt.Rows.Count < 1) {
                return null;
            } else {
                string ut = dt.Rows[0]["UT"].ToString();
                DataTable dt1 = DBConnector.MySqlCon.ExecSql("select * from woscache where UT='" + ut + "'");
                string[] datas = (string[])dt1.Rows[0].ItemArray;
                WosData wosData = new WosData();
                wosData.setDataArray(datas);
                return wosData;
            }
        }
    }
}
