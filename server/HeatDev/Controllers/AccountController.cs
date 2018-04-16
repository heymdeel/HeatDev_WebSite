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
            var tokenVM = await GetTokenVMAsync(user);

            return Ok(tokenVM);
        }

        // POST: api/auth/sign_in
        [HttpPost("sign_in")]
        [ProducesResponseType(typeof(TokenVM), 200)]
        public async Task<IActionResult> SignInUser([FromBody]UserSignInDTO userData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await accountService.PasswordIsValid(userData.Login, userData.Password))
            {
                return BadRequest("wrong login or password");
            }

            User user = await accountService.SignInUserAsync(userData);
            if (user == null)
            {
                return NotFound();
            }

            var tokenVM = await GetTokenVMAsync(user);

            return Ok(tokenVM);
        }

        // POST: api/auth/token
        [HttpPost("token")]
        [ProducesResponseType(typeof(TokenVM), 200)]
        public async Task<IActionResult> RefreshTokens([FromBody] string refreshToken)
        {
            if (!ValidateRerfreshToken(refreshToken))
            {
                return BadRequest("invalid token. need to sign in");
            }

            int userId = GetUserIdFromToken(refreshToken);

            User user = await accountService.FindUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var tokenVM = await GetTokenVMAsync(user);

            return Ok(tokenVM);
        }

        // DELETE: api/auth/token
        [HttpDelete("token")]
        public async Task<IActionResult> InvalidateToken([FromBody] string refreshToken)
        {
            if (!ValidateRerfreshToken(refreshToken))
            {
                return BadRequest("invalid token");
            }

            int userId = GetUserIdFromToken(refreshToken);

            await accountService.InvalidateTokenAsync(userId, refreshToken);

            return NoContent();
        }

        private ClaimsIdentity GetIdentity(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("user_id", user.Id.ToString())
            };

            claims.AddRange(user.Roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r)));

            return new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }

        private string GenerateToken(ClaimsIdentity identity, TokenType tokenType)
        {
            Console.WriteLine(authOptions.Key);

            var jwt = new JwtSecurityToken(
                    issuer: authOptions.Issuer,
                    audience: tokenType == TokenType.Refresh ? authOptions.RefreshAudience : authOptions.AccessAudience,
                    notBefore: DateTime.Now,
                    claims: identity.Claims,
                    expires: DateTime.Now.AddMinutes(tokenType == TokenType.Refresh ? AuthOptions.REFRESH_LIFETIME : AuthOptions.ACCESS_LIFETIME),
                    signingCredentials: new SigningCredentials(authOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private async Task<TokenVM> GetTokenVMAsync(User user)
        {
            ClaimsIdentity identity = GetIdentity(user);

            string refreshToken = GenerateToken(identity, TokenType.Refresh);
            string accessToken = GenerateToken(identity, TokenType.Access);

            await accountService.StoreRefreshTokenAsync(user, refreshToken);

            var tokenVM = Mapper.Map<TokenVM>(user);
            tokenVM.AccessToken = accessToken;
            tokenVM.RefreshToken = refreshToken;

            return tokenVM;
        }

        private bool ValidateRerfreshToken(string refreshToken)
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

            return true;
        }

        private int GetUserIdFromToken(string refreshToken) => int.Parse(new JwtSecurityTokenHandler().ReadJwtToken(refreshToken).Claims.FirstOrDefault(c => c.Type == "user_id").Value);
    }
}