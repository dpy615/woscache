using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using Core.Entity;

namespace Core.DBConnector {
    public class MySqlCon {
        public static string ConnectString = "Server=192.168.139.131;Database=woscache; User=wos;Password=wos;Use Procedure Bodies=false;Charset=utf8;Allow Zero Datetime=True; Pooling=false; Max Pool Size=50;";

        public static DataTable ExecSql(string sql) {
            DataTable dt = new DataTable();

            using (MySqlConnection con = new MySqlConnection(ConnectString)) {
                con.Open();
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(sql, con)) {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public static void ExecInsert(string sql) {
            using (MySqlConnection con = new MySqlConnection(ConnectString)) {
                con.Open();
                using (MySqlCommand command = new MySqlCommand(sql, con)) {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void SaveWosData(WosData wosData) {
            string sql = "insert into wosdata values(";
            sql += GetInsertStr(wosData);
            sql += ")";
        }

        public static string GetInsertStr(WosData wosData) {
            string[] datas = wosData.getDataArray();
            string sReturn = "'";
            for (int i = 0; i < datas.Length; i++) {
                if (i != datas.Length - 1) {
                    sReturn += datas[i] + "','";
                } else {
                    sReturn += datas[i] + "'";
                }
            }
            return sReturn;
        }


        public static void SaveMatchData(WosData wosData, string title) {
            string sql = string.Format("insert into titlematch values('{0}','{1}','{2}',','{3}')",
                title,
                wosData.UT,
                wosData.getDataArray()[62]);
            ExecInsert(sql);
        }
    }
}
