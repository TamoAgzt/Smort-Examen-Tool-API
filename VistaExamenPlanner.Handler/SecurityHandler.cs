using System.Security.Cryptography;
using System.Text;
using Bcrypt = BCrypt.Net.BCrypt;

namespace VistaExamenPlanner.Handler
{
    public static class SecurityHandler
    {
        public static string BcrypyBasicEncryption(string password)
        {
           return Bcrypt.EnhancedHashPassword(password, 10);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            return Bcrypt.Verify(password, hash, true);
        }

        public static string AESBasiccEncryption(string text, byte[] Key, byte[] IV)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(text);
                        }
                        byte[] Encrypted = msEncrypt.ToArray();
                        return Encoding.UTF8.GetString(Encrypted);
                    }
                }
            }
        }

        public static string SHA256BasiccEncryption(string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hashedPassword = sha.ComputeHash(passwordBytes);
                return Encoding.UTF8.GetString(hashedPassword);
            }
        }
    }
}
