using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdxSlots.API.Dtos.Games;
using PdxSlots.API.Dtos.Rounds;
using PdxSlots.API.Dtos.Users;
using PdxSlots.API.Services.Interfaces;
using System.Collections.Generic;

namespace PdxSlots.API.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<AdminUserGetDto>> Get([FromQuery] string search)
        {
            return await _userService.GetUsers(search);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<UserGetDto> GetMe()
        {
            return await _userService.GetMe();
        }

        [HttpPost("me")]
        [Authorize]
        public async Task<UserGetDto> CreateMe([FromBody] UserPostDto userPostDto)
        {
            return await _userService.CreateMe(userPostDto);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<bool> Update([FromRoute] int id, [FromBody] AdminUserPutDto adminUserPutDto)
        {
            return await _userService.UpdateUser(id, adminUserPutDto);
        }

        [HttpPost("invite")]
        [Authorize]
        public async Task<AdminUserGetDto> Invite([FromBody] UserInviteDto userInviteDto)
        {
            return await _userService.InviteUser(userInviteDto);
        }
    }
}