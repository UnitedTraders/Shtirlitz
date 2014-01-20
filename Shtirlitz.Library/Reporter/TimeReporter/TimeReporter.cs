using Shtirlitz.Common;
using System;
using System.IO;
using System.Threading;

namespace Shtirlitz.Reporter.TimeReporter
{
    /// <summary>
    /// Report time
    /// </summary>
    public class TimeReporter : IReporter
    {
        private readonly ITimeProvider timeProvider;

        public TimeReporter()
            : this(new CurrentTimeProvider())
        { }

        public TimeReporter(ITimeProvider timeProvider)
        {
            if (timeProvider == null) throw new ArgumentNullException("timeProvider");

            this.timeProvider = timeProvider;
        }

        public double Weight
        {
            get { return 1d; }
        }

        public string Name
        {
            get { return "Getting current time"; }
        }

        public void Report(string rootPath, CancellationToken cancellationToken, SimpleProgressCallback progressCallback = null)
        {
            // get a file name for the report
            string filename = Path.Combine(rootPath, GetFileNameFromTime(timeProvider.GetTime()));

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
            return string.Format("{0:yyyy-MM-dd HH.mm.ss.ffff zzz}.txt", time)
                         .Replace(":", "."); // replaces colon in time zone offset
        }
    }
}