using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Stock.WebAPI.Services.Abstraction;
using Stock.WebAPI.ViewModels;

namespace Stock.WebAPI.Services
{
    public class AuthService : IAuthService
    {
        string jwtSecret;
        int jwtLifespan;
        public AuthService(string jwtSecret, int jwtLifespan)
        {
            this.jwtSecret = jwtSecret;
            this.jwtLifespan = jwtLifespan;
        }
        public AuthData GetAuthData(string id, string roleName)
        {
            var expirationTime = DateTime.UtcNow.AddSeconds(jwtLifespan);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, id),
                    new Claim(ClaimTypes.Role,roleName)
                }),
                Expires = expirationTime,
                // new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

            return new AuthData
            {
                Token = token,
                TokenExpirationTime = ((DateTimeOffset)expirationTime).ToUnixTimeSeconds(),
                Id = id,
            };
        }

        public string HashPassword(string password)
        {
            var md5 = System.Security.Cryptography.SHA1.Create();

            var buf = md5.ComputeHash(Encoding.ASCII.GetBytes(password));

            var res = Encoding.ASCII.GetString(buf);

            return res;
        }

        public bool VerifyPassword(string actualPassword, string hashedPassword)
        {
            var md5 =  System.Security.Cryptography.SHA1.Create();

            var buf = md5.ComputeHash(Encoding.ASCII.GetBytes(actualPassword));

            var res = Encoding.ASCII.GetString(buf);


            return string.Equals(res, hashedPassword);
        }
    }
}