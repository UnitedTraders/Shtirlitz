using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Shtirlitz.Common;

namespace Shtirlitz.Reporter.TraceRouteReporter
{
    public class MultipleTraceRouteReporter : IReporter
    {
        private readonly IMultiTracer _multiTracer;
        private readonly object _sync = new object();

        public MultipleTraceRouteReporter(IMultiTracer multiTracer)
        {
            _multiTracer = multiTracer;
        }

        #region Settings

        private int _numberOfRounds = 1;
        public int NumberOfRounds
        {
            get { return _numberOfRounds; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value");
                _numberOfRounds = value;
            }
        }


        private readonly List<string> _addresses = new List<string>();
        public List<string> Addresses
        {
            get { return _addresses; }
        }

        #endregion Settings

        private double _weight = 1d;
        public double Weight
        {
            get { return _weight; }
        }


        private string _name = "MultipleTraceRouteReporter";
        public string Name
        {
            get { return _name; }
        }
        

        public void Report(string rootPath, CancellationToken cancellationToken, SimpleProgressCallback progressCallback = null)
        {
            var filename = Path.Combine(rootPath, GetFileNameFromAddreses());
            try
            {
                _multiTracer.MaxHops = 30;
                _multiTracer.PingTimeout = 1500;
                _multiTracer.PackageSize = 64;
                _multiTracer.NumberOfRounds = 5;
                _multiTracer.Trace(Addresses, s => AddResultsToFile(filename, s), cancellationToken, progressCallback);
            }
            catch (IOException ex)
            {
                throw new IOException(string.Format("Failed to create report file \"{0}\". See the inner exceptions for details", filename), ex);
            }

            // report progress
            progressCallback.TryInvoke(1d);
        }


        private void AddResultsToFile(string fileName, string formattedResult)
        {
            lock (_sync)
            {
                using (var outputFileStream = File.AppendText(fileName))
                {
                    outputFileStream.WriteLine(formattedResult);
                    outputFileStream.Close();
                }
            }

        }

        private  string GetFileNameFromAddreses()
        {
            var s = string.Join("_",Addresses.Take(5));
            s = RemoveInvalidFilePathCharacters(s, "");
            s =  string.Format("TraceRouteReport__{0}.txt", s);
            return s;
        }

        private static string RemoveInvalidFilePathCharacters(string filename, string replaceChar)
        {
            var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(filename, replaceChar);
        }
    }
}
