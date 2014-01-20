using System;

namespace Shtirlitz.Reporter.TimeReporter
{
    public interface ITimeProvider
    {
        DateTimeOffset GetTime();
    }
}
