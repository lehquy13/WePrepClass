using System.Net;
using System.Net.Mail;
using FluentEmail.Core;
using FluentEmail.Smtp;
using Matt.SharedKernel.Domain.Interfaces.Emails;
using Microsoft.Extensions.Options;
using WePrepClass.Infrastructure.Models;

namespace WePrepClass.Infrastructure.EmailServices;

internal class EmailSender(IOptions<EmailSettingNames> options) : IEmailSender
{
    private readonly EmailSettingNames _emailSettingNames = options.Value;

    public async Task Send(string email, string subject, string message)
    {
        var senderMail = _emailSettingNames.Email;
        var pw = _emailSettingNames.Password;

        var client = new SmtpClient(_emailSettingNames.SmtpClient, _emailSettingNames.Port)
        {
            EnableSsl = _emailSettingNames.EnableSsl
        };
        client.UseDefaultCredentials = _emailSettingNames.UseDefaultCredentials;
        client.Credentials = new NetworkCredential(senderMail, pw);


        Email.DefaultSender = new SmtpSender(client);

        // Should make use of this
        _ = await Email
            .From(senderMail).To(email)
            .Subject(subject).Body(message)
            .SendAsync();
    }

    public async Task SendHtml(string email, string subject, string template)
    {
        var senderMail = _emailSettingNames.Email;
        var pw = _emailSettingNames.Password;

        var client = new SmtpClient(_emailSettingNames.SmtpClient, _emailSettingNames.Port)
        {
            EnableSsl = _emailSettingNames.EnableSsl
        };
        client.UseDefaultCredentials = _emailSettingNames.UseDefaultCredentials;
        client.Credentials = new NetworkCredential(senderMail, pw);


        Email.DefaultSender = new SmtpSender(client);

        _ = await Email
            .From(senderMail).To(email)
            .Subject(subject).Body(template, true)
            .SendAsync();
    }
}