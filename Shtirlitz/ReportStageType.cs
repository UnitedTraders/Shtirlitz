using Shtirlitz.Archiver;
using Shtirlitz.Reporter;

namespace Shtirlitz
{
    /// <summary>
    /// Describes all the available report generation stages.
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
        /// Stage which consists of multiple inner stages.
        /// </summary>
        Composite
    }
}