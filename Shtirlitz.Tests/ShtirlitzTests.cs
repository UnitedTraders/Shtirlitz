using System;
using System.IO;
using System.Threading;
using Shtirlitz.Tests.Dummies;
using Xunit;

namespace Shtirlitz.Tests
{
    public class ShtirlitzTests : ShtirlitzBaseTestClass
    {
        [Fact]
        public void GeneratesFilesAndArchive()
        {
            RunSynchronously(false);

            DummyReporter.AssertReportGenerated(RootPath);
            Assert.True(File.Exists(ArchiveFilename));
        }

        [Fact]
        public void CleansUpDirectoryOnSuccess()
        {
            RunSynchronously(true);

            Assert.False(Directory.Exists(RootPath));
        }

        [Fact]
        public void LeavesArchiveFileOnSuccess()
        {
            RunSynchronously(true);

            Assert.True(File.Exists(ArchiveFilename));
        }

        [Fact]
        public void CleansUpOnCancel()
        {
            TokenSource.Cancel();

            Assert.Throws<AggregateException>(() => RunSynchronously(true));

            // there is no way to wait reliably for a cleanup task :(
            Thread.Sleep(1000);

            Assert.False(Directory.Exists(RootPath));
            Assert.False(File.Exists(ArchiveFilename));
        }

        [Fact]
        public void CleansUpOnFail()
        {
            Reporter.ShouldFail = true;

            Assert.Throws<AggregateException>(() => RunSynchronously(true));

            // there is no way to wait reliably for a cleanup task :(
            Thread.Sleep(1000);

            Assert.False(Directory.Exists(RootPath));
            Assert.False(File.Exists(ArchiveFilename));
        }

        [Fact]
        public void RaisesPositiveEvents()
        {
            string reportFilename = null;

            Shtirlitz.ReportGenerated += (sender, args) => reportFilename = args.Filename;

            double previousProgress = 0.0;
            int progressReports = 0;

            Shtirlitz.GenerationProgress +=
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
            Assert.Equal(ArchiveFilename, reportFilename);
            Assert.NotEqual(0, progressReports);
            Assert.Equal(1.0, previousProgress);
        }

        [Fact]
        public void RaisesNegativeEventsOnCancel()
        {
            bool cancelEventCalled = false;
            Shtirlitz.ReportCanceled += (sender, args) => cancelEventCalled = true;

            TokenSource.Cancel();

            Assert.Throws<AggregateException>(() => RunSynchronously(true));

            // wait a bit for a continuation to execute
            Thread.Sleep(1000);

            Assert.True(cancelEventCalled);
        }

        [Fact]
        public void RaisesNegativeEventsOnFail()
        {
            bool cancelEventCalled = false;
            Shtirlitz.ReportCanceled += (sender, args) => cancelEventCalled = true;

            Reporter.ShouldFail = true;

            Assert.Throws<AggregateException>(() => RunSynchronously(true));

            // wait a bit for a continuation to execute
            Thread.Sleep(1000);

            Assert.True(cancelEventCalled);
        }

        [Fact]
        public void InvokesSender()
        {
            RunSynchronously(true);

            // check
            Assert.Equal(ArchiveFilename, Sender.ArchiveFilename);
        }
    }
}