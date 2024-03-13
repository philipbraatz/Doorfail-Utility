using MimeKit;

namespace Doorfail.Core.Email;

public class EmailClient(EmailConfiguration config) :IEmailClient
{
    private readonly MailKit.Net.Smtp.SmtpClient Client = new();
    private EmailConfiguration Configuration { get; set; } = config;

    public async Task<string> SendEmail(string fileName, MailboxAddress toAddress, IDictionary<string, string>? parameters = null)
    {
        string html = File.ReadAllText(fileName);

        Client.Connect(Configuration.Host, Configuration.Port);
        Client.Authenticate(Configuration.Username, Configuration.Password);

        MimeMessage email = CreateEmails(html, toAddress, parameters);
        var result = await Client.SendAsync(email);
        await Client.DisconnectAsync(true);

        return result;
    }

    public async Task<string> SendEmails(string fileName, IEnumerable<MailboxAddress> toAddresses, IDictionary<string, string>? parameters = null)
    {
        string html = File.ReadAllText(fileName);

        Client.Connect(Configuration.Host, Configuration.Port);
        Client.Authenticate(Configuration.Username, Configuration.Password);

        MimeMessage email = CreateEmails(html, toAddresses.First(), parameters);
        var result = await Client.SendAsync(email, Configuration.FromAddress, toAddresses);
        await Client.DisconnectAsync(true);

        return result;
    }

    private MimeMessage CreateEmails(string html, MailboxAddress toAddress, IDictionary<string, string> parameters)
    {
        html = PopulateTemplate(html, parameters);

        return new(
            from: [new MailboxAddress(Configuration.Username, "!active@email.com")],
            to: [toAddress],
            subject: "It works :)",
            body: new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = html
            }
        );
    }

    public static string PopulateTemplate(string template, IDictionary<string, string> replacements)
    {
        // Order by length to prevent replacing substrings
        foreach(var replacement in replacements.OrderByDescending(o => o.Key.Length))
        {
            template = template.Replace($"@{replacement.Key}", replacement.Value);
        }

        return template;
    }
}