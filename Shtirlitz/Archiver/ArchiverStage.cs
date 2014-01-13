using System.Threading;
using Shtirlitz.Common;

namespace Shtirlitz.Archiver
{
    public class ArchiverStage : DelegateStage
    {
        private const string Archiving = "Archiving";
        private const ReportStageType ArchivingType = ReportStageType.Archiving;

        public ArchiverStage(string rootPath, string archiveFilename, IArchiver archiver, CancellationToken cancellationToken)
            : base(rootPath, Archiving, ArchivingType, cancellationToken, pc => archiver.Archive(rootPath, archiveFilename, cancellationToken, pc))
        { }
    }
}