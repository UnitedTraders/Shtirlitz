using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using Shtirlitz.Common;

namespace Shtirlitz.Reporter
{
    /// <summary>
    /// Dumps WMI info 
    /// </summary>
    public class WmiDumper : MultiFileReporter
    {
        public const string PathSuffix = "wmi";

        private static readonly string[] DefaultAliases = new []
            {
                "baseboard",
                "bios",
                "bootconfig",
                "computersystem",
                "cpu",
                "csproduct",
                "desktop",
                "desktopmonitor",
                "diskdrive",
                "environment",
                "logicaldisk",
                "memorychip",
                "memphysical",
                "netclient",
                "netprotocol",
                "nic",
                "nicconfig",
                "onboarddevice",
                "os",
                "partition",
                "process",
                "temperature",
                "timezone",
                "volume"
            };

        private readonly string[] aliases;

        public WmiDumper()
            : this(DefaultAliases)
        { }

        public WmiDumper(string[] aliases)
            : base(PathSuffix)
        {
            this.aliases = aliases;
        }

        public ReadOnlyCollection<string> Aliases
        {
            get { return new ReadOnlyCollection<string>(aliases); }
        }

        protected override void ReportInternal(string path, CancellationToken cancellationToken, SimpleProgressCallback progressCallback = null)
        {
            for (int i = 0; i < aliases.Length; i++)
            {
                string alias = aliases[i];

                // create a file name for the report
                string filename = Path.Combine(path, string.Format("{0}.html", alias));

                // add some compativility code
                File.WriteAllText(filename, "<style>span { display: block; }</style>\n");

                // dump report file
                ReporterUtils.DumpOutputToFile("wmic", string.Format("{0} get /format:htable", alias), filename, true);

                // report progress
                progressCallback.TryInvoke((i+1d) / aliases.Length);
            }
        }

        public override double Weight
        {
            get { return 1d; }
        }

        public override string Name
        {
            get { return "Gathering system info"; }
        }
    }
}