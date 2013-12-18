namespace Shtirlitz.Common
{
    /// <summary>
    /// Delegate through which various stages report progress to <see cref="IShtirlitz"/>.
    /// </summary>
    /// <param name="progress">how much progress was done on the scale of 0.0 to 1.0</param>
    /// <param name="stageName">name of the stage that's currently running</param>
    /// <param name="stageType">type of the stage that's currently running</param>
    public delegate void ProgressCallback(double progress, string stageName, ReportStageType stageType);
}