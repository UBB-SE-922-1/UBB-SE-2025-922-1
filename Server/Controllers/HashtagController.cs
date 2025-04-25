using Microsoft.AspNetCore.Mvc;
using DuolingoClassLibrary.Data;
using DuolingoClassLibrary.Entities;
using DuolingoClassLibrary.Repositories.Repos;
using DuolingoClassLibrary.Repositories.Interfaces;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HashtagController : ControllerBase
    {
        private readonly IHashtagRepository _hashtagRepository;

        public HashtagController(IHashtagRepository hashtagRepository)
        {
            _hashtagRepository = hashtagRepository;
        }

        [HttpGet(Name = "GetAllHashtags")]
        public async Task<IEnumerable<Hashtag>> Get()
        {
            return await _hashtagRepository.GetAllHashtags();
        }

        [HttpGet("text/{text}", Name = "GetHashtagByText")]
        public async Task<ActionResult<Hashtag>> GetByText(string text)
        {
            var hashtag = await _hashtagRepository.GetHashtagByText(text);
            if (hashtag == null)
            {
                return NotFound();
            }
            return hashtag;
        }

        [HttpGet("post/{postId}", Name = "GetHashtagsByPostId")]
        public async Task<IEnumerable<Hashtag>> GetByPostId(int postId)
        {
            return await _hashtagRepository.GetHashtagsByPostId(postId);
        }

        [HttpGet("category/{categoryId}", Name = "GetHashtagsByCategory")]
        public async Task<IEnumerable<Hashtag>> GetByCategory(int categoryId)
        {
            return await _hashtagRepository.GetHashtagsByCategory(categoryId);
        }

        [HttpPost(Name = "CreateHashtag")]
        public async Task<ActionResult<Hashtag>> Create([FromBody] HashtagCreateRequest request)
        {
            var hashtag = await _hashtagRepository.CreateHashtag(request.Tag);
            return CreatedAtRoute("GetHashtagByText", new { text = hashtag.Tag }, hashtag);
        }

        [HttpPost("post", Name = "AddHashtagToPost")]
        public async Task<ActionResult> AddHashtagToPost([FromBody] HashtagPostRequest request)
        {
            var result = await _hashtagRepository.AddHashtagToPost(request.PostId, request.HashtagId);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("post/{postId}/{hashtagId}", Name = "RemoveHashtagFromPost")]
        public async Task<ActionResult> RemoveHashtagFromPost(int postId, int hashtagId)
        {
            var result = await _hashtagRepository.RemoveHashtagFromPost(postId, hashtagId);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }
    }

    public class HashtagCreateRequest
    {
        public string Tag { get; set; }
    }

    public class HashtagPostRequest
    {
        public int PostId { get; set; }
        public int HashtagId { get; set; }
    }
} 