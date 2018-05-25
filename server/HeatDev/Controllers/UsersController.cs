﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HeatDev.ViewModels;
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
        /// <response code="200"> tokens have been successfully refreshed </response>
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
    }
}