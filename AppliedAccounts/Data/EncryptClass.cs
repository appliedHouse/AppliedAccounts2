using System.Security.Cryptography;
using System.Text;

namespace AppliedAccounts.Data
{
    public class EncryptClass
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("AppliedAccounts1"); // Must be 16 bytes for AES-128
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("AppliedAccounts2");  // Must be 16 bytes

        public static string Encrypt(string plaintext)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var writer = new StreamWriter(cs))
                    {
                        writer.Write(plaintext);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string ciphertext)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(Convert.FromBase64String(ciphertext)))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var reader = new StreamReader(cs))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }


    #region Password Hasher
    public class PasswordHasher
    {
        public string Password { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;

        public PasswordHasher(string password)
        {
            Password = password;
            Hash = HashPassword();
        }


        public string HashPassword()
        {
            return BCrypt.Net.BCrypt.HashPassword(Password);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
    #endregion

    #region File Excryption
    public class FileEncryption
    {
        // Key must be 16, 24, or 32 bytes long
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("Your16ByteKey123");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("Your16ByteIV1234");

        public static void EncryptFile(string inputFilePath, string outputFilePath)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var inputFileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
                using (var outputFileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                using (var cryptoStream = new CryptoStream(outputFileStream, encryptor, CryptoStreamMode.Write))
                {
                    inputFileStream.CopyTo(cryptoStream);
                }
            }
        }
    }

    public class FileDecryption
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("Your16ByteKey123");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("Your16ByteIV1234");

        public static void DecryptFile(string inputFilePath, string outputFilePath)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var inputFileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
                using (var outputFileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                using (var cryptoStream = new CryptoStream(inputFileStream, decryptor, CryptoStreamMode.Read))
                {
                    cryptoStream.CopyTo(outputFileStream);
                }
            }
        }
    }
    #endregion
}
