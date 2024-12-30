namespace Calmska.Tests.ApiTests.Helper
{
    public class HashPasswordTests
    {
        [Fact]
        public void SetHash_ShouldReturnNonEmptyHash_WhenPasswordIsProvided()
        {
            string password = "securePass";
            string hashedPassword = HashPassword.SetHash(password);
            hashedPassword.Should().NotBeNullOrEmpty();
        }
        [Fact]
        public void SetHash_ShouldReturnDifferentHashes_ForDifferentPasswords()
        {
            string password1 = "Password1";
            string password2 = "Password2";

            string hash1 = HashPassword.SetHash(password1);
            string hash2 = HashPassword.SetHash(password2);

            hash1.Should().NotBe(hash2);
        }
        [Fact]
        public void VerifyPassword_ShouldReturnTrue_WhenPasswordMatchesHash()
        {
            string password = "CorrectPassword";
            string hashedPassword = HashPassword.SetHash(password);

            bool result = HashPassword.VerifyPassword(password, hashedPassword);

            result.Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalse_WhenPasswordDoesNotMatchHash()
        {
            string password = "CorrectPassword";
            string wrongPassword = "WrongPassword";
            string hashedPassword = HashPassword.SetHash(password);

            bool result = HashPassword.VerifyPassword(wrongPassword, hashedPassword);

            result.Should().BeFalse();
        }
    }
}
