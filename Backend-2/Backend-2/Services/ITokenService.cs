using Backend_2.Exeptions;
using Backend_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Web.Helpers;

namespace Backend_2.Services
{
    public interface ITokenService
    {
        ClaimsIdentity GetIdentity(string username, string password);
        Task AddRefreshToken(string username, string token);
        Task<IActionResult> GetToken(UserLoginDto model);
        Task<IActionResult> RefreshToken(TokenModel tokenModel);
    }
    public class TokenService : ITokenService
    {
        private readonly TestContext _context;
        private readonly IUserService _userService;
        public TokenService(TestContext context)
        {
            _context = context;
            _userService = new UserService(_context);
        }
        public ClaimsIdentity GetIdentity(string username, string password)
        {
            UserLogin[] _users = _userService.GetUserLogin();
            var user = _users.FirstOrDefault(x => x.Username == username && Crypto.VerifyHashedPassword(x.Password, password));
            if (user == null)
            {
                return null;
            }
            Role role = _context.Roles.Find(user.RoleId);
            var nameRole = role.Name;

            // Claims описывают набор базовых данных для авторизованного пользователя
            var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, nameRole)
        };

            //Claims identity и будет являться полезной нагрузкой в JWT токене, которая будет проверяться стандартным атрибутом Authorize
            var claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }

        public async Task AddRefreshToken(string username, string token)
        {
            var user = _context.User.Find(username);
            user.RefreshToken = token;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _context.SaveChangesAsync();
        }

        public async Task<IActionResult> GetToken(UserLoginDto model)
        {
            var identity = GetIdentity(model.Username, model.Password);
            if (identity == null)
            {
                throw new Exception();
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                issuer: JwtConfigurations.Issuer,
                audience: JwtConfigurations.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(JwtConfigurations.Lifetime)),
                signingCredentials: new SigningCredentials(JwtConfigurations.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var refreshToken = GenerateRefreshToken();
            await _userService.AddRefreshToken(model.Username, refreshToken);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name,
                refreshToken = refreshToken
            };

            return new JsonResult(response);

        }


        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            string? accessToken = tokenModel.AccessToken;
            string? refreshToken = tokenModel.RefreshToken;

            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                throw new ValidationExeption("Invalid access token or refresh token");
            }

            string username = principal.Identity.Name;

            var user = _userService.GetUserByUserName(username);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new ValidationExeption("Invalid access token or refresh token");
            }
            var now = DateTime.UtcNow;
            var newAccessToken = new JwtSecurityToken(
                issuer: JwtConfigurations.Issuer,
                audience: JwtConfigurations.Audience,
                notBefore: now,
                claims: principal.Claims,
                expires: now.Add(TimeSpan.FromMinutes(JwtConfigurations.Lifetime)),
                signingCredentials: new SigningCredentials(JwtConfigurations.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var newRefreshToken = GenerateRefreshToken();

            await _userService.AddRefreshToken(username, refreshToken);

            return new ObjectResult(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                refreshToken = newRefreshToken
            });

        }


        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = JwtConfigurations.GetSymmetricSecurityKey(),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;

        }
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
