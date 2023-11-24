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
using VistaExamenPlanner.Objecten;

namespace VistaExamenPlanner.Handler
{
    public static class JwtTokenHandler
    {
        private const string TokenSecret = "IWANTTOSETHERAINBOWHIGHINTHESKYIWANTTOSOYOUANDMEONABIRDFLYAWAY";

        public static String GenerateToken (int UserRol, int Id)
        {
           JwtSecurityTokenHandler jwtTokenHandler = new JwtSecurityTokenHandler ();
            RandomNumberGenerator random = RandomNumberGenerator.Create();

            byte[] key = Encoding.UTF8.GetBytes(TokenSecret);

            var claims = new List<Claim>
            {
                new("TimeCreated", DateTime.Now.ToString()),
                new("Rol", UserRol.ToString()),
                new("Id", Id.ToString()),
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
