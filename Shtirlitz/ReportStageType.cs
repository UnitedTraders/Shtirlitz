using Shtirlitz.Archiver;
using Shtirlitz.Reporter;

namespace Shtirlitz
{
    /// <summary>
    /// Describes all the available report generation stages.
    /// 
    /// Except for the <see cref="Composite"/> value, all the values here are show in the order the steps are executed.
    /// </summary>
    public enum ReportStageType
    {
        /// <summary>
        /// Stage where we're collecting the info about the system.
        /// 
        /// That's when all the <see cref="IReporter"/>s are running.
        /// </summary>
        CollectingInfo,
 
        /// <summary>
        /// Stage where we're archiving all the info we've collected into one archive.
        /// 
        /// That's when we call <see cref="IArchiver.Archive"/>.
        /// </summary>
        Archiving,

        /// <summary>
        /// Stage where we're cleaning the files that we don't need anymore.
        /// </summary>
        Cleanup,

        /// <summary>
        /// Stage where we're sending generated report to the developers, or otherwise processing the generated report.
        /// </summary>
        Sending,

        /// <summary>
        /// Stage which consists of multiple inner stages.
        /// </summary>
        Composite
    }
}