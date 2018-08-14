using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public static class EncryptionService
    {
        public static string GetSha256Hash(string value)
        {
            if (value == null)
            {
                return null;
            }

            using (var hashFunction = SHA256.Create())
            {
                return string.Concat(hashFunction
                    .ComputeHash(Encoding.UTF8.GetBytes(value))
                    .Select(item => item.ToString("x2")));
            }
        }
    }
}