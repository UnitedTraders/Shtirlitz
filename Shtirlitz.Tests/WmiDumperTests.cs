using System.Collections.Generic;
using System.IO;
using Shtirlitz.Reporter;
using Xunit;

namespace Shtirlitz.Tests
{
    public class WmiDumperTests : ShtirlitzBaseTestClass
    {
        private readonly WmiDumper dumper;

        public WmiDumperTests()
            : this(new WmiDumper())
        { }

        public WmiDumperTests(WmiDumper dumper)
            : base (new List<IReporter>{ dumper })
        {
            this.dumper = dumper;
        }

        [Fact]
        public void DumpsWmiData()
        {
            // act
            RunSynchronously(false);

            // assert
            foreach (string alias in dumper.Aliases)
            {
                Assert.True(File.Exists(Path.Combine(RootPath, WmiDumper.PathSuffix, string.Format("{0}.html", alias))));
            }
        }
    }
}