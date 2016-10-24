using System;
using System.IO;
using System.Threading;
using Shtirlitz.Common;

namespace Shtirlitz.Reporter.CurrentTimeReporter
{
    public class CurrentTimeReporter : IReporter
    {
        public const string DATE_FORMAT = "yyyy.MM.dd hh.mm.ss zzz";

        private readonly DateTime? _dateTime;

        public CurrentTimeReporter()
        {
        }

        public CurrentTimeReporter(DateTime dateTime)
        {
            _dateTime = dateTime;
        }

        #region IReporter members

        public void Report(string rootPath, CancellationToken cancellationToken, SimpleProgressCallback progressCallback = null)
        {
            var dateTime = _dateTime ?? DateTime.Now;

            var path = Path.Combine(rootPath, GetFileName(dateTime));

            if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);

            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                File.Create(path).Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Filed to create file {path}", ex);
            }

            progressCallback?.Invoke(Weight);
        }
        
        public double Weight => 1d;
        public string Name => "CurrentTimeProvider";

        #endregion

        public string GetFileName(DateTime dateTime)
        {
            return dateTime.ToString(DATE_FORMAT).Replace(":", ".") + ".txt";
        }
    }
}