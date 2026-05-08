using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EnterpriseCommerce.Infrastructure.Services
{
    public static class RefreshTokenGenerator
    {
        public static string Generate()
        {
            var randomBytes = new byte[64];

            using var rng =
                RandomNumberGenerator.Create();

            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }
    }
}
