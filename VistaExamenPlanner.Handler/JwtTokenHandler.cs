using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Paddings;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace VistaExamenPlanner.Handler
{
    public static class JwtTokenHandler
    {
        private const string TokenSecret = "IWANTTOSETHERAINBOWHIGHINTHESKYIWANTTOSOYOUANDMEONABIRDFLYAWAY";


        public static String GenerateToken (string id)
        {
           JwtSecurityTokenHandler jwtTokenHandler = new JwtSecurityTokenHandler ();
            RandomNumberGenerator random = RandomNumberGenerator.Create();

            byte[] key = Encoding.UTF8.GetBytes(TokenSecret);

            var claims = new List<Claim>
            {
                new("TimeCreated", DateTime.Now.ToString()),
                new("Id", id)
            };

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(8),
                Issuer = "http://localhost",
                Audience = "http://localhost",
                SigningCredentials = credentials
            };

            SecurityToken token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);
        }
    }
}
