using System;

namespace Shtirlitz.Reporter
{
    public class CurrentTimeProvider : ITimeProvider
    {
        public DateTimeOffset GetTime()
        {
            return DateTimeOffset.Now;
        }
    }
}