using System;

namespace Shtirlitz.Reporter.TimeReporter
{
    public class CurrentTimeProvider : ITimeProvider
    {
        public DateTimeOffset GetTime()
        {
            return DateTimeOffset.Now;
        }
    }
}