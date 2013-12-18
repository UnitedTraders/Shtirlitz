using System.Threading;
using Shtirlitz.Common;

namespace Shtirlitz.Archiver
{
    /// <summary>
    /// Archives all the info into the one file.
    /// </summary>
    public interface IArchiver
    {
        /// <summary>
        /// Adds all the files from <paramref name="rootPath"/> to the archive file with the name <paramref name="archiveFilename"/>.
        /// 
        /// Supply the <paramref name="progressCallback"/> with an appropriate function, if you want the reports on a progress.
        /// </summary>
        void Archive(string rootPath, string archiveFilename, CancellationToken cancellationToken, SimpleProgressCallback progressCallback = null);
    }
}