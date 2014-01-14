using System;
using System.Threading;

namespace Shtirlitz
{
    public class ReportCanceledEventArgs : EventArgs
    {
        private readonly bool hasFailed;

        public ReportCanceledEventArgs(bool hasFailed)
        {
            this.hasFailed = hasFailed;
        }

        /// <summary>
        /// Gets whether the report generation was interrupted due to error. If it's false, then the report gereation was canceled through the <see cref="CancellationToken"/>.
        /// </summary>
        public bool HasFailed
        {
            get { return hasFailed; }
        }
    }
}