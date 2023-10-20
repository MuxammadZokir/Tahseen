﻿using Microsoft.AspNetCore.Mvc;
using Tahseen.Api.Models;
using Tahseen.Service.DTOs.Users.UserSettings;
using Tahseen.Service.Interfaces.IUsersService;

namespace Tahseen.Api.Controllers.UsersControllers
{
    [ApiController]
    [Route("api[controller]")]
    public class UserSettingsController : ControllerBase
    {
        private IUserSettingService _userSettingService { get; set; }

        public UserSettingsController(IUserSettingService userSettingService)
        {
            _userSettingService = userSettingService;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(UserSettingsForCreationDto dto)
            => Ok(new Response()
            {
                Message = "Success",
                StatusCode = 200,
                Data = await _userSettingService.AddAsync(dto),
            });

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
            => Ok(new Response()
            {
                StatusCode = 200,
                Message = "Success",
                Data = await this._userSettingService.RetrieveAllAsync()
            });

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(long id)
            => Ok(new Response()
            {
                StatusCode = 200,
                Message = "Success",
                Data = await this._userSettingService.RetrieveByIdAsync(id)
            });


    }
}