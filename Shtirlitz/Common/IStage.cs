using System.Threading;

namespace Shtirlitz.Common
{
    /// <summary>
    /// Describes one stage in one report generation.
    /// </summary>
    public interface IStage
    {
        /// <summary>
        /// Gets the root directory of the report.
        /// </summary>
        string RootPath { get; }

        /// <summary>
        /// Gets the relative progress weight of this stage.
        /// 
        /// Basically it should be equal to the number of stages in a composite stage, and 1.0 otherwise.
        /// </summary>
        double Weight { get; }

        /// <summary>
        /// Gets the user-friendly name of the current stage.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the cancellation token for this stage.
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Gets the type of the current stage.
        /// </summary>
        ReportStageType Type { get; }

        /// <summary>
        /// Event is raised periodically to notify about the stage progress.
        /// </summary>
        event ProgressCallback Progress;

        /// <summary>
        /// Synchronously runs the stage.
        /// </summary>
        void Run();
    }
}