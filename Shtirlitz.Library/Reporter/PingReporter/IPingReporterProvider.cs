namespace Shtirlitz.Reporter.PingReporter
{
    public interface IPingReporterProvider
    {
        bool Next();
        PingResult Current { get; }
        double Weight { get; }
    }
}