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

        public bool SearchOracle(string title,string id)
        {
            title = title.Replace("'", "''");
            DataTable dt = DBConnector.OracleCon.ExecuteSelect("select * from wos_match where title = '" + title + "'");
            if (dt.Rows.Count < 1)
            {
                return false;
            }
            else
            {
                var a  = "title_id,title,ut,titlematchvalue,yearmatch";
                DataRow dr = dt.Rows[0];
                string UT = dr["ut"].ToString();
                string matchValue = dr["titlematchvalue"].ToString();
                DBConnector.OracleCon.SaveMatchData(UT, title, id,matchValue);
                return true;
            }
            
        }

        internal int GetMaxTiTleIndex()
        {
            try
            {
                string sql = "select max(INTERNAL_ID) maxIndex from de_reference where  citation_type='J'";
                DataTable dt = DBConnector.OracleCon.ExecuteSelect(sql);
                string strIndex = dt.Rows[0][0].ToString();
                int intIndex = int.Parse(strIndex);
                return intIndex;
            }
            catch (Exception)
            {
                return 0;
            }
            
        }
    }
}
