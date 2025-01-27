using System;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void EnviarConvitePorEmail(string emailDestino, string token)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:SenderEmail"] ?? throw new InvalidOperationException("SenderEmail não está configurado.")));
        email.To.Add(MailboxAddress.Parse(emailDestino ?? throw new ArgumentNullException(nameof(emailDestino))));
        email.Subject = "Convite para acessar o sistema";


        var baseUrl = _configuration["BaseUrl"];
        var builder = new BodyBuilder
        {
            TextBody = $"Você foi convidado para acessar o sistema. Clique no link para aceitar o convite: " +
                       $"{baseUrl}/AceitarConvite?token={token}"
        };

        email.Body = builder.ToMessageBody();

        var smtpPortConfig = _configuration["EmailSettings:SmtpPort"];
        if (!int.TryParse(smtpPortConfig, out int smtpPort))
        {
            throw new InvalidOperationException("SmtpPort não está configurado corretamente.");
        }

        using (var client = new SmtpClient())
        {
            client.Connect(_configuration["EmailSettings:SmtpServer"] ?? throw new InvalidOperationException("SmtpServer não está configurado."),
                           smtpPort,
                           MailKit.Security.SecureSocketOptions.StartTls);

            client.Authenticate(
                _configuration["EmailSettings:SenderEmail"] ?? throw new InvalidOperationException("SenderEmail não está configurado."),
                _configuration["EmailSettings:SenderPassword"] ?? throw new InvalidOperationException("SenderPassword não está configurado."));
            client.Send(email);  
            client.Disconnect(true);
        }
    }
}