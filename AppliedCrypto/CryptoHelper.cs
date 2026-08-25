using System.Security.Cryptography;

namespace AppliedCrypto
{
    public static class CryptoHelper
    {
        private const int Iterations = 10000;
        private const int KeySize = 256;
        private const int BlockSize = 128;

        /// <summary>
        /// Encrypts plain text using a password-derived key.
        /// </summary>
        /// <param name="plainText">Text to encrypt</param>
        /// <param name="passphrase">Secret passphrase (must be kept safe)</param>
        /// <returns>Base64-encoded cipher text</returns>
        public static string MyEncrypt(string plainText, string passphrase)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));
            if (string.IsNullOrEmpty(passphrase))
                throw new ArgumentNullException(nameof(passphrase));

            // Generate a random salt and IV
            byte[] salt = new byte[16];
            byte[] iv = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
                rng.GetBytes(iv);
            }

            // Derive a key from the passphrase and salt
            using (var derive = new Rfc2898DeriveBytes(passphrase, salt, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] key = derive.GetBytes(KeySize / 8);

                using (var aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var ms = new MemoryStream())
                    {
                        // Write salt and IV at the beginning (needed for decryption)
                        ms.Write(salt, 0, salt.Length);
                        ms.Write(iv, 0, iv.Length);

                        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }

                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// Decrypts a Base64-encoded cipher text using the same passphrase.
        /// </summary>
        /// <param name="cipherText">Base64 string from MyEncrypt</param>
        /// <param name="passphrase">Secret passphrase used for encryption</param>
        /// <returns>Decrypted plain text</returns>
        public static string MyDecrypt(string cipherText, string passphrase)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException(nameof(cipherText));
            if (string.IsNullOrEmpty(passphrase))
                throw new ArgumentNullException(nameof(passphrase));

            byte[] fullCipher = Convert.FromBase64String(cipherText);

            // Extract salt and IV from the beginning
            byte[] salt = new byte[16];
            byte[] iv = new byte[16];
            Array.Copy(fullCipher, 0, salt, 0, salt.Length);
            Array.Copy(fullCipher, salt.Length, iv, 0, iv.Length);

            // Derive the same key
            using (var derive = new Rfc2898DeriveBytes(passphrase, salt, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] key = derive.GetBytes(KeySize / 8);

                using (var aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var ms = new MemoryStream(fullCipher, salt.Length + iv.Length, fullCipher.Length - salt.Length - iv.Length))
                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (var sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
}