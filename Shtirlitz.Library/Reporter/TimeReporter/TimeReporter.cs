using Shtirlitz.Common;
using System;
using System.IO;
using System.Threading;

namespace Shtirlitz.Reporter
{
    /// <summary>
    /// Report time
    /// </summary>
    public class TimeReporter : IReporter
    {
        ITimeProvider _timeProvider;

        #region Constructors
        public TimeReporter()
            : this(new CurrentTimeProvider())
        {
        }

        public TimeReporter(ITimeProvider timeProvider)
        {
            if (timeProvider == null)
                throw new ArgumentNullException("timeProvider");

            _timeProvider = timeProvider;
        }
        #endregion

        public double Weight
        {
            get { return 1d; }
        }

        public string Name
        {
            get { return "Gets system time"; }
        }

        public void Report(string rootPath, CancellationToken cancellationToken, SimpleProgressCallback progressCallback = null)
        {
            // get a file name for the report
            string filename = Path.Combine(rootPath, GetFileNameFromTime(_timeProvider.GetTime()));

            // create report file
            try
            {
                File.CreateText(filename).Close();
            }
            catch (IOException ex)
            {
                throw new IOException(string.Format("Failed to create report file \"{0}\". See the inner exceptions for details", filename), ex);
            }

            // report progress
            progressCallback.TryInvoke(1d);
        }

        public string GetFileNameFromTime(DateTimeOffset time)
        {
            return string.Format("{0}.txt", time).Replace(":", ".");
        }
    }
}