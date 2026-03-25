using Calmska.Application.Abstractions;
using Firebase.Auth;

namespace Calmska.Infrastructure.Persistence.Security;

public class FirebaseService : IFirebaseService
{
    private readonly IFirebaseAuthClient _authClient;

    public FirebaseService(IFirebaseAuthClient authClient)
    {
        _authClient = authClient;
    }

    public async Task<FirebaseResponse> CreateUserAsync(string email, string password)
    {
        try
        {
            await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);
            return new FirebaseResponse(true, null);
        }
        catch (Exception ex)
        {
            return new FirebaseResponse(false, ex.Message);
        }
    }

    public async Task<FirebaseResponse> SignInAsync(string email, string password)
    {
        try
        {
            await _authClient.SignInWithEmailAndPasswordAsync(email, password);
            return new FirebaseResponse(true, null);
        }
        catch (Exception ex)
        {
            return new FirebaseResponse(false, ex.Message);
        }
    }
}