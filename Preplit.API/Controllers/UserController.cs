using Preplit.Domain;
using Preplit.Domain.DTOs;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Preplit.API;

// We need to designate this as an API Controller
// And we should probably set a top level route
// hint: If you use the [EntityName]Controller convention, we can essentially
// parameterize the route name
[ApiController]
[Route("api/user")]

public class UserController : ControllerBase
{


    private readonly UserManager<User> _userManager;

    public UserController(UserManager<User> userManager)
    {

        _userManager = userManager;
    }

    // Get: api/admin/user
    // Endpoint to retrieve all Users Admin Only
    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        try
        {
            return Ok(await _userManager.Users.ToListAsync());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // Get: api/admin/user/id/{id}
    // Get user by id Admin Only
    [Authorize(Roles = "Admin")]
    [Authorize(Roles = "Admin")]
    [HttpGet("admin/{id}")]
    public async Task<ActionResult<UserDTO>> GetUserByAdminId([FromRoute] Guid id)
    {
        try
        {
            return Ok(await _userManager.FindByIdAsync(id.ToString()));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // Delete: api/admin/user/{id}/
    // Delete user by id Admin Only
    [Authorize(Roles = "Admin")]
    [Authorize(Roles = "Admin")]
    [HttpDelete("admin/{id}")]
    public async Task<ActionResult<bool>> DeleteUserById([FromRoute] Guid id)
    {
        try
        {
            User? user = await _userManager.FindByIdAsync(id.ToString());
            await _userManager.DeleteAsync(user);
            return Ok(true);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // Get: api/user
    // Get user profile owner only
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<UserDTO>> GetUserById()
    {
        try
        {
            User? user = await GetCurrentUserAsync();
            return Ok(await _userManager.FindByIdAsync(user?.Id.ToString()));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    private async Task<User?> GetCurrentUserAsync()
    {
        return await _userManager.GetUserAsync(HttpContext.User);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("admin/ban/{id}")]
    public async Task<IActionResult> OnPostBanAsync([FromRoute] Guid id)
    {
        try
        {
            User user = await _userManager.FindByIdAsync(id.ToString());

            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

            return Ok(await _userManager.FindByIdAsync(user?.Id.ToString()));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }


    }
    [Authorize(Roles = "Admin")]
    [HttpPost("admin/unban/{id}")]
    public async Task<IActionResult> OnPostUnBanAsync([FromRoute] Guid id)
    {
        try
        {
            User user = await _userManager.FindByIdAsync(id.ToString());
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
            await _userManager.SetLockoutEnabledAsync(user, false);

            return Ok(await _userManager.FindByIdAsync(user?.Id.ToString()));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

    }

}