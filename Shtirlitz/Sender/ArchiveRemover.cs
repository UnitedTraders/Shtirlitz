using System.IO;
using System.Threading;
using Shtirlitz.Common;

namespace Shtirlitz.Sender
{
    /// <summary>
    /// Implements an "<see cref="ISender"/>" that removes the archive file.
    /// 
    /// You can add this "sender" at the end of the queue to automatically clean up the archive file when
    /// previous operations are all successfully completed. (When something goes wrong, <see cref="Shtirlitz"/>
    /// cleans everithing up itself.)
    /// </summary>
    public class ArchiveRemover : ISender
    {
        public void Send(string archiveFilename, CancellationToken cancellationToken, SimpleProgressCallback progressCallback = null)
        {
            File.Delete(archiveFilename);
        }
    }
}