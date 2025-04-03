using Blazored.SessionStorage;
using BusinessObject.Entities;
using DataAccess.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.Implement
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly JwtAuthenticationStateProvider _authStateProvider;
        private readonly IMemberRepository _memeberRepository;
        private readonly IConfiguration _configuration;
        public AuthenticationService(JwtAuthenticationStateProvider authStateProvider, IMemberRepository memeberRepository, IConfiguration configuration)
        {
            _authStateProvider = authStateProvider;
            _configuration = configuration;
            _memeberRepository = memeberRepository;

        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var account = await _memeberRepository.GetMembersByEmailAddress(email);
            if (account == null)
            {
                return false;
            }

            if (!VerifyPassword(password, account.Password))
            {
                return false;
            }
            string token = GenerateJwtToken(account);
            await _authStateProvider.SetTokenAsync(token);
            return true;
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public async Task LogoutAsync()
        {
            await _authStateProvider.LogoutAsync();
        }

        public string GenerateJwtToken(Member account)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JwtSettings:SecretKey")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new List<Claim>
                    {
                        new Claim("id", account.MemberId.ToString()),
                        new Claim("email", account.Email ?? string.Empty),
                        new Claim("companyName", account.CompanyName.ToString()),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, account.Role ?? "User")
                    };
            Console.WriteLine(account.Role ?? "User");
            var token = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("JwtSettings:Issuer"),
                audience: _configuration.GetValue<string>("JwtSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}