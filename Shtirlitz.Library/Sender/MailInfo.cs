namespace Shtirlitz.Sender
{
    /// <summary>
    /// mail settings
    /// </summary>
    public class MailInfo
    {
        private string emailTo;

        public string EmailTo
        {
            get { return emailTo; }
            set { emailTo = value; }
        }

        private string subject;

        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        private string body;

        public string Body
        {
            get { return body; }
            set { body = value; }
        }

        private string host;

        public string Host
        {
            get { return host; }
            set { host = value; }
        }

        private int port;

        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        private string login;

        public string Login
        {
            get { return login; }
            set { login = value; }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; }
        }
    }
}
