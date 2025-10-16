namespace Infrastructure.Persistences.Interfaces
{
    public interface IPasswordHasher
    {
        void GeneratePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
    }
}
