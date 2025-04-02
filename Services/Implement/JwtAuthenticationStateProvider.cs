using Blazored.SessionStorage;
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
        var token = await _sessionStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

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

        try
        {
            ClaimsPrincipal principal = handler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            JwtSecurityToken? jwtToken = validatedToken as JwtSecurityToken;

            if (jwtToken == null || jwtToken.ValidTo < DateTime.UtcNow)
            {
                await LogoutAsync();
                await _sessionStorage.RemoveItemAsync("authToken");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            ClaimsIdentity identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
            ClaimsPrincipal user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }
        catch (SecurityTokenExpiredException)
        {
            await LogoutAsync();
            await _sessionStorage.RemoveItemAsync("authToken");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        catch (Exception)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }
}
