using System;

namespace Shtirlitz
{
    public class ReportGeneratedEventArgs : EventArgs
    {
        private readonly string filename;

        public ReportGeneratedEventArgs(string filename)
        {
            this.filename = filename;
        }

        /// <summary>
        /// Gets the file name of the file with the report.
        /// </summary>
        public string Filename
        {
            get { return filename; }
        }
    }
}