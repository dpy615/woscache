using System;
using System.Collections.Generic;
using System.Text;
using Core.DBConnector;
using System.Data;
namespace Core {
    class Program {
        static void Main(string[] args) {

            MySqlCon.CheckWosData();
            MySqlCon.CheckTitleMatch();
            Console.WriteLine("dfd");
            Console.Read();
        }
    }
}
