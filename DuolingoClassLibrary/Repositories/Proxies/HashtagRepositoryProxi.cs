using DuolingoClassLibrary.Entities;
using DuolingoClassLibrary.Repositories.Interfaces;
using System.Text.Json;
using DuolingoClassLibrary.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace DuolingoClassLibrary.Repositories.Proxies
{
    public class HashtagRepositoryProxi : IHashtagRepository, IDisposable
    {
        private readonly HttpClient _httpClient;

        public HashtagRepositoryProxi()
        {
            _httpClient = new HttpClient();
        }
        
        public async Task<Hashtag> GetHashtagByText(string textToSearchBy)
        {
            try
            {
                var response = await _httpClient.GetAsync(Enviroment.BaseUrl + $"hashtag/text/{textToSearchBy}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to fetch hashtag. Status code: {response.StatusCode}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Hashtag>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting hashtag by text: {ex.Message}");
                return null;
            }
        }
        
        public async Task<Hashtag> CreateHashtag(string newHashtagTag)
        {
            try
            {
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(new { Tag = newHashtagTag }),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(Enviroment.BaseUrl + "hashtag", jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to create hashtag. Status code: {response.StatusCode}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Hashtag>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating hashtag: {ex.Message}");
                throw;
            }
        }
        
        public async Task<List<Hashtag>> GetHashtagsByPostId(int postId)
        {
            try
            {
                var response = await _httpClient.GetAsync(Enviroment.BaseUrl + $"hashtag/post/{postId}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to fetch hashtags for post. Status code: {response.StatusCode}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<Hashtag>>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new List<Hashtag>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting hashtags for post: {ex.Message}");
                return new List<Hashtag>();
            }
        }
        
        public async Task<bool> AddHashtagToPost(int postId, int hashtagId)
        {
            try
            {
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(new { PostId = postId, HashtagId = hashtagId }),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(Enviroment.BaseUrl + "hashtag/post", jsonContent);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding hashtag to post: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> RemoveHashtagFromPost(int postId, int hashtagId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(Enviroment.BaseUrl + $"hashtag/post/{postId}/{hashtagId}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing hashtag from post: {ex.Message}");
                return false;
            }
        }
        
        public async Task<Hashtag> GetHashtagByName(string hashtagName)
        {
            return await GetHashtagByText(hashtagName);
        }
        
        public async Task<List<Hashtag>> GetAllHashtags()
        {
            try
            {
                var response = await _httpClient.GetAsync(Enviroment.BaseUrl + "hashtag");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to fetch hashtags. Status code: {response.StatusCode}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<Hashtag>>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new List<Hashtag>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all hashtags: {ex.Message}");
                return new List<Hashtag>();
            }
        }
        
        public async Task<List<Hashtag>> GetHashtagsByCategory(int categoryId)
        {
            try
            {
                var response = await _httpClient.GetAsync(Enviroment.BaseUrl + $"hashtag/category/{categoryId}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to fetch hashtags for category. Status code: {response.StatusCode}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<Hashtag>>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new List<Hashtag>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting hashtags for category: {ex.Message}");
                return new List<Hashtag>();
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
} 