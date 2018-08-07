using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public class EncryptionService : IEncryptionService
    {        
        public string GetSha256Hash(string value)
        {
            using (var hashFunction = SHA256.Create())
            {
                return string.Concat(hashFunction
                    .ComputeHash(Encoding.UTF8.GetBytes(value))
                    .Select(item => item.ToString("x2")));
            }
        }
    }
}