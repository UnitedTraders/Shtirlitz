using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Shtirlitz.Common;

namespace Shtirlitz.Sender
{
    /// <summary>
    /// Implements an "<see cref="ISender"/>" that copies the archive file to the clipboard.
    /// 
    /// Beware that after doing this, you can't remove the archive file, because what's actually copied is a file's path.
    /// </summary>
    public class ClipboardCopier : ISender
    {
        private readonly bool performCut;

        /// <summary>
        /// Creates the clipboard sender.
        /// 
        /// Set the <paramref name="performCut"/> to false, if you need the file to be Copied instead of Moved when the user decides to paste it.
        /// </summary>
        public ClipboardCopier(bool performCut = true)
        {
            this.performCut = performCut;
        }

        public void Send(string archiveFilename, CancellationToken cancellationToken, SimpleProgressCallback progressCallback = null)
        {
            // create drop effect memory stream
            byte[] moveEffect = new byte[] { (byte) (performCut ? 2 : 5), 0, 0, 0 };
            MemoryStream dropEffect = new MemoryStream();
            dropEffect.Write(moveEffect, 0, moveEffect.Length);

            // create file data object
            DataObject data = new DataObject();
            data.SetFileDropList(new StringCollection { archiveFilename });
            data.SetData("Preferred DropEffect", dropEffect);

            // create STA thread that'll work with Clipboard object
            Thread copyStaThread = new Thread(() =>
                {
                    Clipboard.Clear();
                    Clipboard.SetDataObject(data, true);
                })
                {
                    Name = "Clipboard copy thread"
                };

            copyStaThread.SetApartmentState(ApartmentState.STA);

            // start the thread and wait for it to finish
            copyStaThread.Start();
            copyStaThread.Join();
        }
    }
}