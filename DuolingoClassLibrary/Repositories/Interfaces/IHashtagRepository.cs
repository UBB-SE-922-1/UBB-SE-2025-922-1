using System;
using System.Collections.Generic;
using DuolingoClassLibrary.Entities;
using System.Threading.Tasks;

namespace DuolingoClassLibrary.Repositories.Interfaces
{
    public interface IHashtagRepository
    {
        Task<Hashtag> GetHashtagByText(string textToSearchBy);
        Task<Hashtag> CreateHashtag(string newHashtagTag);
        Task<List<Hashtag>> GetHashtagsByPostId(int postId);
        Task<bool> AddHashtagToPost(int postId, int hashtagId);
        Task<bool> RemoveHashtagFromPost(int postId, int hashtagId);
        Task<Hashtag> GetHashtagByName(string hashtagName);
        Task<List<Hashtag>> GetAllHashtags();
        Task<List<Hashtag>> GetHashtagsByCategory(int categoryId);
    }
} 