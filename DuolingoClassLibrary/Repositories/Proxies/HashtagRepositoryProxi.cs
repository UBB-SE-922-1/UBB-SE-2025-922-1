﻿using DuolingoClassLibrary.Entities;
using DuolingoClassLibrary.Repositories.Interfaces;
using DuolingoClassLibrary.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace DuolingoClassLibrary.Repositories.Proxies
{
    public class HashtagRepositoryProxi : IHashtagRepository, IDisposable
    {
        private readonly HttpClient _httpClient;

        public HashtagRepositoryProxi()
        {
            _httpClient = new HttpClient();
        }
        public async Task<int> CreateHashtag(Hashtag hashtag)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(hashtag), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(Enviroment.BaseUrl + "hashtag", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to create hashtag. Status code: {response.StatusCode}");
            }

            var result = await response.Content.ReadAsStringAsync();
            return int.TryParse(result, out int hashtagId) ? hashtagId : 0;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        public async Task<List<Hashtag>> GetHashtags()
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

        public async Task AddHashtagToPost(int postId, int hashtagId)
        {
            var postHashtag = new PostHashtags { PostId = postId, HashtagId = hashtagId };
            var jsonContent = new StringContent(JsonSerializer.Serialize(postHashtag), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{Enviroment.BaseUrl}hashtag/posthashtag", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to add hashtag to post. Status code: {response.StatusCode}");
            }
        }

        public async Task RemoveHashtagFromPost(int postId, int hashtagId)
        {
            var postHashtag = new PostHashtags { PostId = postId, HashtagId = hashtagId };
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{Enviroment.BaseUrl}hashtag/posthashtag");
            request.Content = new StringContent(JsonSerializer.Serialize(postHashtag), Encoding.UTF8, "application/json");
            
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to remove hashtag from post. Status code: {response.StatusCode}");
            }
        }

        public async Task<List<PostHashtags>> GetAllPostHashtags()
        {
            var response = await _httpClient.GetAsync(Enviroment.BaseUrl + "hashtag/posthashtags");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch post hashtags. Status code: {response.StatusCode}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<PostHashtags>>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result ?? new List<PostHashtags>();
        }

    }
}
