using AutoMapper;
using BlogNetCore.Common.DTOs;
using BlogNetCore.Extensions;
using BlogNetCore.Models;
using BlogNetCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogNetCore.Controllers;

public class ArticlesController : ApiControllerBase
{
    private readonly IArticleService _articleService;
    private readonly ITagService _tagService;
    private readonly IMapper _mapper;

    public ArticlesController(
        IArticleService articleService,
        ITagService tagService,
        IMapper mapper)
    {
        _articleService = articleService;
        _tagService = tagService;
        _mapper = mapper;
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
        var dto = _mapper.Map<IEnumerable<ArticleDto>>(articles);
        return Ok(dto);
    }

    [AllowAnonymous]
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var article = await _articleService.GetArticleBySlug(slug);
        return NotFoundOrResult(article, Ok(article));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var article = await _articleService.GetArticleById(id);
        return await NotFoundOrResult(article, async () =>
        {
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
            await _articleService.ArchiveArticle(article!);
            return NoContent();
        });
    }
}