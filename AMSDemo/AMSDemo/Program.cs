using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Timers;
using AMSDemo.Controllers;

namespace AMSDemo
{
    public class Program
    {
        private static System.Timers.Timer opldTimer;
        private static System.Timers.Timer dialsTimer;

        public static void Main(string[] args)
        {
            SetOPLDTimer();
            //SetDIALSTimer();
            CreateWebHostBuilder(args).Build().Run();            
        }

        private static void SetOPLDTimer()
        {
            // Create a timer with a two second interval.
            opldTimer = new System.Timers.Timer(20000);
            // Hook up the Elapsed event for the timer. 
            opldTimer.Elapsed += OnTimedEvent;
            opldTimer.AutoReset = true;
            opldTimer.Enabled = true;
        }

        private static void SetDIALSTimer()
        {
            // Create a timer with a two second interval.
            dialsTimer = new System.Timers.Timer(2000);
            // Hook up the Elapsed event for the timer. 
            dialsTimer.Elapsed += OnDIALSTimedEvent;
            dialsTimer.AutoReset = true;
            dialsTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            ProcessOPLDController processOPLDController = new ProcessOPLDController();
            processOPLDController.OPLDFileWatcher();
            Console.WriteLine("Filew read at {0:HH:mm:ss.fff}",
                              e.SignalTime);
        }

        private static void OnDIALSTimedEvent(Object source, ElapsedEventArgs e)
        {
            ProcessOPLDController processOPLDController = new ProcessOPLDController();
            processOPLDController.DIALSFileWatcher();
            Console.WriteLine("Filew read at {0:HH:mm:ss.fff}",
                              e.SignalTime);
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
