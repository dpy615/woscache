using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using Core.Entity;

namespace Core.DBConnector {
    public class MySqlCon {
        public static DataTable ExecSql(string sql) {
            DataTable dt = new DataTable();

            using (MySqlConnection con = new MySqlConnection(SearcherTool.ConnectString)) {
                con.Open();
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(sql, con)) {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public static void ExecInsert(string sql) {
            using (MySqlConnection con = new MySqlConnection(SearcherTool.ConnectString)) {
                con.Open();
                using (MySqlCommand command = new MySqlCommand(sql, con)) {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void SaveWosData(WosData wosData) {
            string sql = "insert into wosdata(PT,AU,BA,BE,GP,AF,BF,CA,TI,SO,SE,BS,LA,DT,CT,CY,CL,SP,HO,DE,ID,AB,C1,RP,EM,RI,OI,FU,FX,CR,NR,TC,Z9,U1,U2,PU,PI,PA,SN,EI,BN,J9,JI,PD,PY,VL,`IS`,PN,SU,SI,MA,BP,EP,AR,DI,D2,PG,WC,SC,GA,UT,PM) values(";
            sql += GetInsertStr(wosData);
            sql += ")";
            ExecInsert(sql);
        }

        public static string GetInsertStr(WosData wosData) {
            string[] datas = wosData.getDataArray();
            string sReturn = "'";
            for (int i = 0; i < 62; i++) {
                if (i != 61) {
                    sReturn += datas[i].Replace("'","\\'").Replace("\"","\\\"") + "','";
                } else {
                    sReturn += datas[i].Replace("'", "\\'").Replace("\"", "\\\"") + "'";
                }
            }
            return sReturn;
        }


        public static void SaveMatchData(WosData wosData, string title) {
            string sql = string.Format("insert into titlematch values('{0}','{1}','{2}','{3}')",
                title,
                wosData.UT,
                wosData.getDataArray()[wosData.getDataArray().Length-1],
                "");
            ExecInsert(sql);
        }
    }
}
