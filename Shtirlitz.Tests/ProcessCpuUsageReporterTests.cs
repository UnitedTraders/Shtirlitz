using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Shtirlitz.Reporter;
using Shtirlitz.Reporter.ProcessCpuUsage;
using Xunit;

namespace Shtirlitz.Tests
{
    public class ProcessCpuUsageReporterTests : ShtirlitzBaseTestClass
    {
        public ProcessCpuUsageReporterTests() : this(new ProcessCpuUsageReporter()) { }
        public ProcessCpuUsageReporterTests(ProcessCpuUsageReporter processCpuUsageReporter)
            : base(new List<IReporter> { processCpuUsageReporter })
        {
        }

        private const string anAlwaysExistingWindowsProcess = "svchost";

        [Fact]
        public void CalculatesProcessesCpuUsage()
        {
            // act
            RunSynchronously(false);

            // assert
            var file = Path.Combine(RootPath, ProcessCpuUsageReporter.REPORT_FILE_NAME);
            var reportContent = File.ReadAllText(file);
            var regex = new Regex(string.Format(@"<tr><td>\d+</td><td>{0}</td><td>\d+</td></tr>", anAlwaysExistingWindowsProcess));
            var hasAtLeastSvchost = regex.IsMatch(reportContent);
            
            Assert.True(hasAtLeastSvchost);
        }
    }
}
