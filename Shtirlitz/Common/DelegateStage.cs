using System;
using System.Threading;

namespace Shtirlitz.Common
{
    public abstract class DelegateStage : AbstractStage
    {
        private readonly Action<SimpleProgressCallback> runAction;

        protected DelegateStage(string rootPath, string name, ReportStageType type, CancellationToken cancellationToken, Action<SimpleProgressCallback> runAction)
            : base(rootPath, name, type,cancellationToken)
        {
            this.runAction = runAction;
        }

        public override void Run()
        {
            runAction(RaiseProgress);
        }
    }
}