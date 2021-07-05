using System.Security.Cryptography;
using System.Text;

namespace CodingChainApi.Infrastructure.Common
{
    public static class StringExtensions
    {
        public static string GetHash( this string input, HashAlgorithm hashAlgorithm)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (var i = 0; i < data.Length; i++) sBuilder.Append(data[i].ToString(""));

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}