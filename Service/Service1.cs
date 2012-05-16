using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace ree7.WakeMyPC.Service
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
            try
            {
                if (!System.Diagnostics.EventLog.SourceExists("WakeMyPCService"))
                {
                    System.Diagnostics.EventLog.CreateEventSource(
                        "WakeMyPCService", "RunLog");
                }
                eventLog1.Source = "WakeMyPCService";
                eventLog1.Log = "RunLog";
            }
            catch
            {

            }
        }

        public EventLog ServiceEventLog
        {
            get { return eventLog1; }
        }

        protected override void OnStart(string[] args)
        {
            ServiceEventLog.WriteEntry("Wake my PC Companion Service started.");

        }

        protected override void OnStop()
        {
            ServiceEventLog.WriteEntry("Wake my PC Companion Service stopped.");
        }
    }
}
