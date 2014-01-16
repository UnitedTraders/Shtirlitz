using System.Threading;
//using System.Web;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Shtirlitz.Common;

namespace Shtirlitz.Sender
{
    /// <summary>
    /// Sends email via SMTP
    /// </summary>
    public class SmtpMailer : ISender
    {
        private SmtpClient smtp;
        private MailMessage message;
        private MailInfo mailInfo;

        public SmtpMailer(MailInfo mailInfo)
        {
            this.mailInfo = mailInfo;
        }

        public void Send(string archiveFilename, CancellationToken cancellationToken, SimpleProgressCallback progressCallback = null)
        {
            InitializeSmtp();
            CreateMessage();
            AttachFile(archiveFilename);
            smtp.Send(message);
            message.Dispose();
        }

        /// <summary>
        /// Log in to the SMTP server
        /// </summary>
        private void InitializeSmtp()
        {
            smtp = new SmtpClient(mailInfo.Host, mailInfo.Port);
            smtp.Credentials = new NetworkCredential(mailInfo.Login, mailInfo.Password);
        }

        /// <summary>
        /// Create a message
        /// </summary>
        private void CreateMessage()
        {
            message = new MailMessage();
            message.From = new MailAddress(mailInfo.Login);
            message.To.Add(new MailAddress(mailInfo.EmailTo));
            message.Subject = mailInfo.Subject;
            message.Body = mailInfo.Body;
        }

        /// <summary>
        /// Attach a file
        /// </summary>
        private void AttachFile(string attachFileName)
        {
            Attachment attach = new Attachment(attachFileName, MediaTypeNames.Application.Octet);
            // Adding information to a file
            ContentDisposition disposition = attach.ContentDisposition;
            disposition.CreationDate = System.IO.File.GetCreationTime(attachFileName);
            disposition.ModificationDate = System.IO.File.GetLastWriteTime(attachFileName);
            disposition.ReadDate = System.IO.File.GetLastAccessTime(attachFileName);
            message.Attachments.Add(attach);
        }
    }
}
