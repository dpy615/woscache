﻿using System;
using System.Collections.Generic;
using System.Text;
using Core.DBConnector;
using System.Data;
using System.Configuration;

namespace Core {
    class Program {
        static void Main(string[] args) {

            SearcherTool tool = new SearcherTool();
            tool.start();
            Console.WriteLine("dfd");
            Console.Read();
        }
    }
}
