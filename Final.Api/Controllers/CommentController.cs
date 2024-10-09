using Final.Application.Dtos.CommentDtos;
using Final.Application.Exceptions;
using Final.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Final.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("addComment")]
        public async Task<IActionResult> Create(CommentCreateDto commentCreateDto)
        {
            var commentId = await _commentService.Create(commentCreateDto);
            return Ok(commentId);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll() => Ok(await _commentService.GetAll());

        [HttpGet("game/{gameId}")]
        public async Task<IActionResult> GetAllByGame(int gameId)
        {
            var comments = await _commentService.GetAll(gameId);
            if (comments == null || comments.Count == 0)
            {
                throw new CustomExceptions(404, "Comment", "No comments found for the specified game.");
            }
            return Ok(comments);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var data = await _commentService.GetOne(id);

            if (data is null)
            {
                throw new CustomExceptions(404, "Comment", "Given comment does not exist.");
            }

            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id > 0)
            {
                await _commentService.Delete(id);
                return Ok($"Comment with id '{id}' has been deleted successfully.");
            }

            throw new CustomExceptions(400, "Id", "Given id is invalid.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CommentUpdateDto commentUpdateDto)
        {
            if (id > 0)
            {
                await _commentService.Update(id, commentUpdateDto);
                return Ok("Comment updated successfully.");
            }

            throw new CustomExceptions(404, "Id", "Given id is invalid.");
        }
    }
}
