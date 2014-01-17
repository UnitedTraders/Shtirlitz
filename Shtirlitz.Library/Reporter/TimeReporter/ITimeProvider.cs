using System;

namespace Shtirlitz.Reporter
{
    public interface ITimeProvider
    {
        DateTimeOffset GetTime();
    }
}
