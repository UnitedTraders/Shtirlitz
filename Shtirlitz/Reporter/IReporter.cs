using System.Threading;
using Shtirlitz.Common;

namespace Shtirlitz.Reporter
{
    /// <summary>
    /// Describes the interface of an info reporter.
    /// </summary>
    public interface IReporter
    {
        /// <summary>
        /// Dumps all the information on disk relative to the <paramref name="rootPath"/>.
        /// 
        /// Supply the <paramref name="progressCallback"/> with an appropriate function, if you want the reports on a progress.
        /// </summary>
        void Report(string rootPath, CancellationToken cancellationToken, SimpleProgressCallback progressCallback = null);

        /// <summary>
        /// Gets the relative progress weight of this stage.
        /// 
        /// Basically it should be equal to the number of stages in a composite stage, and 1.0 otherwise.
        /// </summary>
        double Weight { get; }

        /// <summary>
        /// Gets the user-friendly name of the action this reported performs.
        /// </summary>
        string Name { get; }
    }
}