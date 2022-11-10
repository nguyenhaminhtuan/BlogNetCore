using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class CommentsController : ApiControllerBase
{
    [HttpGet("{id:int}/replies")]
    public Task<IActionResult> GetReplies(int id)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id:int}")]
    public Task<IActionResult> Delete(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost("{id:int}/vote")]
    public Task<IActionResult> Upvote(int id)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id:int}/vote")]
    public Task<IActionResult> Downvote(int id)
    {
        throw new NotImplementedException();
    }
}