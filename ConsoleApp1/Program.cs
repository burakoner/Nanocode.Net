using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System;

namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var smtp = new Nanocode.Net.SMTP.SmtpClient(new Nanocode.Net.SMTP.SmtpSender
            {
                From = new MailAddress("phalchatha@gmail.com", "Mezun No-Reply"),
                Host = "smtp.gmail.com",
                Port = 587,
                Credentials = new NetworkCredential("phalchatha@gmail.com", "abfmrirxjtcogxsk"),
                EnableSsl = true,
            });
            smtp.Send("Burak Öner<info@burakoner.com>", "Send", @"Hey Chandler,

I just wanted to let you know that Monica and I were going to go play some paintball, you in?

-- Joey");
            await smtp.SendAsync("Burak Öner<info@burakoner.com>", "SendAsync", @"Hey Chandler,

I just wanted to let you know that Monica and I were going to go play some paintball, you in?

-- Joey");
















            Console.WriteLine("Hello, World!");
        }
    }
}