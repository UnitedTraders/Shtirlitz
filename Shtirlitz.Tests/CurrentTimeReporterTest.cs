using System;
using System.IO;
using System.Threading;
using Shtirlitz.Reporter.CurrentTimeReporter;
using Xunit;

namespace Shtirlitz.Tests
{
    public class CurrentTimeReporterTest : ShtirlitzBaseTestClass
    {
        [Fact]
        public void Report()
        {
            const string rootPath = @"C:\Temp";

            var dateTime = DateTime.Now;

            var reporter = new CurrentTimeReporter(dateTime);
            reporter.Report(rootPath, default(CancellationToken));

            Assert.True(File.Exists(Path.Combine(rootPath, reporter.GetFileName(dateTime))));
        }
    }
}