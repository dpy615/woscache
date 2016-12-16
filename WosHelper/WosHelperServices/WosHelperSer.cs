using Core;
using System;
using System.ServiceProcess;

namespace WosHelperServices
{
    partial class WosHelperSer : ServiceBase {
        SearcherTool tool;
        public WosHelperSer() {
            InitializeComponent();
        }

        public void start()
        {
            tool = new SearcherTool();
            tool.start();
        }

        protected override void OnStart(string[] args) {
            tool = new SearcherTool();
            tool.start();
        }

        protected override void OnStop()
        {
            if (tool != null)
            {
                tool.stop();
            }
        }
    }
}
