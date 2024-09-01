using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Final.Application.Services.Implementations
{
    public class TokenService : ITokenService
    {
        public string GetToken(string secretKey, string audience, string issuer, User user, IList<string> roles)
        {
            //jwt
            var handler = new JwtSecurityTokenHandler();
            var privateKey = Encoding.UTF8.GetBytes(secretKey);
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(privateKey),
                SecurityAlgorithms.HmacSha256);

            var ci = new ClaimsIdentity();

            ci.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            ci.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            ci.AddClaim(new Claim(ClaimTypes.GivenName, user.Name));
            ci.AddClaim(new Claim(ClaimTypes.Surname, user.Surname));
            ci.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            ci.AddClaim(new Claim("Adress", "Baku"));//isdediyimizi bele elave edirik,key value mentigi ile

            ci.AddClaims(roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList());

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddMinutes(30),
                Subject = ci,
                Audience = audience,
                Issuer = issuer
            };

            var token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }
    }
}
