using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Museo.Models;
using Museo.Models.Dtos;
using Museo.Services;
using System.Security.Claims;


namespace Museo.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")] 
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        private Guid GetCurrentUserId()
        {

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID is missing from token claims.");
            }

            return Guid.Parse(userIdClaim);
        }


        // GET /api/comments/canvas/{canvasId}
        [HttpGet("canvas/{canvasId:guid}")]
        
        public async Task<ActionResult<IEnumerable<CommentResponseDto>>> GetCommentsByCanvas(Guid canvasId)
        {
            var comments = await _commentService.GetCommentsByCanvas(canvasId);
            return Ok(comments);
        }

        // POST /api/comments
        [HttpPost]
        [Authorize(Roles = "Visitante")]
        public async Task<ActionResult<CommentResponseDto>> Create([FromBody] CreateCommentDto dto)
        {
            
            Guid currentUserId = GetCurrentUserId();
            
            var newComment = await _commentService.CreateComment(dto, currentUserId);
            return CreatedAtAction(
                nameof(GetCommentsByCanvas),
                new { canvasId = newComment.CanvasId },
                newComment);
        }

        // PUT /api/comments/{commentId}
        [HttpPut("{commentId:guid}")]
        [Authorize(Roles = "Visitante")]
        public async Task<ActionResult<CommentResponseDto>> Update(Guid commentId, [FromBody] UpdateCommentDto dto)
        {
            try
            {
                Guid currentUserId = GetCurrentUserId();

                var updatedComment = await _commentService.UpdateComment(commentId, dto, currentUserId);

                return Ok(updatedComment);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(); 
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message }); 
            }
        }

        // DELETE /api/comments/{commentId}
        [HttpDelete("{commentId:guid}")]
        [Authorize(Roles = "Visitante")]
        public async Task<IActionResult> Delete(Guid commentId)
        {
            try
            {
                Guid currentUserId = GetCurrentUserId();

                await _commentService.DeleteComment(commentId, currentUserId);

                return NoContent(); 
            }
            catch (KeyNotFoundException)
            {
                return NotFound(); 
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message }); 
            }
        }
    }
}




