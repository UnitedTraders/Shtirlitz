using System.Threading;
using Shtirlitz.Common;

namespace Shtirlitz.Archiver
{
    public class ArchiverStage : DelegateStage
    {
        private const string Archiving = "Archiving";
        private const ReportStageType ArchivingType = ReportStageType.Archiving;

        private static readonly IArchiver defaultArchiver = new DotNetZipArchiver();

        public ArchiverStage(string rootPath, string archiveFilename, CancellationToken cancellationToken)
            : this(rootPath, archiveFilename, defaultArchiver, cancellationToken)
        { }

        public ArchiverStage(string rootPath, string archiveFilename, IArchiver archiver, CancellationToken cancellationToken)
            : base(rootPath, Archiving, ArchivingType, cancellationToken, pc => archiver.Archive(rootPath, archiveFilename, cancellationToken, pc))
        { }

        public static IArchiver DefaultArchiver
        {
            get { return defaultArchiver; }
        }
    }
}