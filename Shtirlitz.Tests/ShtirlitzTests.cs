using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Shtirlitz.Common;
using Shtirlitz.Reporter;
using Xunit;

namespace Shtirlitz.Tests
{
    public class ShtirlitzTests : IDisposable
    {
        private readonly DummyReporter reporter = new DummyReporter();
        private readonly Shtirlitz shtirlitz;
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

        // these fields are used for a cleanup after running a test
        private string rootPath;
        private readonly string archiveFilename;

        public ShtirlitzTests()
        {
            shtirlitz = new Shtirlitz(new List<IReporter>{ reporter });

            // generate archive file name
            archiveFilename = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".zip");
        }

        private void RunSynchronously(bool cleanup)
        {
            Task mainTask = shtirlitz.Start(tokenSource.Token, out rootPath, cleanup, archiveFilename);

            // wait for the report to finish generating
            mainTask.Wait();
        }

        [Fact]
        public void GeneratesFilesAndArchive()
        {
            RunSynchronously(false);

            DummyReporter.AssertReportGenerated(rootPath);
            Assert.True(File.Exists(archiveFilename));
        }

        [Fact]
        public void CleansUpDirectoryOnSuccess()
        {
            RunSynchronously(true);

            Assert.False(Directory.Exists(rootPath));
            Assert.True(File.Exists(archiveFilename));
        }

        [Fact]
        public void CleansUpOnCancel()
        {
            tokenSource.Cancel();

            Assert.Throws<AggregateException>(() => RunSynchronously(true));

            // there is no way to wait reliably for a cleanup task :(
            Thread.Sleep(1000);

            Assert.False(Directory.Exists(rootPath));
            Assert.False(File.Exists(archiveFilename));
        }

        [Fact]
        public void CleansUpOnFail()
        {
            reporter.ShouldFail = true;

            Assert.Throws<AggregateException>(() => RunSynchronously(true));

            // there is no way to wait reliably for a cleanup task :(
            Thread.Sleep(1000);

            Assert.False(Directory.Exists(rootPath));
            Assert.False(File.Exists(archiveFilename));
        }

        [Fact]
        public void RaisesPositiveEvents()
        {
            string reportFilename = null;

            shtirlitz.ReportGenerated += (sender, args) => reportFilename = args.Filename;

            double previousProgress = 0.0;
            int progressReports = 0;

            shtirlitz.GenerationProgress +=
                (sender, args) =>
                {
                    // check that the progress reported is always bigger than the previous one
                    Assert.True(args.Progress >= previousProgress);

                    // remember previous progress
                    previousProgress = args.Progress;
                    progressReports++;
                };

            // act
            RunSynchronously(true);

            // wait a bit more for the events to be called in the next task
            Thread.Sleep(1000);

            // check
            Assert.Equal(archiveFilename, reportFilename);
            Assert.NotEqual(0, progressReports);
            Assert.Equal(1.0, previousProgress);
        }

        [Fact]
        public void RaisesNegativeEventsOnCancel()
        {
            bool cancelEventCalled = false;
            shtirlitz.ReportCanceled += (sender, args) => cancelEventCalled = true;

            tokenSource.Cancel();

            Assert.Throws<AggregateException>(() => RunSynchronously(true));

            // wait a bit for a continuation to execute
            Thread.Sleep(1000);

            Assert.True(cancelEventCalled);
        }

        [Fact]
        public void RaisesNegativeEventsOnFail()
        {
            bool cancelEventCalled = false;
            shtirlitz.ReportCanceled += (sender, args) => cancelEventCalled = true;

            reporter.ShouldFail = true;

            Assert.Throws<AggregateException>(() => RunSynchronously(true));

            // wait a bit for a continuation to execute
            Thread.Sleep(1000);

            Assert.True(cancelEventCalled);
        }

        private class DummyReporter : IReporter
        {
            public void Report(string rootPath, CancellationToken cancellationToken, SimpleProgressCallback progressCallback = null)
            {
                File.WriteAllText(Path.Combine(rootPath, "test1.txt"), "This is a first test file");
                
                string dirName = Directory.CreateDirectory(Path.Combine(rootPath, "testDir")).FullName;

                cancellationToken.ThrowIfCancellationRequested();

                File.WriteAllText(Path.Combine(dirName, "test2.txt"), "This is a second test file");

                if (ShouldFail)
                {
                    throw new InvalidOperationException("Fail requested");
                }
            }

            public static void AssertReportGenerated(string rootPath)
            {
                Assert.True(File.Exists(Path.Combine(rootPath, "test1.txt")));
                Assert.True(Directory.Exists(Path.Combine(rootPath, "testDir")));
                Assert.True(File.Exists(Path.Combine(rootPath, "testDir/", "test2.txt")));
            }

            public bool ShouldFail { get; set; }

            public double Weight
            {
                get { return 1; }
            }

            public string Name
            {
                get { return "Dummy"; }
            }
        }

        public void Dispose()
        {
            Console.WriteLine("Root path: {0}", rootPath);
            Console.WriteLine("Archive file name: {0}", archiveFilename);

            Shtirlitz.PerformFullCleanup(rootPath, archiveFilename);
        }
    }
}