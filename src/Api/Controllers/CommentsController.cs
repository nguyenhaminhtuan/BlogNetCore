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
public class CommentsController : ApiControllerBase
{
    private readonly IMapper _mapper;
    private readonly IVoteService _voteService;
    private readonly ICommentService _commentService;
    private readonly IAuthorizationService _authorizationService;
    
    public CommentsController(
        IMapper mapper,
        IVoteService voteService,
        ICommentService commentService,
        IAuthorizationService authorizationService)
    {
        _mapper = mapper;
        _voteService = voteService;
        _commentService = commentService;
        _authorizationService = authorizationService;
    }

    [HttpGet("{id:int}/replies")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get replies by comment pagination")]
    [SwaggerResponse(StatusCodes.Status200OK, "Get replies successfully", typeof(PaginatedDto<ReplyDto>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid paginate input data")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Comment not found")]
    public async Task<IActionResult> GetReplies(
        int id,
        [FromQuery] PaginateQuery paginate,
        [FromServices] IValidator<PaginateQuery> validator)
    {
        var validationResult = await validator.ValidateAsync(paginate);
        validationResult.AddToModelState(ModelState);
        if (!validationResult.IsValid)
            return BadRequest(ModelState);
        
        var comment = await _commentService.GetCommentById(id);
        if (comment is null)
            return NotFound("Comment not found");

        var paginatedReplies =
            await _commentService.GetRepliesByCommentPagination(paginate.PageIndex, paginate.PageSize, comment.Id);
        return Ok(_mapper.Map<PaginatedDto<ReplyDto>>(paginatedReplies));
    }
    
    [HttpPost("{id:int}/replies")]
    [SwaggerOperation(Summary = "Reply a comment")]
    [SwaggerResponse(StatusCodes.Status200OK, "Reply created")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid reply input data")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Permission denied")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Comment not found")]
    public async Task<IActionResult> CreateReply(
        int id, 
        [FromBody] CreateReplyDto dto,
        [FromServices] IValidator<CreateReplyDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        validationResult.AddToModelState(ModelState);
        if (!validationResult.IsValid)
            return BadRequest(ModelState);
        
        var comment = await _commentService.GetCommentById(id);
        if (comment is null)
            return NotFound("Comment not found");
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, comment, CommentOperations.Reply);
        if (!authorizationResult.Succeeded)
            return Forbid("You do not have permission to reply this comment");
        
        await _commentService.ReplyComment(dto.Body, User.GetUserId(), comment, dto.ReplyToId);
        return Ok();
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete comment")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Comment deleted")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Permission denied")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Comment not found")]
    public async Task<IActionResult> Delete(int id)
    {
        var comment = await _commentService.GetCommentById(id);
        if (comment is null)
            return NotFound("Comment not found");

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, comment, CommentOperations.Delete);
        if (!authorizationResult.Succeeded)
            return Forbid("You do not have permission to delete this comment");

        await _commentService.DeleteComment(comment);
        return NoContent();
    }

    private async Task<IActionResult> Vote(int id, bool upvote)
    {
        var comment = await _commentService.GetCommentById(id);
        if (comment is null)
            return NotFound("Comment not found");

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, comment, CommentOperations.Vote);
        if (!authorizationResult.Succeeded)
            return Forbid("You do not have permission to vote this comment");

        var userId = User.GetUserId();
        if (await _voteService.GetCommentVoteByUser(userId, comment) is not null)
            return Conflict("You already voted this comment");

        if (upvote)
            await _voteService.UpvoteComment(comment.Id, userId);
        else 
            await _voteService.DownvoteComment(comment.Id, userId);

        return Ok();
    }

    [HttpPost("{id:int}/vote/up")]
    [SwaggerOperation(Summary = "Up vote comment")]
    [SwaggerResponse(StatusCodes.Status200OK, "Up vote comment successfully")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Permission denied")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Comment not found")]
    public async Task<IActionResult> UpVote(int id)
    {
        return await Vote(id, true);
    }

    [HttpPost("{id:int}/vote/down")]
    [SwaggerOperation(Summary = "Down vote comment")]
    [SwaggerResponse(StatusCodes.Status200OK, "Up vote comment successfully")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Permission denied")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Comment not found")]
    public async Task<IActionResult> DownVote(int id)
    {
        return await Vote(id, false);
    }

    [HttpDelete("{id:int}/vote")]
    [SwaggerOperation(Summary = "Remove comment vote of current user")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Remove vote successfully")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication required")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Permission denied")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Comment not found")]
    public async Task<IActionResult> UnVote(int id)
    {
        var comment = await _commentService.GetCommentById(id);
        if (comment is null)
            return NotFound("Comment not found");
        
        var userId = User.GetUserId();
        var vote = await _voteService.GetCommentVoteByUser(userId, comment);
        if (vote is null)
            return NoContent();

        await _voteService.DeleteVote(vote);
        return NoContent();
    }
}