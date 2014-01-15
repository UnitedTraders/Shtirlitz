using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shtirlitz.Reporter;
using Xunit;
using System.IO;
namespace Shtirlitz.Tests
{
	public class TimeReporterTests : ShtirlitzBaseTestClass
	{
		readonly TimeReporter _reporter;
		static DateTimeOffset _customTime = new DateTime(2014, 1, 15, 0, 0, 0);

		public TimeReporterTests()
			: this(new TimeReporter(new TestTimeProvider(_customTime)))
		{
		}

		public TimeReporterTests(TimeReporter reporter)
			: base(new List<IReporter> { reporter })
		{
			_reporter = reporter;
		}

		[Fact]
		public void TimeReporter_ReportTest()
		{
			// act
			RunSynchronously(false);

			// assert
			Assert.True(File.Exists(Path.Combine(RootPath, _reporter.GetFileNameFromTime(_customTime))));
		}

		class TestTimeProvider : ITimeProvider
		{
			DateTimeOffset _time;

			public TestTimeProvider(DateTimeOffset time)
			{
				_time = time;
			}

			public DateTimeOffset GetTime()
			{
				return _time;
			}
		}
	}
}
