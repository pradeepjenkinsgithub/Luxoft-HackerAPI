//using Microsoft.AspNetCore.Mvc;
//using System.Net.Http;
//using System.Text.Json;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Linq;

//[ApiController]
//[Route("api/[controller]")]
//public class HackerNewsController : ControllerBase
//{
//    private readonly HttpClient _httpClient;

//    public HackerNewsController(HttpClient httpClient)
//    {
//        _httpClient = httpClient;
//    }

//    [HttpGet("beststories")]
//    public async Task<IActionResult> GetBestStories([FromQuery] int n = 42845091)
//    {
//        const string bestStoriesUrl = "https://hacker-news.firebaseio.com/v0/beststories.json";
//        var storyIds = await _httpClient.GetFromJsonAsync<List<int>>(bestStoriesUrl);

//        if (storyIds == null || !storyIds.Any())
//        {
//            return NotFound("No stories found.");
//        }

//        var topStories = await GetTopStories(storyIds, n);
//        return Ok(topStories);
//    }

//    private async Task<List<Story>> GetTopStories(List<int> storyIds, int n)
//    {
//        var tasks = storyIds.Take(n * 2).Select(id => GetStoryById(id)); // Fetch more in case some are null
//        var stories = await Task.WhenAll(tasks);

//        return stories.Where(s => s != null)
//                      .OrderByDescending(s => s.Score)
//                      .Take(n)
//                      .ToList();
//    }

//    private async Task<Story?> GetStoryById(int id)
//    {
//        string storyUrl = $"https://hacker-news.firebaseio.com/v0/item/{id}.json";
//        return await _httpClient.GetFromJsonAsync<Story>(storyUrl);
//    }
//}

//public class Story
//{
//    public string Title { get; set; } = string.Empty;
//    public string Url { get; set; } = string.Empty;
//    public int Score { get; set; }
//    public string By { get; set; } = string.Empty;
//    public int Time { get; set; }
//}


using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class HackerNewsController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HackerNewsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("beststories")]
    public async Task<IActionResult> GetBestStories([FromQuery] int n = 42796496)
    {
        const string bestStoriesUrl = "https://hacker-news.firebaseio.com/v0/beststories.json";
        var httpClient = _httpClientFactory.CreateClient();
        var storyIds = await httpClient.GetFromJsonAsync<List<int>>(bestStoriesUrl);

        if (storyIds == null || !storyIds.Any())
        {
            return NotFound("No stories found.");
        }

        var topStories = await GetTopStories(httpClient, storyIds, n);
        return Ok(topStories);
    }

    private async Task<List<Story>> GetTopStories(HttpClient httpClient, List<int> storyIds, int n)
    {
        var tasks = storyIds.Take(n * 2).Select(id => GetStoryById(httpClient, id));
        var stories = await Task.WhenAll(tasks);

        return stories.Where(s => s != null)
                      .OrderByDescending(s => s.Score)
                      .Take(n)
                      .ToList();
    }

    private async Task<Story?> GetStoryById(HttpClient httpClient, int id)
    {
        string storyUrl = $"https://hacker-news.firebaseio.com/v0/item/{id}.json";
        return await httpClient.GetFromJsonAsync<Story>(storyUrl);
    }
}

public class Story
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int Score { get; set; }
    public string By { get; set; } = string.Empty;
    public int Time { get; set; }
}
