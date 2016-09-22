using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Core.DBConnector {
    public class MySqlCon {
        private  static string ConnectString = "Server=192.168.139.131;Database=woscache; User=wos;Password=wos;Use Procedure Bodies=false;Charset=utf8;Allow Zero Datetime=True; Pooling=false; Max Pool Size=50;";

        public static DataTable ExecSql(string sql) {
            DataTable dt = new DataTable();

            using (MySqlConnection con = new MySqlConnection(ConnectString)) {
                con.Open();
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(sql,con)) {
                    adapter.Fill(dt);
                }
            }

            return dt;
        }
        
    }
}
