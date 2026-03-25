namespace Calmska.Domain.Interfaces;

public record SetHashResult(string Hash, string? Error);
public record VerifyPasswordResult(bool Verified, string? Error);
public interface IPasswordHasher
{
    SetHashResult SetHash(string password);
    VerifyPasswordResult VerifyPassword(string enteredPassword, string storedPassword);
}