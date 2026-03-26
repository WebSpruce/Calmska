namespace Calmska.Domain.Filters;

public sealed class AccountFilter
{
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PasswordHashed { get; set; }

    public AccountFilter(Guid? userId, string? userName, string? email, string? passwordHashed)
    {
        UserId = userId;
        UserName = userName;
        Email = email;
        PasswordHashed = passwordHashed;
    }
}