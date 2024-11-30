using System.Diagnostics;

namespace Calmska.Api.Helper
{
    internal class HashPassword
    {
        internal static string SetHash(string password)
        {
            try
            {
                return BCrypt.Net.BCrypt.EnhancedHashPassword(password, 10);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hash Password Error: {ex}");
                return string.Empty;
            }
        }
        internal static bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.EnhancedVerify(enteredPassword, storedPassword);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Verify Password Error: {ex}");
                return false;
            }
        }
    }
}
