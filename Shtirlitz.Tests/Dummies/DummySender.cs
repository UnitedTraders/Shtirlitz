using System.Threading;
using Shtirlitz.Common;
using Shtirlitz.Sender;

namespace Shtirlitz.Tests.Dummies
{
    internal class DummySender : ISender
    {
        public void Send(string archiveFilename, CancellationToken cancellationToken, SimpleProgressCallback progressCallback = null)
        {
            // just store the archive file name for verification
            ArchiveFilename = archiveFilename;
        }

        public string ArchiveFilename { get; set; }
    }
}