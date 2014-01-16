using System.Collections.Generic;
using Shtirlitz.Sender;
using Xunit;

namespace Shtirlitz.Tests
{
    public class SmtpMailerTests : ShtirlitzBaseTestClass
    {
        private static MailInfo mailInfo = new MailInfo();

        public SmtpMailerTests()
            : this(new SmtpMailer(mailInfo))
        {
            mailInfo.Subject = "Hi!";
            mailInfo.Body = "bla bla bla";
            mailInfo.EmailTo = "test@gmail.com";
            mailInfo.Host = "smtp.mail.ru";
            mailInfo.Port = 25;
            mailInfo.Login = "test@mail.ru";
            mailInfo.Password = "*****";

        }

        public SmtpMailerTests(SmtpMailer smtpMailer)
            : base (null, new List<ISender> { smtpMailer })
        { }

        [Fact]
        public void SendTheArchiveFile()
        {
            // act and assert
            Assert.DoesNotThrow(() => RunSynchronously(true));
        }
    }
}
