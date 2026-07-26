public static class RandomFileNameGenerator
{
    private static readonly Random _random = new Random();
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const string Numbers = "0123456789";
    private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    // Alphanumeric (letters + numbers)
    public static string GenerateAlphaNumeric(int length = 6)
    {
        var result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = Chars[_random.Next(Chars.Length)];
        }
        return new string(result);
    }

    // Numbers only
    public static string GenerateNumeric(int length = 6)
    {
        var result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = Numbers[_random.Next(Numbers.Length)];
        }
        return new string(result);
    }

    // Letters only
    public static string GenerateLetters(int length = 6)
    {
        var result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = Letters[_random.Next(Letters.Length)];
        }
        return new string(result);
    }
}