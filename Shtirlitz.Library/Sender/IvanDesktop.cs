using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shtirlitz.Sender
{
    public class IvanDesktop:ISender
    {
        public void Send(string archiveFilename, System.Threading.CancellationToken cancellationToken, Common.SimpleProgressCallback progressCallback = null)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "//"+DateTime.Now.Ticks.ToString()+".zip";
            File.Create(path);
            File.Copy(archiveFilename, path);
        }
    }
}
