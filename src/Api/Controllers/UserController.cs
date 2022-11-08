using Api.Auth;
using Api.Controllers.DTOs;
using Api.Extensions;
using Api.Models;
using Api.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize(Policy = AuthorizationPolicies.ActiveUserOnly)]
[Authorize(Policy = AuthorizationPolicies.VerifiedUserOnly)]
public class UserController : ApiControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IArticleService _articleService;
    
    public UserController(
        IMapper mapper,
        IUserService userService,
        IArticleService articleService)
    {
        _mapper = mapper;
        _userService = userService;
        _articleService = articleService;
    }
    
    [HttpPut]
    public async Task<IActionResult> Update(UpdateProfileDto request)
    {
        var user = await _userService.GetUserById(User.GetUserId());
        await _userService.UpdateUserProfile(user!, request.ProfileName, request.DisplayName);
        return NoContent();
    }
    
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _userService.GetUserById(User.GetUserId());
        var profileDto = _mapper.Map<ProfileDto>(user);
        profileDto.TotalArticles = await _articleService.CountPublishedArticlesByAuthor(user!.Id);
        return Ok(profileDto);
    }

    [HttpGet("articles")]
    public async Task<IActionResult> GetArticles([FromQuery] PaginateParams query)
    {
        var articlesPaginated = await _articleService
            .GetPublishedArticlesByAuthorPagination(User.GetUserId(), query);
        return Ok(_mapper.Map<PaginatedDto<ArticleDto>>(articlesPaginated));
    }
}