using System;
using System.Collections.Generic;
using Shtirlitz.Reporter.PingReporter;

namespace Shtirlitz.Tests.Reporter.PingReporter.Common
{
    public class PingResultCompareer : IComparer<PingResult>
    {
        public int Compare(PingResult x, PingResult y)
        {
            var result = string.Compare(x.Host, y.Host, StringComparison.Ordinal);
            if (result == 0)
                result = x.Send.CompareTo(y.Send);
            if (result == 0)
                result = x.Receive.CompareTo(y.Receive);
            if (result == 0)
                result = x.Lost.CompareTo(y.Lost);

            return result;
        }
    }
}