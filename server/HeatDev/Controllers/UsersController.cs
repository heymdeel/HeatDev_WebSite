using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HeatDev.ViewModels;
using HeatDevBLL.Models.DTO;
using HeatDevBLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HeatDev.Controllers
{
    [Produces("application/json")]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly IAccountService accountService;

        public UsersController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        // GET: api/users/{id}/profile
        /// <summary> Get user profile </summary>
        /// <response code="200"> user profile </response>
        /// <response code="403"> wrong token </response>
        /// <response code="404"> user doesn't exist </response>
        [HttpGet("{userId:int}/profile")]
        [Authorize]
        [ProducesResponseType(typeof(UserProfileVM), 200)]
        public async Task<IActionResult> GetProfile([FromRoute]int userId)
        {
            if (User.GetUserId() != userId)
            {
                return Forbid();
            }

            var profile = await accountService.GetUserProfileAsync(userId);
            if (profile == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<UserProfileVM>(profile));
        }

        // PUT: api/users/{id}/profile
        /// <summary> Change user's profile </summary>
        /// <response code="200"> profile updated </response>
        /// <response code="400"> invalid data </response>
        /// <response code="401"> need to atuhorize </response>
        /// <response code="403"> wrong token </response>
        /// <response code="404"> user doesn't exist </response>
        [HttpPut("{userId:int}/profile")]
        [Authorize]
        public async Task<IActionResult> ChangeProfile([FromRoute]int userId, [FromBody]UserProfileDTO newProfile)
        {
            if (userId != User.GetUserId())
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var profile = await accountService.GetUserProfileAsync(userId);
            if (profile == null)
            {
                return NotFound();
            }

            await accountService.UpdateUserProfileAsync(profile, newProfile);

            return Ok();
        }

        // PUT: api/users/{id}/password
        /// <summary> Change user's password </summary>
        /// <response code="200"> password changed </response>
        /// <response code="400"> invalid data </response>
        /// <response code="401"> need to atuhorize </response>
        /// <response code="403"> wrong token </response>
        /// <response code="404"> user doesn't exist </response>
        [HttpPut("{userId:int}/password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromRoute]int userId, [FromBody] ChangePasswordDTO passwordData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (userId != User.GetUserId())
            {
                return Forbid();
            }

            var user = await accountService.FindUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (!accountService.PasswordIsValidAsync(user, passwordData.OldPassword))
            {
                return Forbid();
            }

            await accountService.ChangeUserPasswordAsync(user, passwordData.NewPassword);

            return Ok();
        }
    }
}