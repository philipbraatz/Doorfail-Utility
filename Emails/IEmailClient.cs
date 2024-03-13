using MimeKit;

namespace Doorfail.Core.Email;
public interface IEmailClient
{
    Task<string> SendEmail(string fileName, MailboxAddress toAddress, IDictionary<string, string>? parameters = null);
    Task<string> SendEmails(string fileName, IEnumerable<MailboxAddress> toAddresses, IDictionary<string, string>? parameters = null);
}