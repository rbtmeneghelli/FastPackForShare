using FastPackForShare.Cryptography;
using FastPackForShare.Extensions;
using FastPackForShare.Interfaces;
using FastPackForShare.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Frozen;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FastPackForShare.Services;

public class TokenService : ITokenService
{
    private List<RefreshTokensModel> _refreshTokens = new List<RefreshTokensModel>();
    private JwtConfigModel _jwtConfigModel { get; set; }
    private readonly IHttpClientFactory _ihttpClientFactory;
    private KeyCloakConfigModel _keyCloakConfigModel;

    public TokenService(IHttpClientFactory ihttpClientFactory, IOptions<KeyCloakConfigModel> keyCloakConfigModel)
    {
        _ihttpClientFactory = ihttpClientFactory;
        _keyCloakConfigModel = keyCloakConfigModel?.Value ?? throw new ArgumentException("KeyCloak não pode ser nulo", nameof(keyCloakConfigModel));
    }

    public JwtConfigModel SetJwtConfigModel(JwtConfigModel jwtConfigModel)
    {
        _jwtConfigModel = jwtConfigModel;
        return _jwtConfigModel;
    }

    public string GenerateToken(AuthenticationModel authenticationModel)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtConfigModel.Key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtConfigModel.Issuer,
            Audience = _jwtConfigModel.Audience,
            Subject = new ClaimsIdentity(authenticationModel.Roles),
            Expires = DateOnlyExtension.GetDateTimeNowFromBrazil().AddHours(_jwtConfigModel.Expired),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenCreated = tokenHandler.WriteToken(token);
        var encryptToken = CryptographyHashTokenManager.EncryptToken(tokenCreated, _jwtConfigModel.Key);
        return encryptToken;
    }

    public string GenerateRefreshToken(IEnumerable<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtConfigModel.Key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(_jwtConfigModel.Expired),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var key = Encoding.ASCII.GetBytes(_jwtConfigModel.Key);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (!(securityToken is JwtSecurityToken jwtSecurityToken))
            throw new SecurityTokenException("Token Invalido");
        else if (!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Token Invalido");

        return principal;
    }

    public void SaveRefreshToken(string username, string refreshToken)
    {
        _refreshTokens.Add(new RefreshTokensModel(username, refreshToken));
    }

    public string GetRefreshToken(string username)
    {
        return _refreshTokens.FirstOrDefault(x => x.Username == username).RefreshToken;
    }

    public void DeleteRefreshToken(string username, string refreshToken)
    {
        var item = _refreshTokens.FirstOrDefault(x => x.Username == username && x.RefreshToken == refreshToken);
        _refreshTokens.Remove(item);
    }

    public bool ValidateTokenAuthentication(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var validacoes = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = _jwtConfigModel.Issuer,
            ValidAudience = _jwtConfigModel.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfigModel.Key))
        };

        if (GuardClauseExtension.IsNullOrWhiteSpace(token))
            return false;

        var dataToken = handler.ValidateToken(token, validacoes, out var tokenSecure).Identity as ClaimsIdentity;
        bool tokenIsValid = dataToken != null ? true : false;
        return tokenIsValid;

    }

    public async Task<string> CreateJwtTokenByKeyCloak(string url, string login, string password)
    {
        var httpClient = _ihttpClientFactory.CreateClient("Signed");
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(90));
        httpClient.DefaultRequestHeaders.Accept.Clear();
        var tokenResponse = await httpClient.PostAsync(url,
        new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _keyCloakConfigModel.ClientId,
            ["client_secret"] = _keyCloakConfigModel.ClientSecret,
            ["grant_type"] = "password",
            ["username"] = login,
            ["password"] = password,
            ["scope"] = "openid profile"
        }.ToFrozenDictionary()), cts.Token);

        tokenResponse.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await tokenResponse.Content.ReadAsStringAsync());
        var token = doc.RootElement.GetProperty("access_token").GetString();
        return token?.Substring(0, 25);
    }

    #region Metodos para validação de acesso em duas etapas

    public string GenerateCodeTwoFactory(long userId, string username)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes($"{userId}//{username}:{DateTime.Now.Ticks}");
        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(dataBytes);
        string codeTwoFactory = Convert.ToBase64String(hashBytes);
        return codeTwoFactory;
    }

    public bool CheckCodeTwoFactory(long userId, string username, string inputCodeTwoFactory)
    {
        long expirationTime = TimeSpan.FromMinutes(10).Ticks;
        long ticksNow = DateOnlyExtension.GetDateTimeNowFromBrazil().Ticks;
        string expectedCode = GenerateCodeTwoFactory(userId, username);
        bool codesMatch = (inputCodeTwoFactory == expectedCode);
        bool withinExpiration = (DateOnlyExtension.GetDateTimeNowFromBrazil().Ticks - ticksNow) <= expirationTime;
        return codesMatch && withinExpiration; // Se for true o codigo da validação de duas etapas está OK
    }

    #endregion

    #region Metodos para gerar token de redefinição de senha até 24horas, por exemplo

    public string GenerateTokenRescuePassword(string email, string jwtSecretKey)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtSecretKey);

        var brazilianFuse = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        var brazilianDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow, brazilianFuse);

        var brazilianExpiration = new DateTime(
            brazilianDate.Year,
            brazilianDate.Month,
            brazilianDate.Day,
            brazilianDate.Hour, brazilianDate.Minute, brazilianDate.Second,
            DateTimeKind.Unspecified
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Email, email)
            }),

            Expires = brazilianExpiration.AddDays(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<CustomResponseModel> ValidateTokenRescuePassword(string token)
    {
        var brazilianFuse = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        var brazilianDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow, brazilianFuse);

        var tokenHandler = new JwtSecurityTokenHandler();

        if (tokenHandler.CanReadToken(token) is false)
            return new CustomResponseModel { StatusCode = (int)HttpStatusCode.InternalServerError, Data = null, Message = "Token Inválido" };

        var tokenData = tokenHandler.ReadToken(token);

        var validDate = TimeZoneInfo.ConvertTime(tokenData.ValidTo, brazilianFuse);

        bool expired = ((brazilianDate.Ticks / TimeSpan.TicksPerSecond) > (validDate.Ticks / TimeSpan.TicksPerSecond));

        if (expired)
            return new CustomResponseModel { StatusCode = (int)HttpStatusCode.InternalServerError, Data = null, Message = "Token Inválido" };

        return new CustomResponseModel { StatusCode = (int)HttpStatusCode.OK, Data = null, Message = "Token Valido" };
    }

    #endregion

    private AuthenticationModel ExtractDataFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var claims = jwtToken.Claims.ToList();
        AuthenticationModel dadosAutenticacaoTokenDTO = new();
        dadosAutenticacaoTokenDTO.Id = long.Parse(claims[0]?.Value);

        return dadosAutenticacaoTokenDTO;
    }
}
