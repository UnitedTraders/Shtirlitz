using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Shtirlitz.Archiver;
using Shtirlitz.Common;
using Shtirlitz.Reporter;

namespace Shtirlitz
{
    public class Shtirlitz : IShtirlitz
    {
        private readonly IList<IReporter> reporters;
        private readonly IArchiver archiver;

        /// <summary>
        /// Initializes Shtirlitz with the default archiver.
        /// </summary>
        public Shtirlitz(IList<IReporter> reporters)
            : this(reporters, ArchiverStage.DefaultArchiver)
        { }

        public Shtirlitz(IList<IReporter> reporters, IArchiver archiver)
        {
            this.reporters = reporters;
            this.archiver = archiver;
        }

        public Task<string> Start(CancellationToken cancellationToken, string archiveFilename = null)
        {
            string rootPath;
            return Start(cancellationToken, out rootPath, false, archiveFilename);
        }

        /// <summary>
        /// Starts the info collection process. This is the more advanced method.
        /// 
        /// You can retrieve the report generation result either through the <see cref="ReportGenerated"/> event, or by adding a continuation for the returned task.
        /// You can monitor the progress of the operation through <see cref="GenerationProgress"/> event.
        /// </summary>
        /// <param name="cancellationToken">cancellation token for the operation</param>
        /// <param name="rootPath">returns the path to the temporary directory with report files</param>
        /// <param name="cleanup">specify true to remove the temporary files after report generation, otherwise false</param>
        /// <param name="archiveFilename">set to non-null to override the filename of the archive which will be generated</param>
        /// <returns>the task that returns the archive file name</returns>
        public Task<string> Start(CancellationToken cancellationToken, out string rootPath, bool cleanup = true, string archiveFilename = null)
        {
            // generate paths here, because we need to know them for cleanup on cancel
            
            // create temporary directory
            string localRootPath = rootPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(localRootPath);

            // generate archive file name
            if (archiveFilename == null)
            {
                archiveFilename = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".zip");
            }

            // start the main task
            Task<string> mainTask = Task.Factory.StartNew(() => RunInternal(cancellationToken, localRootPath, archiveFilename, cleanup), TaskCreationOptions.LongRunning);

            // add the success continuation
            mainTask.ContinueWith(t => RaiseReportGenerated(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);

            // and the cancel continuation
            mainTask.ContinueWith(t => FullCleanup(localRootPath, archiveFilename), TaskContinuationOptions.NotOnRanToCompletion);

            return mainTask;
        }

        /// <summary>
        /// Runs the report generation.
        /// </summary>
        /// <param name="cancellationToken">cancellation token which cancels the operation</param>
        /// <param name="rootPath">path to the pre-created directory into which all the report files should be added</param>
        /// <param name="archiveFilename">set to non-null string to override the default archive file name</param>
        /// <param name="cleanup">set to false, if you want Shtirlitz to leave the raw files in place. To ease the debugging, for example.</param>
        /// <returns>file name of the created archive</returns>
        private string RunInternal(CancellationToken cancellationToken, string rootPath, string archiveFilename, bool cleanup = true)
        {
            // build stage list
            List<IStage> stages = new List<IStage>();

            // start with report generation
            foreach (IReporter reporter in reporters)
            {
                stages.Add(new ReporterStage(rootPath, reporter, cancellationToken));
            }

            // then archive all the files
            stages.Add(new ArchiverStage(rootPath, archiveFilename, archiver, cancellationToken));

            // then clean them up
            if (cleanup)
            {
                stages.Add(new CleanupStage(rootPath, cancellationToken));
            }

            // pack all the stages into the stage runner
            StageRunner runner = new StageRunner(rootPath, stages, cancellationToken, "Everything");

            // subscribe to the progress updates on a runner
            runner.Progress += RaiseGenerationProgress;

            // start report generation
            runner.Run();

            // unsubscribe from progress update on a runner
            runner.Progress -= RaiseGenerationProgress;

            return archiveFilename;
        }

        /// <summary>
        /// Removes both temporary directory and an archive file. Useful when the operation was canceled or otherwise failed.
        /// </summary>
        public static void PerformFullCleanup(string rootPath, string archiveFilename)
        {
            // remove directory
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, true);
            }

            // remove archive file
            if (File.Exists(archiveFilename))
            {
                File.Delete(archiveFilename);
            }
        }

        private void FullCleanup(string rootPath, string archiveFilename)
        {
            PerformFullCleanup(rootPath, archiveFilename);

            // notify about report generation cancellation
            RaiseReportCanceled();
        }

        public event EventHandler<GenerationProgressEventArgs> GenerationProgress;

        protected virtual void RaiseGenerationProgress(double progress, string stageName, ReportStageType stageType)
        {
            EventHandler<GenerationProgressEventArgs> handler = GenerationProgress;
            if (handler != null)
            {
                handler(this, new GenerationProgressEventArgs(progress, stageName, stageType));
            }
        }

        public event EventHandler<ReportGeneratedEventArgs> ReportGenerated;

        protected virtual void RaiseReportGenerated(string reportFilename)
        {
            EventHandler<ReportGeneratedEventArgs> handler = ReportGenerated;
            if (handler != null)
            {
                handler(this, new ReportGeneratedEventArgs(reportFilename));
            }
        }

        public event EventHandler ReportCanceled;

        protected virtual void RaiseReportCanceled()
        {
            EventHandler handler = ReportCanceled;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}