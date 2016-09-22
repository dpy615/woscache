using System;
using System.Collections.Generic;
using System.Text;
using Core.DBConnector;
using System.Data;
namespace Core {
    class Program {
        static void Main(string[] args) {
            string sql = "select * From wosdata";
            DataTable dt = MySqlCon.ExecSql(sql);
            Console.WriteLine(dt.Rows.Count);
            Console.Read();
        }
    }
}
