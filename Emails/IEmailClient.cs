using MimeKit;

namespace Doorfail.Core.Email;
public interface IEmailClient
{
    Task<(string,TimeSpan)> SendEmail(string fileName, MailboxAddress toAddress, IDictionary<string, string>? parameters = null);
    Task<(string, TimeSpan)> SendEmails(string fileName, IEnumerable<MailboxAddress> toAddresses, IDictionary<string, string>? parameters = null);
}