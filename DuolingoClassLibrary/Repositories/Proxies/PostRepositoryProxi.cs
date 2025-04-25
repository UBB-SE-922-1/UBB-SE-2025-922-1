using DuolingoClassLibrary.Entities;
using DuolingoClassLibrary.Repositories.Interfaces;
using System.Text.Json;
using DuolingoClassLibrary.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace DuolingoClassLibrary.Repositories.Proxies
{
    public class PostRepositoryProxi : IPostRepository, IDisposable
    {
        private readonly HttpClient _httpClient;

        public PostRepositoryProxi()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<Post>> GetPosts()
        {
            try
            {
                var response = await _httpClient.GetAsync(Enviroment.BaseUrl + "post");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to fetch posts. Status code: {response.StatusCode}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<Post>>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new List<Post>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting posts: {ex.Message}");
                return new List<Post>();
            }
        }

        public async Task<int> CreatePost(Post post)
        {
            try
            {
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(post),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(Enviroment.BaseUrl + "post", jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to create post. Status code: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var createdId = JsonSerializer.Deserialize<int>(content);
                return createdId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating post: {ex.Message}");
                return -1;
            }
        }

        public async Task DeletePost(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(Enviroment.BaseUrl + $"post/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to delete post. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting post: {ex.Message}");
            }
        }

        public async Task UpdatePost(Post post)
        {
            try
            {
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(post),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PutAsync(Enviroment.BaseUrl + $"post/{post.Id}", jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to update post. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating post: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
} 