using System.Collections.Generic;
using System.IO;
using Shtirlitz.Sender;
using Xunit;

namespace Shtirlitz.Tests
{
    public class ArchiveRemoverTests : ShtirlitzBaseTestClass
    {
        public ArchiveRemoverTests()
            : this(new ArchiveRemover())
        { }

        public ArchiveRemoverTests(ArchiveRemover remover)
            : base (null, new List<ISender> { remover })
        { }

        [Fact]
        public void RemovesTheArchiveFile()
        {
            // act
            RunSynchronously(true);

            // assert
            Assert.False(File.Exists(ArchiveFilename));
        }
    }
}