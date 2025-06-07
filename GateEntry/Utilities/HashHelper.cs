using System.Security.Cryptography;
using System.Text;

namespace GateEntry.Utilities;

public class HashHelper
{
    public static string ComputeSha256Hash(string rawData)
    {
        using (var sha256 = SHA256.Create())
        {
            // Convert string to byte array
            var bytes = Encoding.UTF8.GetBytes(rawData);

            // Compute hash
            var hashBytes = sha256.ComputeHash(bytes);

            // Convert hash to hex string
            var builder = new StringBuilder();
            foreach (var b in hashBytes)
                builder.Append(b.ToString("x2")); // lowercase hex

            return builder.ToString();
        }
    }
}