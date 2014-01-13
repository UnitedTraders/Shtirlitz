using System.IO;
using System.Threading;
using Shtirlitz.Common;

namespace Shtirlitz.Reporter
{
    public abstract class MultiFileReporter : IReporter
    {
        private readonly string pathSuffix;

        protected MultiFileReporter(string pathSuffix)
        {
            this.pathSuffix = pathSuffix;
        }

        public void Report(string rootPath, CancellationToken cancellationToken, SimpleProgressCallback progressCallback = null)
        {
            // create report subdirectory
            string newPath = Path.Combine(rootPath, pathSuffix);
            Directory.CreateDirectory(newPath);

            // run reporting code
            ReportInternal(newPath, cancellationToken, progressCallback);
        }

        /// <summary>
        /// Implement reporting logic in this method.
        /// </summary>
        /// <param name="path">fill directory path including path suffix</param>
        /// <param name="cancellationToken">cancellation token for the operation</param>
        /// <param name="progressCallback">callback to execute for progress reports</param>
        protected abstract void ReportInternal(string path, CancellationToken cancellationToken, SimpleProgressCallback progressCallback = null);

        public abstract double Weight { get; }

        public abstract string Name { get; }
    }
}