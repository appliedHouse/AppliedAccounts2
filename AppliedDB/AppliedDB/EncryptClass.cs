namespace AppliedAccounts.Data
{
    public class EncryptClass
    {
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
}
