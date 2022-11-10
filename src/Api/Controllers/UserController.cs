using Api.Auth;
using Api.Controllers.DTOs;
using Api.Extensions;
using Api.Models;
using Api.Services;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Authorize(Policy = AuthorizationPolicies.ActiveUserOnly)]
[Authorize(Policy = AuthorizationPolicies.VerifiedUserOnly)]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
[SwaggerResponse(StatusCodes.Status403Forbidden, "Unverified or disabled account")]
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
    [SwaggerOperation(Summary = "Update current logged in user")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "User was updated successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid user update data")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "New profile name was taken")]
    public async Task<IActionResult> Update(
        UpdateUserDto dto,
        [FromServices] IValidator<UpdateUserDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        validationResult.AddToModelState(ModelState);
        if (!validationResult.IsValid)
            return BadRequest(ModelState);
        
        var user = await _userService.GetUserById(User.GetUserId());
        await _userService.UpdateUser(user!, dto.ProfileName, dto.DisplayName);
        return NoContent();
    }
    
    [HttpGet("profile")]
    [SwaggerOperation(Summary = "Get current logged in user profile")]
    [SwaggerResponse(StatusCodes.Status200OK, "Get profile successfully", typeof(ProfileDto))]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _userService.GetUserById(User.GetUserId());
        var profileDto = _mapper.Map<ProfileDto>(user);
        profileDto.TotalArticles = await _articleService.CountPublishedArticlesByAuthor(user!.Id);
        return Ok(profileDto);
    }

    [HttpGet("articles")]
    [SwaggerOperation(Summary = "Get current logged in user articles")]
    [SwaggerResponse(StatusCodes.Status200OK, "Get articles successfully", typeof(PaginatedDto<ArticleDto>))]
    public async Task<IActionResult> GetArticles(
        [FromServices] IValidator<PaginateQuery> validator,
        [FromQuery] PaginateQuery paginate,
        [FromQuery] ArticleStatus status = ArticleStatus.Published)
    {
        if (status > ArticleStatus.Archived)
            ModelState.AddModelError("status", "Invalid article status");
        
        var validationResult = await validator.ValidateAsync(paginate);
        validationResult.AddToModelState(ModelState);
        if (!validationResult.IsValid || !ModelState.IsValid)
            return BadRequest(ModelState);

        var articlesPaginated = await _articleService
            .GetArticlesByAuthorFilterPagination(
                User.GetUserId(), 
                paginate.PageIndex,
                paginate.PageSize,
                status);
        return Ok(_mapper.Map<PaginatedDto<ArticleDto>>(articlesPaginated));
    }
}