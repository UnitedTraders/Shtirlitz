using System.Diagnostics;
using System.IO;

namespace Shtirlitz.Reporter
{
    public static class ReporterUtils
    {
        public static void DumpOutputToFile(string program, string arguments, string filename, bool append = false)
        {
            using (Process process = new Process())
            {
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = program;
                process.StartInfo.Arguments = arguments;

                process.Start();

                using (Stream fileStream = File.Open(filename, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    process.StandardOutput.BaseStream.CopyTo(fileStream);
                }

                // since we were copying the stream, the process should have exited by now
                process.WaitForExit();
            }
        }
    }
}