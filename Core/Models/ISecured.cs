using System.Security.Cryptography;
using System.Text;

namespace Doorfail.Core.Models
{
    public interface ISecured
    {
        public string Hash { get; set; }
    }

    public class PrivateEntity :ISecured
    {
        public string Hash { get; set; }

        // Encrypt the string with salt using a secure algorithm
        public static ISecured Encrypt(string secret, string salt)
        {
            using var sha256 = SHA256.Create();
            var secretBytes = Encoding.UTF8.GetBytes(secret + salt);
            var hashBytes = sha256.ComputeHash(secretBytes);
            var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

            return new PrivateEntity
            {
                Hash = hash
            };
        }

        public static bool Verify(ISecured secured, string secret, string salt)
        {
            using var sha256 = SHA256.Create();
            var secretBytes = Encoding.UTF8.GetBytes(secret + salt);
            var hashBytes = sha256.ComputeHash(secretBytes);
            var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

            return secured.Hash == hash;
        }
    }
}