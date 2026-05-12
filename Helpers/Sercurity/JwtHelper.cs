using System.Security.Cryptography;

namespace CMVC.Helpers.Sercurity
{
    public class JwtHelper
    {
        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
