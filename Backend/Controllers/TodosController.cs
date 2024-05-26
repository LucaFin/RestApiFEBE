using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly IJsonPlaceholderService _jsonPlaceholderService;

        public TodosController(IJsonPlaceholderService jsonPlaceholderService)
        {
            _jsonPlaceholderService = jsonPlaceholderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTodos(int userId, int limit = 10, int offset = 0)
        {
            Log.Information("GetTodos called with userId: {UserId}, limit: {Limit}, offset: {Offset}", userId, limit, offset);

            if (limit <= 0 || offset < 0)
            {
                Log.Warning("Invalid limit or offset. Limit: {Limit}, Offset: {Offset}", limit, offset);
                return BadRequest();
            }

            try
            {
                var todos = await _jsonPlaceholderService.GetTodosByUserIdAsync(userId);
                var totalCount = todos.Count();
                var paginatedTodos = todos.Skip(offset).Take(limit);
                Log.Information("Returning {Count} todos", paginatedTodos.Count());
                return Ok(new PaginatedResponse<Todo> { TotalCount = totalCount, Items = paginatedTodos });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while getting todos");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
