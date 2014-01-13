using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Shtirlitz.Archiver;
using Shtirlitz.Reporter;
using Shtirlitz.Sender;
using Shtirlitz.Tests.Dummies;

namespace Shtirlitz.Tests
{
    /// <summary>
    /// Base class for the tests that creates a shtirlitz and cleans up after running a test.
    /// </summary>
    public abstract class ShtirlitzBaseTestClass : IDisposable
    {
        private readonly DummyReporter reporter = new DummyReporter();
        private readonly DummySender dummySender = new DummySender();
        private readonly Shtirlitz shtirlitz;
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

        // these fields are used for a cleanup after running a test
        private string rootPath;
        private readonly string archiveFilename;

        protected ShtirlitzBaseTestClass(List<IReporter> otherReporters = null, List<ISender> otherSenders = null)
        {
            List<IReporter> reporters = new List<IReporter> { reporter };
            List<ISender> senders = new List<ISender> { dummySender };

            if (otherReporters != null)
            {
                reporters.AddRange(otherReporters);
            }
            if (otherSenders != null)
            {
                senders.AddRange(otherSenders);
            }

            shtirlitz = new Shtirlitz(reporters, new DotNetZipArchiver(), senders);

            // generate archive file name
            archiveFilename = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".zip");
        }

        protected void RunSynchronously(bool cleanup)
        {
            Task mainTask = shtirlitz.Start(tokenSource.Token, out rootPath, cleanup, archiveFilename);

            // wait for the report to finish generating
            mainTask.Wait();
        }

        protected CancellationTokenSource TokenSource
        {
            get { return tokenSource; }
        }

        protected string RootPath
        {
            get { return rootPath; }
        }

        protected string ArchiveFilename
        {
            get { return archiveFilename; }
        }

        protected Shtirlitz Shtirlitz
        {
            get { return shtirlitz; }
        }

        internal DummyReporter Reporter
        {
            get { return reporter; }
        }

        internal DummySender Sender
        {
            get { return dummySender; }
        }

        public void Dispose()
        {
            Console.WriteLine("Root path: {0}", rootPath);
            Console.WriteLine("Archive file name: {0}", archiveFilename);

            Shtirlitz.PerformFullCleanup(rootPath, archiveFilename);
        }
    }
}