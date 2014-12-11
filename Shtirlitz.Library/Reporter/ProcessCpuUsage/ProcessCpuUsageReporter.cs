using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Shtirlitz.Common;

namespace Shtirlitz.Reporter.ProcessCpuUsage
{
    public class ProcessCpuUsageReporter : IReporter
    {
        public const string REPORT_FILE_NAME = "ProcessCpuUsage.html";

        public void Report(string rootPath, CancellationToken cancellationToken, SimpleProgressCallback progressCallback = null)
        {
            InitCounters(cancellationToken);
            progressCallback.TryInvoke(0.33);

            CalculateCpuUsage(cancellationToken);
            progressCallback.TryInvoke(0.66);

            GenerateAndSaveReport(rootPath, cancellationToken);
            progressCallback.TryInvoke(1.0);
        }

        private readonly Dictionary<int, ProcessInfo> processInfos = new Dictionary<int, ProcessInfo>();
        private readonly List<string> errors = new List<string>();
        private readonly Dictionary<int, PerformanceCounter> counters = new Dictionary<int, PerformanceCounter>();

        private void InitCounters(CancellationToken cancellationToken)
        {
            var processes = Process.GetProcesses();
            for (int i = 0; i < processes.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var process = processes[i];
                if (process.Id == 0) continue; //Idle process

                AddCounterForAProcess(process);
            }
        }
        private void AddCounterForAProcess(Process process)
        {
            var counter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName);
            counter.NextValue();
            counters.Add(process.Id, counter);
        }
        private void CalculateCpuUsage(CancellationToken cancellationToken)
        {
            try
            {
                var processes = Process.GetProcesses();
                for (int i = 0; i < processes.Length; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var process = processes[i];
                    if (process.Id == 0) continue; //Idle process

                    try
                    {
                        if (!processInfos.ContainsKey(process.Id))
                        {
                            processInfos.Add(process.Id, new ProcessInfo(process.Id, process.ProcessName));
                        }

                        if (!counters.ContainsKey(process.Id))
                        {
                            AddCounterForAProcess(process);
                        }

                        var cpuUsage = counters[process.Id].NextValue()/Environment.ProcessorCount;
                        processInfos[process.Id].Cpu = Math.Round(cpuUsage).ToString();
                    }
                    catch (Win32Exception e)
                    {
                        errors.Add(process.ProcessName + " - " + e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                errors.Add(e.Message);
            }
        }

        private void GenerateAndSaveReport(string rootPath, CancellationToken cancellationToken)
        {
            var content = new StringBuilder("<html><body><table border=\"1\" cellspacing=\"0\" cellpadding=\"5\">");
            content.AppendLine("<tr><td><b>Id</b></td><td><b>Name</b></td><td><b>Cpu (%)</b></td></tr>");
            var processes = processInfos.Values.OrderByDescending(p => p.Cpu).ToList();
            for (int i = 0; i < processes.Count(); i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var process = processes[i];

                content.AppendLine(string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>",
                    process.ProcessId, process.ProcessName, process.Cpu));
            }
            content.AppendLine("</table></body></html>");

            string filename = Path.Combine(rootPath, REPORT_FILE_NAME);
            File.WriteAllText(filename, content.ToString());
        }

        public double Weight
        {
            get { return 1d; }
        }

        public string Name
        {
            get { return "Getting processes CPU usage"; }
        }
    }
}
