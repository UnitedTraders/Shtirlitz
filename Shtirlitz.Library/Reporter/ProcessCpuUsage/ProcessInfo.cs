namespace Shtirlitz.Reporter.ProcessCpuUsage
{
    public class ProcessInfo
    {
        public ProcessInfo(int processId, string processName)
        {
            ProcessId = processId;
            ProcessName = processName;
        }

        public int ProcessId { get; set; }
        public string ProcessName { get; set; }
        public string Cpu { get; set; }
    }
}
