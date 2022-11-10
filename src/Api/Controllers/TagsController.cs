using Api.Controllers.DTOs;
using Api.Models;
using Api.Services;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

public class TagsController : ApiControllerBase
{
    private readonly IMapper _mapper;
    private readonly ITagService _tagService;
    private readonly IArticleService _articleService;

    public TagsController(
        IMapper mapper,
        ITagService tagService,
        IArticleService articleService)
    {
        _mapper = mapper;
        _tagService = tagService;
        _articleService = articleService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all available tags")]
    [SwaggerResponse(StatusCodes.Status200OK, "Get tags successfully", typeof(IEnumerable<TagDto>))]
    public async Task<IActionResult> GetAll()
    {
        var tags = await _tagService.GetTags();
        return Ok(_mapper.Map<IEnumerable<TagDto>>(tags));
    }

    [HttpGet("{slug}/articles")]
    public async Task<IActionResult> GetArticlesBySlugPagination(
        string slug,
        [FromQuery] PaginateQuery paginate,
        [FromServices] IValidator<PaginateQuery> validator)
    {
        var validateResult = await validator.ValidateAsync(paginate);
        validateResult.AddToModelState(ModelState);
        if (!validateResult.IsValid)
            BadRequest(ModelState);

        var tag = await _tagService.GetTagBySlug(slug);
        if (tag is null)
            return NotFound("Tag not found");

        var paginatedArticles =
            await _articleService.GetPublishedArticlesByTagPagination(tag.Id, paginate.PageIndex, paginate.PageSize);
        return Ok(_mapper.Map<PaginatedDto<ArticleDto>>(paginatedArticles));
    }
}