using System.Net.Http;
using System.Text.Json;
using Backend.Models;

namespace Backend.Services
{
    public class JsonPlaceholderService : IJsonPlaceholderService
    {
        private readonly HttpClient _httpClient;

        public JsonPlaceholderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var response = await _httpClient.GetStringAsync("https://jsonplaceholder.typicode.com/users");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<IEnumerable<User>>(response, options) ?? new List<User>();
        }

        public async Task<IEnumerable<Todo>> GetTodosByUserIdAsync(int userId)
        {
            var response = await _httpClient.GetStringAsync($"https://jsonplaceholder.typicode.com/todos?userId={userId}");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<IEnumerable<Todo>>(response, options) ?? new List<Todo>();
        }
    }
}
