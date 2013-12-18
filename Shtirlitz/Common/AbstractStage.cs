using System.Threading;

namespace Shtirlitz.Common
{
    public abstract class AbstractStage : IStage
    {
        private readonly string rootPath;
        private readonly string name;
        private readonly ReportStageType type;
        private readonly CancellationToken cancellationToken;
        
        protected AbstractStage(string rootPath, string name, ReportStageType type, CancellationToken cancellationToken)
        {
            this.rootPath = rootPath;
            this.name = name;
            this.type = type;
            this.cancellationToken = cancellationToken;
        }

        public string RootPath
        {
            get { return rootPath; }
        }

        public virtual double Weight
        {
            get { return 1.0; }
        }

        public string Name
        {
            get { return name; }
        }

        public ReportStageType Type
        {
            get { return type; }
        }

        public CancellationToken CancellationToken
        {
            get { return cancellationToken; }
        }

        public event ProgressCallback Progress;

        protected void RaiseProgress(double progress)
        {
            RaiseProgress(progress, name, type);
        }

        protected virtual void RaiseProgress(double progress, string stageName, ReportStageType stageType)
        {
            ProgressCallback handler = Progress;
            if (handler != null)
            {
                handler(progress, stageName, stageType);
            }
        }

        public abstract void Run();
    }
}