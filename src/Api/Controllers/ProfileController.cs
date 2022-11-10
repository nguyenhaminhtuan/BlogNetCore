using Api.Controllers.DTOs;
using Api.Models;
using AutoMapper;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
    [SwaggerOperation(Summary = "Get profile by profile name")]
    [SwaggerResponse(StatusCodes.Status200OK, "Get profile successfully", typeof(ProfileDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Profile not found")]
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
    [SwaggerOperation(Summary = "Get published articles by profile name and pagination")]
    [SwaggerResponse(StatusCodes.Status200OK, "Get articles successfully", typeof(PaginatedDto<ArticleDto>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Profile not found")]
    public async Task<IActionResult> GetArticles(string profileName, [FromQuery] PaginateQuery query)
    {
        var user = await _userService.GetUserByProfileName(profileName);
        if (user is null)
            return NotFound("Profile not found");
        
        var articlesPaginated = await _articleService
            .GetArticlesByAuthorFilterPagination(user.Id, query.PageIndex, query.PageSize, ArticleStatus.Published);
        return Ok(_mapper.Map<PaginatedDto<ArticleDto>>(articlesPaginated));
    }
}