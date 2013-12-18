using System.Threading;
using Shtirlitz.Common;

namespace Shtirlitz.Reporter
{
    public class ReporterStage : DelegateStage
    {
        private readonly IReporter reporter;

        public ReporterStage(string rootPath, IReporter reporter, CancellationToken cancellationToken)
            : base(rootPath, reporter.Name, ReportStageType.CollectingInfo, cancellationToken, pc => reporter.Report(rootPath, cancellationToken, pc))
        {
            this.reporter = reporter;
        }

        public override double Weight
        {
            get { return reporter.Weight; }
        }
    }
}