using System.Threading;
using Ionic.Zip;
using Shtirlitz.Common;

namespace Shtirlitz.Archiver
{
    public class DotNetZipArchiver : IArchiver
    {
        public void Archive(string rootPath, string archiveFilename, CancellationToken cancellationToken, SimpleProgressCallback progressCallback = null)
        {
            using (ZipFile file = new ZipFile(archiveFilename))
            {
                // add all the files from the report to the zip file
                file.AddDirectory(rootPath);

                // subscribe for the archiving progress event
                file.SaveProgress += (sender, args) =>
                                         {
                                             // check if the cancellation was requested
                                             if (cancellationToken.IsCancellationRequested)
                                             {
                                                 args.Cancel = true;
                                                 return;
                                             }

                                             if (args.EntriesTotal == 0)
                                             {
                                                 // avoid division by zero
                                                 return;
                                             }

                                             progressCallback.TryInvoke((args.EntriesSaved + (args.TotalBytesToTransfer > 0 ? 1.0 * args.BytesTransferred / args.TotalBytesToTransfer : 0.0)) / args.EntriesTotal);
                                         };

                // safe file to disk
                file.Save();

                cancellationToken.ThrowIfCancellationRequested();
            }

            // report operation finished
            progressCallback.TryInvoke(1.0);
        }
    }
}