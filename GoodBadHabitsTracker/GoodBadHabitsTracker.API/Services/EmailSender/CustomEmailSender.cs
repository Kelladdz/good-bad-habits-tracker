using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using GoodBadHabitsTracker.Core.Domain.IdentityModels;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MimeKit.Utils;
using System.Net.Mail;

namespace GoodBadHabitsTracker.API.Services.EmailSender
{
    public class CustomEmailSender(IOptions<MailSettings> mailSettings, IWebHostEnvironment environment) : ICustomEmailSender<ApplicationUser>, IEmailSender<ApplicationUser>
    {
        public Task SendWelcomeMessageAsync(ApplicationUser user, string email)
        {
            using (MimeMessage emailMessage = new MimeMessage())
                
            {
                MailboxAddress emailFrom = new MailboxAddress(mailSettings.Value.DisplayName, mailSettings.Value.Email);
                emailMessage.From.Add(emailFrom);
                MailboxAddress emailTo = new MailboxAddress(user.UserName, user.Email);
                emailMessage.To.Add(emailTo);

                emailMessage.Subject = "Welcome To GoodBadHabitsTracker.";

                BodyBuilder emailBodyBuilder = new BodyBuilder();
                emailBodyBuilder.HtmlBody = File.ReadAllText(environment.WebRootPath + "\\EmailBodies\\welcome.html").Replace("{userName}", user.UserName);
                emailMessage.Body = emailBodyBuilder.ToMessageBody();
                //this is the SmtpClient from the Mailkit.Net.Smtp namespace, not the System.Net.Mail one
                using (MailKit.Net.Smtp.SmtpClient mailClient = new MailKit.Net.Smtp.SmtpClient())
                {

                    mailClient.CheckCertificateRevocation = false;
                    mailClient.Connect(mailSettings.Value.Host, mailSettings.Value.Port, MailKit.Security.SecureSocketOptions.StartTls);
                    mailClient.Authenticate("goodbadhabitstracker@gmail.com", "gpig isdo ytzx shjy");
                    mailClient.Send(emailMessage);
                    mailClient.Disconnect(true);
                }
            }
            return Task.CompletedTask;
        }
        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink, string token)
        {
            using (MimeMessage emailMessage = new MimeMessage())

            {
                MailboxAddress emailFrom = new MailboxAddress(mailSettings.Value.DisplayName, mailSettings.Value.Email);
                emailMessage.From.Add(emailFrom);
                MailboxAddress emailTo = new MailboxAddress(user.UserName, user.Email);
                emailMessage.To.Add(emailTo);

                emailMessage.Subject = "Password Reset Request";

                BodyBuilder emailBodyBuilder = new BodyBuilder();
                emailBodyBuilder.HtmlBody = File.ReadAllText(environment.WebRootPath + "\\EmailBodies\\resetPassword.html").Replace("{token}", token).Replace("{userId}", user.Id.ToString());
                emailMessage.Body = emailBodyBuilder.ToMessageBody();
                //this is the SmtpClient from the Mailkit.Net.Smtp namespace, not the System.Net.Mail one
                using (MailKit.Net.Smtp.SmtpClient mailClient = new MailKit.Net.Smtp.SmtpClient())
                {

                    mailClient.CheckCertificateRevocation = false;
                    mailClient.Connect(mailSettings.Value.Host, mailSettings.Value.Port, MailKit.Security.SecureSocketOptions.StartTls);
                    mailClient.Authenticate("goodbadhabitstracker@gmail.com", "gpig isdo ytzx shjy");
                    mailClient.Send(emailMessage);
                    mailClient.Disconnect(true);
                }
            }
            return Task.CompletedTask;
        }
        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
            throw new NotImplementedException();
        }

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            throw new NotImplementedException();
        }

        Task IEmailSender<ApplicationUser>.SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            throw new NotImplementedException();
        }
    }
}
