namespace Calmska.Domain.Interfaces;

public record FirebaseResponse(bool IsSuccess, string? Error);
public interface IFirebaseService
{
    Task<FirebaseResponse> CreateUserAsync(string email, string password);
    Task<FirebaseResponse> SignInAsync(string email, string password);
}