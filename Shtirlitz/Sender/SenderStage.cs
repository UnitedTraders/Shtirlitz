using System.Threading;
using Shtirlitz.Common;

namespace Shtirlitz.Sender
{
    public class SenderStage : DelegateStage
    {
        /// <summary>
        /// This stage is different in that the <see cref="AbstractStage.RootPath"/> for this class is actually an archive file name.
        /// </summary>
        public SenderStage(string archiveFilename, ISender sender, CancellationToken cancellationToken)
            : base(archiveFilename, "Sending the report", ReportStageType.Sending, cancellationToken, pc => sender.Send(archiveFilename, cancellationToken, pc))
        { }
    }
}