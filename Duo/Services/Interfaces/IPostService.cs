using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Server.Entities;

namespace Duo.Services.Interfaces
{
    public interface IPostService
    {
        int CreatePost(Post newPost);
        void DeletePost(int postId);
        void UpdatePost(Post postToUpdate);
        Post? GetPostById(int postId);
        Collection<Post> GetPostsByCategory(int categoryId, int pageNumber, int pageSize);
        List<Post> GetPaginatedPosts(int pageNumber, int pageSize);
        int GetTotalPostCount();
        int GetPostCountByCategoryId(int categoryId);
        int GetPostCountByHashtags(List<string> hashtagList);
        List<Hashtag> GetAllHashtags();
        List<Hashtag> GetHashtagsByCategory(int categoryId);
        List<Hashtag> GetHashtags(int? categoryId);
        List<Post> GetPostsByHashtags(List<string> hashtagList, int pageNumber, int pageSize);
        bool ValidatePostOwnership(int authorUserId, int targetPostId);
        List<Hashtag> GetHashtagsByPostId(int postId);
        bool LikePost(int postId);
        Post? GetPostDetailsWithMetadata(int postId);
        bool AddHashtagToPost(int postId, string hashtagName, int userId);
        bool RemoveHashtagFromPost(int postId, int hashtagId, int userId);
        int CreatePostWithHashtags(Post newPost, List<string> hashtagList, int authorId);
        (List<Post> Posts, int TotalCount) GetFilteredAndFormattedPosts(
            int? categoryId,
            List<string> selectedHashtags,
            string filterText,
            int currentPage,
            int itemsPerPage);
        HashSet<string> ToggleHashtagSelection(HashSet<string> currentHashtags, string hashtagToToggle, string allHashtagsFilter);
    }
} 