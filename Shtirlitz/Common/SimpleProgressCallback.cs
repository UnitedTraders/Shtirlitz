namespace Shtirlitz.Common
{
    /// <summary>
    /// Delegate through which stage executors report progress to stage wrappers.
    /// </summary>
    /// <param name="progress">how much progress was done on the scale of 0.0 to 1.0</param>
    public delegate void SimpleProgressCallback(double progress);
}