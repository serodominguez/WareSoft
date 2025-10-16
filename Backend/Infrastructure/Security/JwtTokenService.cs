using Domain.Entities;
using Infrastructure.Persistences.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Security
{
    public class JwtTokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(Users user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.PK_USER.ToString()),
                new Claim(ClaimTypes.Name, user.USER_NAME!),
                new Claim(ClaimTypes.Role, user.Roles!.ROLE_NAME!),

                new Claim("pk_user",user.PK_USER.ToString()),
                new Claim("user_name", user.USER_NAME!),
                new Claim("role", user.Roles.ROLE_NAME!),
                new Claim("store_name", user.Stores!.STORE_NAME!),
                new Claim("pk_store", user.PK_STORE.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(300),
                notBefore: DateTime.Now,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
