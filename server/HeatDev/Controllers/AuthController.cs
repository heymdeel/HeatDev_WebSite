using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using HeatDev.Options;
using HeatDev.ViewModels;
using HeatDevBLL.Models.DTO;
using HeatDevBLL.Models.Entities;
using HeatDevBLL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HeatDev.Controllers
{
    [Produces("application/json")]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IAccountService accountService;
        private readonly AuthOptions authOptions;

        public AuthController(IAccountService accountService, IOptions<AuthOptions> options)
        {
            this.authOptions = options.Value;
            this.accountService = accountService;
        }

        // POST: api/auth/sign_up
        /// <summary> Client sign up </summary>
        /// <response code="200"> user has been successfully signed up </response>
        /// <response code="400"> errors in model valdation or user already exists </response>
        [HttpPost("sign_up")]
        [ProducesResponseType(typeof(TokenVM), 200)]
        public async Task<IActionResult> SignUpUser([FromBody]UserSignUpDTO userData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await accountService.UserExistsAsync(userData.Login))
            {
                return BadRequest("user already exists");
            }

            User user = await accountService.SignUpUserAsync(userData);

            var tokens = await GenerateAndStoreTokensAsync(user);
            var tokenVM = Mapper.Map<TokenVM>(user);
            tokenVM.AccessToken = tokens.access;
            tokenVM.RefreshToken = tokens.refresh;

            return Ok(tokenVM);
        }

        // POST: api/auth/sign_in
        /// <summary> User sign in </summary>
        /// <response code="200"> user has been successfully signed in </response>
        /// <response code="400"> errors in model valdation or user already exists </response>
        /// <response code="404"> wrong login/password </response>
        [HttpPost("sign_in")]
        [ProducesResponseType(typeof(TokenVM), 200)]
        public async Task<IActionResult> SignInUser([FromBody]UserSignInDTO userData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = await accountService.SignInUserAsync(userData);
            if (user == null)
            {
                return NotFound();
            }

            var tokens = await GenerateAndStoreTokensAsync(user);
            var tokenVM = Mapper.Map<TokenVM>(user);
            tokenVM.AccessToken = tokens.access;
            tokenVM.RefreshToken = tokens.refresh;

            return Ok(tokenVM);
        }

        // POST: api/auth/token/refresh
        /// <summary> Refreshing tokens </summary>
        /// <response code="200"> tokens have been successfully refreshed </response>
        /// <response code="400"> refresh token is invalid </response>
        /// <response code="404"> user doesn't exist </response>
        [HttpPost("token/refresh")]
        [ProducesResponseType(typeof(TokenVM), 200)]
        public async Task<IActionResult> RefreshTokens([FromBody] string refreshToken)
        {
            int userId = GetUserIdFromToken(refreshToken);

            if (!await ValidateRerfreshTokenAsync(userId, refreshToken))
            {
                return BadRequest("invalid token. need to sign in");
            }

            User user = await accountService.FindUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var tokens = await GenerateAndStoreTokensAsync(user);
            var tokenVM = Mapper.Map<TokenVM>(user);
            tokenVM.AccessToken = tokens.access;
            tokenVM.RefreshToken = tokens.refresh;

            return Ok(tokenVM);
        }

        // POST: api/auth/token/invalidate
        /// <summary> Invalidate refresh token </summary>
        /// <response code="200"> refresh token has been successfully invalidated </response>
        /// <response code="400"> refresh token is invalid </response>
        [HttpPost("token/invalidate")]
        public async Task<IActionResult> InvalidateToken([FromBody]string refreshToken)
        {
            int userId = GetUserIdFromToken(refreshToken);

            if (!await ValidateRerfreshTokenAsync(userId, refreshToken))
            {
                return BadRequest("invalid token");
            }

            await accountService.InvalidateTokenAsync(userId, refreshToken);

            return NoContent();
        }

        private async Task<(string refresh, string access)> GenerateAndStoreTokensAsync(User user)
        {
            ClaimsIdentity identity = GetIdentity(user);

            string refreshToken = GenerateToken(identity, TokenType.Refresh);
            string accessToken = GenerateToken(identity, TokenType.Access);

            await accountService.StoreRefreshTokenAsync(user, refreshToken);

            return (refreshToken, accessToken);
        }

        private ClaimsIdentity GetIdentity(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim("user_id", user.Id.ToString())
            };

            claims.AddRange(user.Roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r)));

            return new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }

        private string GenerateToken(ClaimsIdentity identity, TokenType tokenType)
        {
            var jwt = new JwtSecurityToken(
                    issuer: authOptions.Issuer,
                    audience: tokenType == TokenType.Refresh ? authOptions.RefreshAudience : authOptions.AccessAudience,
                    notBefore: DateTime.Now,
                    claims: identity.Claims,
                    expires: DateTime.Now.AddMinutes(tokenType == TokenType.Refresh ? AuthOptions.REFRESH_LIFETIME : AuthOptions.ACCESS_LIFETIME),
                    signingCredentials: new SigningCredentials(authOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private async Task<bool> ValidateRerfreshTokenAsync(int userId, string refreshToken)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = authOptions.RefreshAudience,

                ValidateLifetime = true,

                IssuerSigningKey = authOptions.GetSymmetricSecurityKey(),
                ValidateIssuerSigningKey = true,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken = null;
            try
            {
                tokenHandler.ValidateToken(refreshToken, validationParameters, out validatedToken);
            }
            catch (SecurityTokenException)
            {
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }

            if (!await accountService.TokenIsValidAsync(userId, refreshToken))
            {
                return false;
            }

            return true;
        }

        private int GetUserIdFromToken(string refreshToken) => int.Parse(new JwtSecurityTokenHandler().ReadJwtToken(refreshToken).Claims.FirstOrDefault(c => c.Type == "user_id").Value);
    }
}