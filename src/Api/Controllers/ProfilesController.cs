using Api.Controllers.DTOs;
using AutoMapper;
using Api.Extensions;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class ProfilesController : ApiControllerBase
{
    private readonly IMapper _mapper;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUserService _userService;

    public ProfilesController(
        IMapper mapper,
        IAuthorizationService authorizationService,
        IUserService userService)
    {
        _mapper = mapper;
        _authorizationService = authorizationService;
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpGet("{profileName}")]
    public async Task<IActionResult> GetByProfileName(string profileName)
    {
        var user = await _userService.GetUserProfileAsync(profileName);
        return user is null ? NotFound() : Ok(_mapper.Map<ProfileDto>(user));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProfileDto request)
    {
        var user = await _userService.GetUserByIdAsync(User.GetUserId());
        await _userService.CreateUserProfileAsync(user!, request.ProfileName, request.DisplayName);
        return CreatedAtAction(nameof(GetByProfileName), 
            new {profileName = user!.ProfileName}, 
            _mapper.Map<ProfileDto>(user));
    }
}