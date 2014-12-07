using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Shtirlitz.Common;
using Shtirlitz.Reporter;
using Shtirlitz.Reporter.TraceRouteReporter;
using Xunit;

namespace Shtirlitz.Tests.TraceRouteReporterTests
{
    public class MultipleTraceRouteReporterTests : ShtirlitzBaseTestClass
    {
        private readonly MultipleTraceRouteReporter _reporter;

        public MultipleTraceRouteReporterTests()
            : this(new MultipleTraceRouteReporter(new TestMultiTracer()))
        {
        }

        public MultipleTraceRouteReporterTests(MultipleTraceRouteReporter reporter)
            : base(new List<IReporter> { reporter })
        {
            _reporter = reporter;
            _reporter.Addresses.AddRange(new[]{"1","2"});
        }

        [Fact]
        public void CreatesTheRightFile()
        {
            // act
            RunSynchronously(false);

            
            // assert
            Assert.True(File.Exists(Path.Combine(RootPath, "TraceRouteReport__1_2.txt")));
        }


        private class TestMultiTracer : IMultiTracer
        {
            public int MaxHops { get; set; }
            public int PingTimeout { get; set; }
            public int PingTimeFrame { get; set; }
            public int PackageSize { get; set; }
            public int NumberOfRounds { get; set; }

            public void Trace(
                IEnumerable<string> addresslist,
                Action<string> printResults,
                CancellationToken token,
                SimpleProgressCallback progressCallback = null)
            {
                foreach (var a in addresslist)
                {
                    printResults(string.Format("{0}_test", a));
                    token.ThrowIfCancellationRequested();
                }
            }
        }
        
    }


   

  
    
}
