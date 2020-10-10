using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProject.AuthorizationAuthentication
{
    public class AuthOptions
    {
        public const string Issuer = "WebApiAuthServer";
        public const string Audience = "WebApiAuthClient";
        public const int Lifetime = 1;

        private const string Key = "TestEncryptingKey";

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }
    }
}
