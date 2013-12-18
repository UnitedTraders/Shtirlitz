using System.IO;
using System.Threading;

namespace Shtirlitz.Common
{
    /// <summary>
    /// The stage that removes the root directory.
    /// </summary>
    public class CleanupStage : DelegateStage
    {
        private const string Cleanup = "Cleanup";
        private const ReportStageType CleanupType = ReportStageType.Cleanup;

        public CleanupStage(string rootPath, CancellationToken cancellationToken)
            : base(rootPath, Cleanup, CleanupType, cancellationToken, pc => CleanupRoot(rootPath, pc))
        { }

        private static void CleanupRoot(string rootPath, SimpleProgressCallback progressCallback)
        {
            if (!Directory.Exists(rootPath))
            {
                // nothing to remove
                return;
            }

            Directory.Delete(rootPath, true);

            // notify of the completion
            progressCallback(1.0);
        }
    }
}