using MimeKit;

namespace Doorfail.Email;

public interface IEmailClient
{
    Task<(string, TimeSpan)> SendEmail(string fileName, string subject, MailboxAddress toAddress, IDictionary<string, string> parameters = null);

    Task<(string, TimeSpan)> SendEmails(string fileName, string subject, IEnumerable<MailboxAddress> toAddresses, IDictionary<string, string> parameters = null);
}