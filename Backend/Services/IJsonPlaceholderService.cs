using Backend.Models;

namespace Backend.Services
{
    public interface IJsonPlaceholderService
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<IEnumerable<Todo>> GetTodosByUserIdAsync(int userId);
    }
}
