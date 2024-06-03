using MailKit.Net.Smtp;
using MimeKit;
using SeekFilesCompare.environment;
using SeekFilesCompare.multiThreads;
using SeekFilesCompare.timer;

namespace SeekFilesCompare.mail
{
    public static class SendMail
    {
        public static void SendAMail()
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(Settings.Name, Settings.UserName));
            email.To.Add(new MailboxAddress(Settings.Name, Settings.UserName));

            email.Subject = "SeekFiles";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)

            {
                Text = $"<b>Un petit message pour dire que le hashing pour : <br> {SeekFiles.RootFileSrc} est finis. <br> </b>" +
                       $"L'execution a durée : <br> {TimerSeek.GetElapsedTime()}"
            };

            using (var smtp = new SmtpClient())
            {
                smtp.Connect(Settings.Server, Settings.Port, false);

                smtp.Authenticate(Settings.UserName, Settings.AppPassword);

                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }
    }
}
