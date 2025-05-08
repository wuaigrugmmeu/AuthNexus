using AuthNexus.Domain.Services;
using System.Security.Cryptography;

namespace AuthNexus.Infrastructure.Authentication
{
    /// <summary>
    /// 密码哈希服务实现
    /// </summary>
    public class PasswordHashingService : IPasswordHashingService
    {
        private const int SaltSize = 16; // 128 bit
        private const int KeySize = 32; // 256 bit
        private const int Iterations = 10000;
        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;
        private const char Delimiter = ':';

        /// <summary>
        /// 对密码进行哈希处理
        /// </summary>
        public string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithm,
                KeySize);

            return string.Join(
                Delimiter,
                Convert.ToBase64String(hash),
                Convert.ToBase64String(salt),
                Iterations,
                HashAlgorithm);
        }

        /// <summary>
        /// 验证密码是否匹配
        /// </summary>
        public bool VerifyPassword(string password, string passwordHash)
        {
            var elements = passwordHash.Split(Delimiter);
            if (elements.Length != 4)
            {
                return false;
            }

            var hash = Convert.FromBase64String(elements[0]);
            var salt = Convert.FromBase64String(elements[1]);
            var iterations = int.Parse(elements[2]);
            var algorithm = new HashAlgorithmName(elements[3]);

            var inputHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                algorithm,
                hash.Length);

            return CryptographicOperations.FixedTimeEquals(hash, inputHash);
        }
    }
}