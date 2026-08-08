namespace AppliedCrypto
{
    internal class CryptoModel
    {
        public string PlainText { get; set; } = string.Empty;
        public string Passphrase { get; set; } = string.Empty;
        public string EncryptedText { get; set; } = string.Empty;
        public string DecryptedText { get; set; } = string.Empty;
    }
}
