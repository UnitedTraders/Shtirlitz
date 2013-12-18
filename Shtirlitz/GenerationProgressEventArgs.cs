using System;

namespace Shtirlitz
{
    public class GenerationProgressEventArgs : EventArgs
    {
        private readonly double progress;
        private readonly string stageName;
        private readonly ReportStageType stageType;

        public GenerationProgressEventArgs(double progress, string stageName, ReportStageType stageType)
        {
            this.progress = progress;
            this.stageName = stageName;
            this.stageType = stageType;
        }

        /// <summary>
        /// Gets the overall progress of the report generation.
        /// 
        /// Value of 0.0 means that no progress was made. Value of 1.0 means that operation was completed.
        /// </summary>
        public double Progress
        {
            get { return progress; }
        }

        /// <summary>
        /// Gets the name of the report generation stage that executes now.
        /// </summary>
        public string StageName
        {
            get { return stageName; }
        }

        /// <summary>
        /// Gets the type of the current stage.
        /// </summary>
        public ReportStageType StageType
        {
            get { return stageType; }
        }
    }
}