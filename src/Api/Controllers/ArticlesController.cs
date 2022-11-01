﻿using AutoMapper;
using Api.Authorization;
using Api.Controllers.DTOs;
using Api.Extensions;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class ArticlesController : ApiControllerBase
{
    private readonly IMapper _mapper;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUserService _userService;
    private readonly IArticleService _articleService;
    private readonly ITagService _tagService;

    public ArticlesController(
        IMapper mapper,
        IAuthorizationService authorizationService,
        IUserService userService,
        IArticleService articleService,
        ITagService tagService)
    {
        _mapper = mapper;
        _authorizationService = authorizationService;
        _userService = userService;
        _articleService = articleService;
        _tagService = tagService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateArticleDto request)
    {
        var tags = (await _tagService.GetTagsById(request.TagIds)).ToHashSet();
        if (tags.Count != request.TagIds.Count)
            return BadRequest("Some tags was invalid or not available");
     
        var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, null, ArticleOperations.Create);
        if (!authorizationResult.Succeeded)
            return Forbid();
        
        var author = await _userService.GetUserByIdAsync(User.GetUserId());
        var article = await _articleService.CreateArticleAsync(
            title: request.Title,
            content: request.Content,
            author: author!,
            tags: tags);
        return CreatedAtAction(nameof(GetBySlug), 
            new { slug = article.Slug },
            _mapper.Map<ArticleDto>(article));
    }

    [AllowAnonymous]
    [HttpGet("feed")]
    public async Task<IActionResult> GetFeed()
    {
        var articles = await _articleService.GetFeedArticlesAsync();
        return Ok(_mapper.Map<IEnumerable<ArticleDto>>(articles));
    }

    [AllowAnonymous]
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var article = await _articleService.GetArticleBySlugAsync(slug);
        if (article is null)
            return NotFound();
        
        var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, article, ArticleOperations.Read);
        if (!authorizationResult.Succeeded)
            return Forbid();
        
        return Ok(_mapper.Map<ArticleDto>(article));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(UpdateArticleDto request, int id)
    {
        var tags = (await _tagService.GetTagsById(request.TagIds)).ToHashSet();
        if (tags.Count != request.TagIds.Count)
            return BadRequest("Some tags was invalid or not available");
        
        var article = await _articleService.GetArticleByIdAsync(id);
        if (article is null)
            return NotFound();
        
        var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, article, ArticleOperations.Update);
        if (!authorizationResult.Succeeded)
            return Forbid();
        
        await _articleService.UpdateArticleAsync(article, request.Title, request.Content, tags);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var article = await _articleService.GetArticleByIdAsync(id);
        if (article is null)
            return NotFound();
        
        var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, article, ArticleOperations.Delete);
        if (!authorizationResult.Succeeded)
            return Forbid();
        
        await _articleService.DeleteArticleAsync(article);
        return NoContent();
    }

    [HttpPost("{id:int}/publish")]
    public async Task<IActionResult> Publish(int id)
    {
        var article = await _articleService.GetArticleByIdAsync(id);
        if (article is null)
            return NotFound();
        
        var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, article, ArticleOperations.Publish);
        if (!authorizationResult.Succeeded)
            return Forbid();
        
        await _articleService.PublishArticleAsync(article);
        return NoContent();
    }
    
    [HttpPost("{id:int}/archive")]
    public async Task<IActionResult> Archive(int id)
    {
        var article = await _articleService.GetArticleByIdAsync(id);
        if (article is null)
            return NotFound();
        
        var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, article, ArticleOperations.Archive);
        if (!authorizationResult.Succeeded)
            return Forbid();
        
        await _articleService.ArchiveArticleAsync(article);
        return NoContent();
    }
}