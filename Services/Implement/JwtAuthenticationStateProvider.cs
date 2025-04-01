using Blazored.SessionStorage;
using BusinessObject.Entities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ISessionStorageService _sessionStorage;
    private readonly IConfiguration _configuration;
    private bool _isPrerendering = true;

    public JwtAuthenticationStateProvider(ISessionStorageService sessionStorage, IConfiguration configuration)
    {
        _sessionStorage = sessionStorage;
        _configuration = configuration;
    }

    public async Task LogoutAsync()
    {
        await _sessionStorage.RemoveItemAsync("authToken");
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task SetTokenAsync(string token)
    {
        Console.WriteLine($"Saving token to session storage: {token}");
        await _sessionStorage.SetItemAsync("authToken", token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
    public async Task<int?> GetIdRoleAsync()
    {
        var token = await _sessionStorage.GetItemAsync<string>("authToken");
        if (string.IsNullOrEmpty(token))
            return null;

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "id");
        return int.Parse(roleClaim?.Value!);
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_isPrerendering)
        {
            Console.WriteLine("Session storage is not available during prerendering");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var token = await _sessionStorage.GetItemAsync<string>("authToken");
        Console.WriteLine($"Retrieved token from storage: {token}");

        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("No token found, returning unauthenticated state");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        try
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JwtSettings:SecretKey")!)),
                ValidateIssuer = true,
                ValidIssuer = _configuration.GetValue<string>("JwtSettings:Issuer"),
                ValidateAudience = true,
                ValidAudience = _configuration.GetValue<string>("JwtSettings:Audience"),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            ClaimsPrincipal principal = handler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            JwtSecurityToken? jwtToken = validatedToken as JwtSecurityToken;

            if (jwtToken == null)
            {
                Console.WriteLine("Token is not a valid JWT");
                await LogoutAsync();
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            ClaimsIdentity identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
            Console.WriteLine("Token validated successfully");
            Console.WriteLine($"Claims: id={identity.FindFirst("id")?.Value}, email={identity.FindFirst("email")?.Value}");
            ClaimsPrincipal user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}");
            await LogoutAsync();
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public void SetPrerendering(bool isPrerendering)
    {
        _isPrerendering = isPrerendering;
    }
}
