using Calmska.Application.Abstractions;

namespace Calmska.Infrastructure.Persistence.Security;

public class PasswordHasher : IPasswordHasher
{
    public SetHashResult SetHash(string password)
    {
        try
        {
            return new SetHashResult(BCrypt.Net.BCrypt.EnhancedHashPassword(password, 10), null);
        }
        catch (Exception ex)
        {
            return new SetHashResult(string.Empty, ex.Message);
        }
    }

    public VerifyPasswordResult VerifyPassword(string enteredPassword, string storedPassword)
    {
        try
        {
            return new VerifyPasswordResult(BCrypt.Net.BCrypt.EnhancedVerify(enteredPassword, storedPassword), null);
        }
        catch (Exception ex)
        {
            return new VerifyPasswordResult(false, ex.Message);
        }
    }
}