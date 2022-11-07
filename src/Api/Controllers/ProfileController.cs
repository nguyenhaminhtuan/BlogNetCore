using Api.Auth;
using Api.Controllers.DTOs;
using AutoMapper;
using Api.Extensions;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize(Policy = AuthorizationPolicies.ActiveUserOnly)]
[Authorize(Policy = AuthorizationPolicies.VerifiedUserOnly)]
public class ProfileController : ApiControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public ProfileController(
        IMapper mapper,
        IUserService userService)
    {
        _mapper = mapper;
        _userService = userService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCurrentUserProfile()
    {
        await Task.CompletedTask;
        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("{profileName}")]
    public async Task<IActionResult> GetByProfileName(string profileName)
    {
        var user = await _userService.GetUserProfile(profileName);
        return user is null ? NotFound() : Ok(_mapper.Map<ProfileDto>(user));
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateProfileDto request)
    {
        var user = await _userService.GetUserById(User.GetUserId());
        await _userService.UpdateUserProfile(user!, request.ProfileName, request.DisplayName);
        return NoContent();
    }
}