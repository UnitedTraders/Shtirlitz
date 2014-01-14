using System;
using System.Threading;

namespace Shtirlitz
{
    public class ReportCanceledEventArgs : EventArgs
    {
        private readonly bool hasFailed;
        private readonly Exception exception;

        public ReportCanceledEventArgs(bool hasFailed, Exception exception = null)
        {
            this.hasFailed = hasFailed;
            this.exception = exception;
        }

        /// <summary>
        /// Gets whether the report generation was interrupted due to error. If it's false, then the report gereation was canceled through the <see cref="CancellationToken"/>.
        /// </summary>
        public bool HasFailed
        {
            get { return hasFailed; }
        }

        /// <summary>
        /// If the report generation failed, gets the exception that caused the operation to fail.
        /// </summary>
        public Exception Exception
        {
            get { return exception; }
        }
    }
}