using AutoMapper;
using BlogNetCore.Authorization;
using BlogNetCore.Common.DTOs;
using BlogNetCore.Extensions;
using BlogNetCore.Models;
using BlogNetCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogNetCore.Controllers;

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
    public async Task<IActionResult> Create([FromBody] CreateArticleDto request)
    {
        var tagIdsSet = request.TagIds.ToHashSet();
        if (tagIdsSet.Count != request.TagIds.Count)
            return ValidationProblem("Contains duplicate Tag ID.");

        var tags = tagIdsSet.Count > 0 ? (await _tagService.GetTagsById(tagIdsSet)).ToList()
            : new List<Tag>();
        if (tagIdsSet.Count > 0 && tagIdsSet.Count != tags.Count)
            return ValidationProblem("Some tags was invalid or not available.");

        await _articleService.CreateArticle(
            title: request.Title,
            content: request.Content,
            authorId: HttpContext.GetCurrentUserId(),
            tags);
        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("feed")]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> GetFeed()
    {
        var articles = await _articleService.GetFeedArticles();
        return Ok(_mapper.Map<IEnumerable<ArticleDto>>(articles));
    }

    [AllowAnonymous]
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var article = await _articleService.GetArticleBySlug(slug);
        return await NotFoundOrResult(article, async () =>
        {
            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, article, Operations.Read);
            if (!authorizationResult.Succeeded)
                return Forbid();
            
            return Ok(_mapper.Map<ArticleDto>(article));
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var article = await _articleService.GetArticleById(id);
        return await NotFoundOrResult(article, async () =>
        {
            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, article, Operations.Delete);
            if (!authorizationResult.Succeeded)
                return Forbid();
            
            await _articleService.DeleteArticle(article!);
            return NoContent();
        });
    }

    [HttpPost("{id:int}/publish")]
    public async Task<IActionResult> Publish(int id)
    {
        var article = await _articleService.GetArticleById(id);
        return await NotFoundOrResult(article, async () =>
        {
            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, article, Operations.Update);
            if (!authorizationResult.Succeeded)
                return Forbid();
            
            await _articleService.PublishArticle(article!);
            return NoContent();
        });
    }
    
    [HttpPost("{id:int}/archive")]
    public async Task<IActionResult> Archive(int id)
    {
        var article = await _articleService.GetArticleById(id);
        return await NotFoundOrResult(article, async () =>
        {
            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, article, Operations.Update);
            if (!authorizationResult.Succeeded)
                return Forbid();
            
            await _articleService.ArchiveArticle(article!);
            return NoContent();
        });
    }
}