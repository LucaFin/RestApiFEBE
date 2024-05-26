using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IJsonPlaceholderService _jsonPlaceholderService;

        public UsersController(IJsonPlaceholderService jsonPlaceholderService)
        {
            _jsonPlaceholderService = jsonPlaceholderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(int limit = 10, int offset = 0)
        {
            Log.Information("GetUsers called with limit: {Limit}, offset: {Offset}", limit, offset);

            if (limit <= 0 || offset < 0)
            {
                Log.Warning("Invalid limit or offset. Limit: {Limit}, Offset: {Offset}", limit, offset);
                return BadRequest();
            }

            try
            {
                var users = await _jsonPlaceholderService.GetUsersAsync();
                var totalCount = users.Count();
                var paginatedUsers = users.Skip(offset).Take(limit);
                Log.Information("Returning {Count} users", paginatedUsers.Count());
                return Ok(new PaginatedResponse<User> { TotalCount = totalCount, Items = paginatedUsers });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while getting users");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
