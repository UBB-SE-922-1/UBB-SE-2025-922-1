using Server.Entities;
using Server.Repositories.Interfaces;
using System.Text.Json;

public class CategoryRepositoryProxi : ICategoryRepository, IDisposable
{
    private readonly HttpClient _httpClient;

    public CategoryRepositoryProxi()
    {
        _httpClient = new HttpClient();
    }

    public List<Category> GetCategories()
    {
        var response = _httpClient.GetAsync("https://localhost:7160/category").Result;

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to fetch categories. Status code: {response.StatusCode}");
        }

        var jsonResponse = response.Content.ReadAsStringAsync().Result;
        var result = JsonSerializer.Deserialize<List<Category>>(jsonResponse) ?? new List<Category>();

        return result;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}