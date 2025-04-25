using Microsoft.AspNetCore.Mvc;
using DuolingoClassLibrary.Entities;
using DuolingoClassLibrary.Repositories.Interfaces;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;

        public CommentController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        [HttpGet("{id}", Name = "GetCommentById")]
        public ActionResult<Comment> GetById(int id)
        {
            var comment = _commentRepository.GetCommentById(id);
            if (comment == null)
            {
                return NotFound();
            }
            return comment;
        }

        [HttpGet("post/{postId}", Name = "GetCommentsByPostId")]
        public ActionResult<IEnumerable<Comment>> GetByPostId(int postId)
        {
            var comments = _commentRepository.GetCommentsByPostId(postId);
            return Ok(comments);
        }

        [HttpGet("count/{postId}", Name = "GetCommentsCountForPost")]
        public ActionResult<int> GetCount(int postId)
        {
            var count = _commentRepository.GetCommentsCountForPost(postId);
            return Ok(count);
        }

        [HttpPost(Name = "CreateComment")]
        public ActionResult<int> Create([FromBody] Comment comment)
        {
            var commentId = _commentRepository.CreateComment(comment);
            return Ok(commentId);
        }

        [HttpDelete("{id}", Name = "DeleteComment")]
        public ActionResult Delete(int id)
        {
            var result = _commentRepository.DeleteComment(id);
            if (result)
            {
                return NoContent();
            }
            return BadRequest();
        }

        [HttpPut("{id}/like", Name = "LikeComment")]
        public ActionResult Like(int id)
        {
            var result = _commentRepository.IncrementLikeCount(id);
            if (result)
            {
                return NoContent();
            }
            return BadRequest();
        }
    }
} 