using Api.Auth;
using AutoMapper;
using Api.Controllers.DTOs;
using Api.Extensions;
using Api.Models;
using Api.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Authorize(Policy = AuthorizationPolicies.ActiveUserOnly)]
[Authorize(Policy = AuthorizationPolicies.VerifiedUserOnly)]
public class ArticlesController : ApiControllerBase
{
    private readonly IMapper _mapper;
    private readonly IAuthorizationService _authorizationService;
    private readonly IArticleService _articleService;
    private readonly ITagService _tagService;

    public ArticlesController(
        IMapper mapper,
        IAuthorizationService authorizationService,
        IArticleService articleService,
        ITagService tagService)
    {
        _mapper = mapper;
        _authorizationService = authorizationService;
        _articleService = articleService;
        _tagService = tagService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new draft article")]
    [SwaggerResponse(StatusCodes.Status201Created, "Draft article was created", typeof(ArticleDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input article data", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Do not have permission for create a draft article")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Article title already taken")]
    public async Task<IActionResult> Create(
        CreateArticleDto request,
        [FromServices] IValidator<CreateArticleDto> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        validationResult.AddToModelState(ModelState);
        if (!validationResult.IsValid)
            return BadRequest(ModelState);

        var tags = (await _tagService.GetTagsById(request.TagIds)).ToHashSet();
        if (tags.Count != request.TagIds.Count)
            return BadRequest("One or more tags were not found");

        var article = new Article();
        var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, article, ArticleOperations.Create);
        if (!authorizationResult.Succeeded)
            return Forbid("You do not have permission for create article");

        article = await _articleService.CreateArticle(
            title: request.Title,
            content: request.Content,
            authorId: User.GetUserId(),
            tags: tags);
        return CreatedAtAction(nameof(GetBySlug),
            new { slug = article.Slug },
            _mapper.Map<ArticleDto>(article));
    }

    [HttpGet]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get all published articles by pagination")]
    [SwaggerResponse(StatusCodes.Status200OK, "Get articles successfully", typeof(PaginatedDto<ArticleDto>))]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginateQuery query,
        [FromServices] IValidator<PaginateQuery> validator)
    {
        var validationResult = await validator.ValidateAsync(query);
        validationResult.AddToModelState(ModelState);
        if (!validationResult.IsValid)
            return BadRequest(ModelState);

        var paginatedArticles = await _articleService
            .GetPublishedArticlesByPagination(query.PageIndex, query.PageSize);
        return Ok(_mapper.Map<PaginatedDto<ArticleDto>>(paginatedArticles));
    }

    [HttpGet("{slug}")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get article by slug")]
    [SwaggerResponse(StatusCodes.Status200OK, "Get article successfully", typeof(ArticleDto))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Do not have permission to read article")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Article not found")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var article = await _articleService.GetArticleBySlug(slug);
        if (article is null)
            return NotFound("Article not found");

        var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, article, ArticleOperations.Read);
        if (!authorizationResult.Succeeded)
            return Forbid("You do not have permission for read this article");

        return Ok(_mapper.Map<ArticleDto>(article));
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Update a article")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Article was updated")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input article data", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Do not have permission for update article")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Can't find any article with given id")]
    public async Task<IActionResult> Update(
        int id,
        UpdateArticleDto request,
        [FromServices] IValidator<UpdateArticleDto> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        validationResult.AddToModelState(ModelState);
        if (!validationResult.IsValid)
            return BadRequest(ModelState);

        var tags = (await _tagService.GetTagsById(request.TagIds)).ToHashSet();
        if (tags.Count != request.TagIds.Count)
            return BadRequest("One or more tags were not found");

        var article = await _articleService.GetArticleById(id);
        if (article is null)
            return NotFound("Article not found");

        var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, article, ArticleOperations.Update);
        if (!authorizationResult.Succeeded)
            return Forbid("You do not have permission for create article");

        await _articleService.UpdateArticle(article, request.Title, request.Content, tags);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete a article")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Article was updated")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Don't have permission for delete article")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Can't find any article with given id")]
    public async Task<IActionResult> Delete(int id)
    {
        var article = await _articleService.GetArticleById(id);
        if (article is null)
            return NotFound("Article not found");

        var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, article, ArticleOperations.Delete);
        if (!authorizationResult.Succeeded)
            return Forbid("You do not have permission for delete this article");

        await _articleService.DeleteArticle(article);
        return NoContent();
    }

    [HttpPost("{id:int}/publish")]
    [SwaggerOperation(Summary = "Publish a draft article")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Article was published")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Don't have permission for publish article")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Can't find any article with given id")]
    public async Task<IActionResult> Publish(int id)
    {
        var article = await _articleService.GetArticleById(id);
        if (article is null)
            return NotFound("Article not found");

        var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, article, ArticleOperations.Publish);
        if (!authorizationResult.Succeeded)
            return Forbid("You do not have permission for update this article");

        await _articleService.PublishArticle(article);
        return NoContent();
    }

    [HttpPost("{id:int}/archive")]
    [SwaggerOperation(Summary = "Archive a published article")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Article was archived")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Don't have permission for archive article")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Can't find any article with given id")]
    public async Task<IActionResult> Archive(int id)
    {
        var article = await _articleService.GetArticleById(id);
        if (article is null)
            return NotFound("Article not found");

        var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, article, ArticleOperations.Archive);
        if (!authorizationResult.Succeeded)
            return Forbid("You do not have permission for archive this article");

        await _articleService.ArchiveArticle(article);
        return NoContent();
    }

    [HttpPost("{id:int}/vote")]
    [SwaggerOperation(Summary = "Upvote article")]
    public Task<IActionResult> Upvote(int id)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id:int}/vote")]
    [SwaggerOperation(Summary = "Downvote article")]
    public Task<IActionResult> Downvote(int id)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id:int}/comments")]
    [SwaggerOperation(Summary = "Get article's comments")]
    public Task<IActionResult> GetComments(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost("{id:int}/comments")]
    [SwaggerOperation(Summary = "Create article comment")]
    public Task<IActionResult> CreateComment(int id)
    {
        throw new NotImplementedException();
    }
}