using Preplit.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Preplit.Services.Users.Commands
{
    public class GenerateToken
    {
        public class Command : IRequest<string>
        {
            public required User User { get; set; }
        }

        public class Handler(UserManager<User> userManager, IConfiguration config) : IRequestHandler<Command, string>
        {
            public async Task<string> Handle(Command request, CancellationToken ct)
            {
                User user = request.User;
                List<Claim> claims = [
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email!)
                ];

                var roles = await userManager.GetRolesAsync(user);
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
                var jwtKey = config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt Key not configured");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddDays(double.Parse(config["Jwt:ExpireDays"]!)),
                    SigningCredentials = creds,
                    Issuer = config["Jwt:Issuer"],
                    Audience = config["Jwt:Audience"]
                };

                return new JsonWebTokenHandler().CreateToken(tokenDescriptor);                
            }
        }
    }
}