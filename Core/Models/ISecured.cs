using System.Security.Cryptography;
using System.Text;

namespace Doorfail.Core.Entities
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
            using SHA256 sha256 = SHA256.Create();
            byte[] secretBytes = Encoding.UTF8.GetBytes(secret + salt);
            byte[] hashBytes = sha256.ComputeHash(secretBytes);
            string hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

            return new PrivateEntity
            {
                Hash = hash
            };
        }

        public static bool Verify(ISecured secured, string secret, string salt)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] secretBytes = Encoding.UTF8.GetBytes(secret + salt);
            byte[] hashBytes = sha256.ComputeHash(secretBytes);
            string hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

            return secured.Hash == hash;
        }
    }
}