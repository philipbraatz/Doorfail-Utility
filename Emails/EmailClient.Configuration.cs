using MimeKit;

namespace Doorfail.Core.Email;
public struct EmailConfiguration
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string From { get; set; }
    public string Subject { get; set; }
    public string FromName { get; set; }
    public string FromEmail { get; set; }

    public MailboxAddress FromAddress => new(FromName, FromEmail);
}
