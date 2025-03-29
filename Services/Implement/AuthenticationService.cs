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
        private readonly IMemeberRepository _memeberRepository;
        private readonly IConfiguration _configuration;
        public AuthenticationService(JwtAuthenticationStateProvider authStateProvider, IMemeberRepository memeberRepository, IConfiguration configuration)
        {
            _authStateProvider = authStateProvider;
            _configuration = configuration;
            _memeberRepository = memeberRepository;

        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var account = await _memeberRepository.Login(username, password);
            if (account != null)
            {
                string token = GenerateJwtToken(account);
                await _authStateProvider.SetTokenAsync(token);
                return true;
            }
            return false;
        }

        public async Task LogoutAsync()
        {
            await _authStateProvider.LogoutAsync();
        }

        private string GenerateJwtToken(Member account)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JwtSettings:SecretKey")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, account.Email),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

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