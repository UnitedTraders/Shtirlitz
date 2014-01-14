using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shtirlitz
{
    public interface IShtirlitz
    {
        /// <summary>
        /// Starts the info collection process.
        /// 
        /// You can retrieve the report generation result either through the <see cref="ReportGenerated"/> event, or by adding a continuation for the returned task.
        /// You can monitor the progress of the operation through <see cref="GenerationProgress"/> event.
        /// </summary>
        /// <param name="cancellationToken">cancellation token for the operation</param>
        /// <param name="archiveFilename">set to non-null to override the filename of the archive which will be generated</param>
        /// <returns>the task that returns the archive file name</returns>
        Task<string> Start(CancellationToken cancellationToken, string archiveFilename = null);

        /// <summary>
        /// Event is raised periodically to report the progress of the report generation.
        /// </summary>
        event EventHandler<GenerationProgressEventArgs> GenerationProgress;

        /// <summary>
        /// Event is raised when the report has finished generating and is ready for sending.
        /// </summary>
        event EventHandler<ReportGeneratedEventArgs> ReportGenerated;

        /// <summary>
        /// Event is raised when the report generation is canceled by a supplied token.
        /// 
        /// Also, raised if report generation was interrupted due to an error.
        /// </summary>
        event EventHandler<ReportCanceledEventArgs> ReportCanceled;
    }
}