using AutoMapper;
using BlogNetCore.Common.DTOs;
using BlogNetCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogNetCore.Controllers;

public class ProfilesController : ApiControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    
    public ProfilesController(IMapper mapper, IUserService userService)
    {
        _mapper = mapper;
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpGet("{username}")]
    public async Task<IActionResult> GetByUsername(string username)
    {
        var user = await _userService.GetUserByUsername(username);
        return NotFoundOrResult(user, Ok(_mapper.Map<ProfileDto>(user)));
    }
}