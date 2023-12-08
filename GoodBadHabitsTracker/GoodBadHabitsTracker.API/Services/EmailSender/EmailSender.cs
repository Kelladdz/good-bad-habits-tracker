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

namespace GoodBadHabitsTracker.API.Services.EmailSender
{
    public class EmailSender(IOptions<MailSettings> mailSettings) : IEmailSender<ApplicationUser>
    {
        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
                using (MimeMessage emailMessage = new MimeMessage())
                {
                    MailboxAddress emailFrom = new MailboxAddress(mailSettings.Value.DisplayName, mailSettings.Value.Email);
                    emailMessage.From.Add(emailFrom);
                    MailboxAddress emailTo = new MailboxAddress(user.UserName, user.Email);
                    emailMessage.To.Add(emailTo);

                    emailMessage.Subject = "Welcome To GoodBadHabitsTracker.";

                    BodyBuilder emailBodyBuilder = new BodyBuilder();
                    emailBodyBuilder.TextBody = confirmationLink;

                    emailMessage.Body = emailBodyBuilder.ToMessageBody();
                    //this is the SmtpClient from the Mailkit.Net.Smtp namespace, not the System.Net.Mail one
                    using (SmtpClient mailClient = new SmtpClient())
                    {
                        mailClient.Connect(mailSettings.Value.Host, mailSettings.Value.Port, MailKit.Security.SecureSocketOptions.StartTls);
                        mailClient.Authenticate(mailSettings.Value.Email, mailSettings.Value.Password);
                        mailClient.Send(emailMessage);
                        mailClient.Disconnect(true);
                    }
                }

                return null;
        }

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            throw new NotImplementedException();
        }

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            throw new NotImplementedException();
        }
    }
}
