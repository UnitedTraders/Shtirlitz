using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Shtirlitz.Common;


namespace Shtirlitz.Reporter.ThreadsDumpReporter
{
    /// <summary>
    /// Thread dump reporter
    /// </summary>
    public sealed class ThreadsDumpReporter : MultiFileReporter
    {
        public const string PathSuffix = "threads";

        private readonly object _syncRoot = new object(); // sync acces for class
        private readonly List<Thread> _threads = new List<Thread>(); // list registered threads


        public ThreadsDumpReporter()
            : base(PathSuffix)
        {
            // empty
        }


        #region MultiFileReporter
        public override double Weight
        {
            get { return 1d; }
        }


        public override string Name
        {
            get { return "Getting threads dump"; }
        }


        protected override void ReportInternal(string path, CancellationToken cancellationToken, SimpleProgressCallback progressCallback = null)
        {
            List<ThreadDump> listDumps = CreateDumps(true);

            for (int d = 0; d < listDumps.Count; d++)
            {
                var dump = listDumps[d];

                // create a file name for the report
                string filename = Path.Combine(path, string.Format("{0}.txt", dump.ManagedThreadId));

                StringBuilder strReport = new StringBuilder();
                strReport.AppendLine(dump.ManagedThreadId.ToString());  // thread id
                strReport.AppendLine(dump.Name);                        // thread name

                StackTrace stack = dump.StackTrace;                     // thread stack trace
                if (stack != null)
                {
                    for(int f = 0; f < stack.FrameCount; f++)
                    {
                        strReport.AppendLine(stack.GetFrame(f).ToString());
                    }
                }

                File.WriteAllText(filename, strReport.ToString());      // write to file

                // report progress
                progressCallback.TryInvoke((d + 1d) / listDumps.Count);

                // exit, if cancellation was requested
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
        #endregion


        public void RegisterThread(Thread thread)
        {
            lock (_syncRoot)
            {
                if (!_threads.Contains(thread))
                    _threads.Add(thread);
            }
        }


        public void UnregisterThread(Thread thread)
        {
            lock (_syncRoot)
            {
                _threads.Remove(thread);
            }
        }


        public List<ThreadDump> CreateDumps(bool needFileInfo)
        {
            // copy _threads to temp array
            Thread[] arrThreads;
            lock (_syncRoot)
            {
                arrThreads = _threads.ToArray();
            }

            List<ThreadDump> dumpList = new List<ThreadDump>(); //result

            foreach (var thread in arrThreads)
            {
                // try get stack trace for thread (started && !stopped)
                StackTrace stackTrace;
                try
                {
                    stackTrace = GetThreadStackTrace(thread, needFileInfo);
                }
                catch (ThreadStateException) // thread unstarted or stopped
                {
                    stackTrace = null;
                }

                // create dump
                ThreadDump dump = new ThreadDump(thread.Name, thread.ManagedThreadId, stackTrace);

                dumpList.Add(dump);
            }

            return dumpList;
        }


        public static StackTrace GetThreadStackTrace(Thread targetThread, bool needFileInfo)
        {
            StackTrace stackTrace;
            if (targetThread == Thread.CurrentThread) // current thread
            {
                stackTrace = new StackTrace(needFileInfo); // skip ... frames (reporter stack)
            }
            else // other thread
            {
                targetThread.Suspend();
                try
                {
                    stackTrace = new StackTrace(targetThread, needFileInfo);
                }
                finally
                {
                    targetThread.Resume();
                }
            }
            return stackTrace;
        }
    }
}
