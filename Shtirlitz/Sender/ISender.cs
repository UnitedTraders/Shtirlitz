using System.Threading;
using Shtirlitz.Common;

namespace Shtirlitz.Sender
{
    /// <summary>
    /// Describes an interface of the report sender.
    /// </summary>
    public interface ISender
    {
        /// <summary>
        /// Sends the report file at <paramref name="archiveFilename"/>.
        /// 
        /// Supply the <paramref name="progressCallback"/> with an appropriate function, if you want the reports on a progress.
        /// 
        /// This method should execute synchronously and only exit when all of the work was done.
        /// </summary>
        void Send(string archiveFilename, CancellationToken cancellationToken, SimpleProgressCallback progressCallback = null);
    }
}