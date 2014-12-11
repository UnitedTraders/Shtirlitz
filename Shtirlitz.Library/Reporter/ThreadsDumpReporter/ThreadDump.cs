using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace Shtirlitz.Reporter.ThreadsDumpReporter
{
    /// <summary>
    /// Thread dump
    /// </summary>
    public class ThreadDump
    {
        public string Name { get; protected set; }
        public int ManagedThreadId { get; protected set; }
        public StackTrace StackTrace { get; protected set; }

        public ThreadDump(string name, int managedThreadId, StackTrace stackTrace)
        {
            this.Name = name;
            this.ManagedThreadId = managedThreadId;
            this.StackTrace = stackTrace;
        }
    }
}
