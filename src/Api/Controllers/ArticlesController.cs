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
    private readonly IVoteService _voteService;
    private readonly ICommentService _commentService;

    public ArticlesController(
        IMapper mapper,
        IAuthorizationService authorizationService,
        IArticleService articleService,
        ITagService tagService,
        IVoteService voteService,
        ICommentService commentService)
    {
        _mapper = mapper;
        _authorizationService = authorizationService;
        _articleService = articleService;
        _tagService = tagService;
        _voteService = voteService;
        _commentService = commentService;
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

    private async Task<IActionResult> Vote(int articleId, bool isPositive)
    {
        var article = await _articleService.GetArticleById(articleId);
        if (article is null)
            return NotFound("Article not found");
        
        var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, article, ArticleOperations.Vote);
        if (!authorizationResult.Succeeded)
            return Forbid("You do not have permission to vote this article");

        await _voteService.VoteArticle(article.Id, User.GetUserId(), isPositive);
        return Ok();
    }

    [HttpPost("{id:int}/vote/up")]
    [SwaggerOperation(Summary = "Up vote article")]
    [SwaggerResponse(StatusCodes.Status200OK, "Up vote article successfully")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Don't have permission to vote article")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Can't find any article with given id")]
    public async Task<IActionResult> UpVote(int id)
    {
        return await Vote(id, true);
    }

    [HttpPost("{id:int}/vote/down")]
    [SwaggerOperation(Summary = "Down vote article")]
    [SwaggerResponse(StatusCodes.Status200OK, "Down vote article successfully")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Don't have permission to vote article")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Can't find any article with given id")]
    public async Task<IActionResult> DownVote(int id)
    {
        return await Vote(id, false);
    }

    [HttpDelete("{id:int}/vote")]
    [SwaggerOperation(Summary = "Remove article vote of current user")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Remove vote successfully")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Can't find any article with given id")]
    public async Task<IActionResult> UnVote(int id)
    {
        var article = await _articleService.GetArticleById(id);
        if (article is null)
            return NotFound("Article not found");

        var vote = await _voteService.GetArticleVoteByUser(User.GetUserId(), article);
        if (vote is null)
            return NoContent();
        
        await _voteService.DeleteVote(vote);
        return NoContent();
    }

    [HttpGet("{id:int}/comments")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get article's comments")]
    [SwaggerResponse(StatusCodes.Status200OK, "Get comments successfully", typeof(PaginatedDto<CommentDto>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid paginate input data")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Can't find any article with given id")]
    public async Task<IActionResult> GetComments(
        int id,
        [FromQuery] PaginateQuery paginate,
        [FromServices] IValidator<PaginateQuery> validator)
    {
        var validationResult = await validator.ValidateAsync(paginate);
        validationResult.AddToModelState(ModelState);
        if (!validationResult.IsValid)
            return BadRequest(ModelState);
        
        var article = await _articleService.GetArticleById(id);
        if (article is null)
            return NotFound("Article not found");

        var paginatedComments =
            await _commentService.GetCommentsByArticlePagination(paginate.PageIndex, paginate.PageSize, article.Id);
        return Ok(_mapper.Map<PaginatedDto<CommentDto>>(paginatedComments));
    }

    [HttpPost("{id:int}/comments")]
    [SwaggerOperation(Summary = "Create article comment")]
    [SwaggerResponse(StatusCodes.Status200OK, "New article comment created")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input comment data")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Don't have permission to comment article")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Can't find any article with given id")]
    public async Task<IActionResult> CreateComment(
        int id,
        [FromBody] CreateCommentDto dto,
        [FromServices] IValidator<CreateCommentDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        validationResult.AddToModelState(ModelState);
        if (!validationResult.IsValid)
            return BadRequest(ModelState);
        
        var article = await _articleService.GetArticleById(id);
        if (article is null)
            return NotFound("Article not found");
        
        var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, article, ArticleOperations.Comment);
        if (!authorizationResult.Succeeded)
            return Forbid("You do not have permission to comment this article");
        
        await _commentService.CreateArticleComment(dto.Body, User.GetUserId(), article);
        return Ok();
    }
}