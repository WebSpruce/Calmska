namespace Calmska.Domain.Entities
{
    public class Account
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHashed { get; set; } = string.Empty;
        public Account()
        {
            UserId = Guid.NewGuid();
        }
    }
}
