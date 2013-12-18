namespace Shtirlitz.Common
{
    public static class ProgressCallbackUtil
    {
        public static void TryInvoke(this ProgressCallback callback, double progress, string stageName, ReportStageType stageType)
        {
            if (callback == null)
            {
                return;
            }

            callback(progress, stageName, stageType);
        }

        public static void TryInvoke(this SimpleProgressCallback callback, double progress)
        {
            if (callback == null)
            {
                return;
            }

            callback(progress);
        }
    }
}