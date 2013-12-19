using System;
using System.IO;
using System.Threading;
using Shtirlitz.Common;
using Shtirlitz.Reporter;
using Xunit;

namespace Shtirlitz.Tests.Dummies
{
    internal class DummyReporter : IReporter
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
}