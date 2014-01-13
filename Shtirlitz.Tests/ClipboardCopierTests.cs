using System.Collections.Generic;
using System.IO;
using Shtirlitz.Sender;
using Xunit;

namespace Shtirlitz.Tests
{
    public class ClipboardCopierTests : ShtirlitzBaseTestClass
    {
        public ClipboardCopierTests()
            : this(new ClipboardCopier())
        { }

        public ClipboardCopierTests(ClipboardCopier copier)
            : base (null, new List<ISender> { copier })
        { }

        [Fact(Skip = "This test is intended for checking by hand")] // remove the skip before running a test manually
        public void CutsTheFile()
        {
            // act
            RunSynchronously(true);

            // assert
            Assert.True(File.Exists(ArchiveFilename));

            // put breakpoint here press Ctrl+V in Windows Explorer to check the operation
            // without a breakpoint, file will be removed by Dispose method
        }
    }
}