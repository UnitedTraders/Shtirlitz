using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Samples.Debugging.MdbgEngine;
using System.Reflection;


namespace Shtirlitz.Reporter
{
    public class ThreadReporter:IReporter
    {
        public void Report(string rootPath, System.Threading.CancellationToken cancellationToken, Common.SimpleProgressCallback progressCallback = null)
        {
            // get a file name for the report
            string filename = Path.Combine(rootPath, "ProcessManagedThreads.html");

            //ThreadsWatcherAppName
            string threadsWatcher = @"ThreadsWatcher.exe";

            // dump report file
            ReporterUtils.DumpOutputToFile(threadsWatcher, Process.GetCurrentProcess().ProcessName, filename, true);

        }

        public double Weight
        {
            get { return 2d; }
        }

        public string Name
        {
            get { return "Dumping information about managed threads in a current process"; }
        }
    }
}
