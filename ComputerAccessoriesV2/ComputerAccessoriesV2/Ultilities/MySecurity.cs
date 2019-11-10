using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.Ultilities
{
    public class MySecurity
    {
        public static string EncryptPassword(string password)
        {
            SHA256 sha = SHA256.Create();
            byte[] rs = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(rs);
        }

        public static string DecryptPassword(string password)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            SHA256Managed sha = new SHA256Managed();
            byte[] rs = sha.ComputeHash(encoder.GetBytes(password));
            return Convert.ToBase64String(rs);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string DecryptSHA256(string data)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            SHA256Managed sha = new SHA256Managed();
            byte[] rs = sha.ComputeHash(encoder.GetBytes(data));
            return Convert.ToBase64String(rs);
        }

        public static string EncryptSHA256(string data)
        {
            System.Security.Cryptography.SHA256 sha = System.Security.Cryptography.SHA256.Create();
            byte[] rs = sha.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(rs);
        }
    }
}
