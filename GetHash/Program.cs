using System;
using BCrypt.Net;

public class PasswordSecurity
{
    // Hashes the password
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    // Verifies the password against the hashed password
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Password Security Console Application");
        Console.WriteLine("-------------------------------------");

        // Step 1: Enter and hash a password
        Console.Write("Enter a password to hash: ");
        string password = Console.ReadLine();
        string hashedPassword = PasswordSecurity.HashPassword(password);
        Console.WriteLine($"\nHashed Password: {hashedPassword}\n");

        // Step 2: Verify the password
        Console.Write("Enter the same password to verify: ");
        string verifyPassword = Console.ReadLine();
        bool isVerified = PasswordSecurity.VerifyPassword(verifyPassword, hashedPassword);
        Console.WriteLine($"Password Verified: {isVerified}");

        // Step 3: Test with an incorrect password
        Console.Write("\nEnter an incorrect password to test: ");
        string wrongPassword = Console.ReadLine();
        bool isIncorrectVerified = PasswordSecurity.VerifyPassword(wrongPassword, hashedPassword);
        Console.WriteLine($"Wrong Password Verified: {isIncorrectVerified}");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
