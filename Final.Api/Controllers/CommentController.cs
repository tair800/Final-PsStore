using Final.Application.Dtos.CommentDtos;
using Final.Application.Exceptions;
using Final.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("forAdmin")]
        [Authorize(Roles = "admin,superAdmin")]
        public async Task<IActionResult> GetAllAdmin() => Ok(await _commentService.GetAll());

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

        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetCommentWithHistory(int id)
        {
            var comment = await _commentService.GetOne(id);

            if (comment == null)
            {
                throw new CustomExceptions(404, "Comment", "Given comment does not exist.");
            }

            var commentHistory = await _commentService.GetCommentHistory(id); // Method to fetch history

            var result = new
            {
                Comment = comment,
                History = commentHistory
            };

            return Ok(result);
        }

        [HttpPost("ReactToComment")]
        public async Task<IActionResult> ReactToComment([FromBody] CommentLikeDto reactionDto)
        {
            if (reactionDto.CommentId <= 0)
            {
                return BadRequest("Invalid CommentId.");
            }

            try
            {
                await _commentService.LikeOrDislikeComment(reactionDto);
                return Ok("Reaction recorded successfully.");
            }
            catch (CustomExceptions ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}/reactions")]
        public async Task<IActionResult> GetCommentReactions(int id)
        {
            var reactions = await _commentService.GetCommentReactions(id);
            if (reactions == null || !reactions.Any())
            {
                return NotFound("No reactions found for the specified comment.");
            }
            return Ok(reactions);
        }
    }
}
