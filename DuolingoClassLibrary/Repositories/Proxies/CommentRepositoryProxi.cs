using DuolingoClassLibrary.Entities;
using DuolingoClassLibrary.Repositories.Interfaces;
using System.Text.Json;
using DuolingoClassLibrary.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace DuolingoClassLibrary.Repositories.Proxies
{
    public class CommentRepositoryProxi : ICommentRepository, IDisposable
    {
        private readonly HttpClient _httpClient;

        public CommentRepositoryProxi()
        {
            _httpClient = new HttpClient();
        }

        public List<Comment> GetCommentsByPostId(int postId)
        {
            try
            {
                var response = _httpClient.GetAsync(Enviroment.BaseUrl + $"comment/post/{postId}").Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to fetch comments. Status code: {response.StatusCode}");
                }

                var jsonResponse = response.Content.ReadAsStringAsync().Result;
                var result = JsonSerializer.Deserialize<List<Comment>>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new List<Comment>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting comments: {ex.Message}");
                return new List<Comment>();
            }
        }

        public Comment GetCommentById(int commentId)
        {
            try
            {
                var response = _httpClient.GetAsync(Enviroment.BaseUrl + $"comment/{commentId}").Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to fetch comment. Status code: {response.StatusCode}");
                }

                var jsonResponse = response.Content.ReadAsStringAsync().Result;
                var result = JsonSerializer.Deserialize<Comment>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting comment: {ex.Message}");
                return null;
            }
        }

        public List<Comment> GetRepliesByCommentId(int parentCommentId)
        {
            try
            {
                var response = _httpClient.GetAsync(Enviroment.BaseUrl + $"comment/replies/{parentCommentId}").Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to fetch replies. Status code: {response.StatusCode}");
                }

                var jsonResponse = response.Content.ReadAsStringAsync().Result;
                var result = JsonSerializer.Deserialize<List<Comment>>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new List<Comment>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting replies: {ex.Message}");
                return new List<Comment>();
            }
        }

        public int CreateComment(Comment comment)
        {
            try
            {
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(comment),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = _httpClient.PostAsync(Enviroment.BaseUrl + "comment", jsonContent).Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to create comment. Status code: {response.StatusCode}");
                }

                var content = response.Content.ReadAsStringAsync().Result;
                var createdId = JsonSerializer.Deserialize<int>(content);
                return createdId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating comment: {ex.Message}");
                return -1;
            }
        }

        public bool DeleteComment(int commentId)
        {
            try
            {
                var response = _httpClient.DeleteAsync(Enviroment.BaseUrl + $"comment/{commentId}").Result;

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting comment: {ex.Message}");
                return false;
            }
        }

        public bool IncrementLikeCount(int commentId)
        {
            try
            {
                var response = _httpClient.PutAsync(Enviroment.BaseUrl + $"comment/{commentId}/like", null).Result;

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error liking comment: {ex.Message}");
                return false;
            }
        }

        public int GetCommentsCountForPost(int postId)
        {
            try
            {
                var response = _httpClient.GetAsync(Enviroment.BaseUrl + $"comment/count/{postId}").Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to fetch comment count. Status code: {response.StatusCode}");
                }

                var content = response.Content.ReadAsStringAsync().Result;
                var count = JsonSerializer.Deserialize<int>(content);
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting comment count: {ex.Message}");
                return 0;
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
} 