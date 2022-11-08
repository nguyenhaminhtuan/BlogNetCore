using Api.Controllers.DTOs;
using Api.Models;
using AutoMapper;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class ProfileController : ApiControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IArticleService _articleService;

    public ProfileController(
        IMapper mapper,
        IUserService userService,
        IArticleService articleService)
    {
        _mapper = mapper;
        _userService = userService;
        _articleService = articleService;
    }
    
    [HttpGet("{profileName}")]
    public async Task<IActionResult> GetByProfileName(string profileName)
    {
        var user = await _userService.GetUserByProfileName(profileName);
        if (user is null)
            return NotFound("Profile not found");
        
        var profile = _mapper.Map<ProfileDto>(user);
        profile.TotalArticles = await _articleService.CountPublishedArticlesByAuthor(user.Id);
        return Ok(profile);
    }
    
    [HttpGet("{profileName}/articles")]
    public async Task<IActionResult> GetArticles(string profileName, [FromQuery] PaginateParams query)
    {
        var user = await _userService.GetUserByProfileName(profileName);
        if (user is null)
            return NotFound("Profile not found");
        
        var articlesPaginated = await _articleService
            .GetPublishedArticlesByAuthorPagination(user.Id, query);
        return Ok(_mapper.Map<PaginatedDto<ArticleDto>>(articlesPaginated));
    }
}