using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Nanocode.Net.SMTP
{
    public class SmtpClient : System.Net.Mail.SmtpClient
    {
        private MailAddress _from { get; set; }

        public SmtpClient(SmtpSender smtpSender) : base()
        {
            this.SetSender(smtpSender);
        }

        public SmtpClient(string host, int port, bool enableSsl, NetworkCredential credentials) : base()
        {
            this.Host = host;
            this.Port = port; ;
            this.EnableSsl = enableSsl;
            this.UseDefaultCredentials = false;
            this.Credentials = credentials;
        }

        public void SetSender(SmtpSender smtpSender)
        {
            if (smtpSender != null)
            {
                this._from = smtpSender.From;
                this.Host = smtpSender.Host;
                this.Port = smtpSender.Port;
                this.UseDefaultCredentials = false;
                this.Credentials = smtpSender.Credentials;
                this.EnableSsl = smtpSender.EnableSsl;
            }
        }

        public void Send(string recipient, string subject, string body)
        {
            var message = new MailMessage(this._from, new MailAddress(recipient))
            {
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Subject = subject,
                Body = body
            };

            this.Send(message);
        }

        public Task SendAsync(string recipient, string subject, string body)
        {
            var message = new MailMessage(this._from, new MailAddress(recipient))
            {
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Subject = subject,
                Body = body
            };
            return this.SendMailAsync(message);
        }
    }

    public class SmtpSender
    {
        public NetworkCredential Credentials { get; set; }
        public MailAddress From { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
    }
}
