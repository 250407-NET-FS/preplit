using Preplit.Domain;
using Preplit.Domain.DTOs;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Preplit.Services.Users.Queries;
using Preplit.Services.Users.Commands;

namespace Preplit.API
{

    // We need to designate this as an API Controller
    // And we should probably set a top level route
    // hint: If you use the [EntityName]Controller convention, we can essentially
    // parameterize the route name
    [ApiController]
    [Route("api/user")]

    public class UserController(UserManager<User> userManager) : ApiController
    {


        private readonly UserManager<User> _userManager = userManager;

        // Get: api/admin/user
        // Endpoint to retrieve all Users Admin Only
        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers(CancellationToken ct)
        {
            try
            {
                return Ok(await Mediator.Send(new GetUserList.Query(), ct));
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
        public async Task<ActionResult<UserDTO>> GetUserByAdminId([FromRoute] Guid id, CancellationToken ct)
        {
            try
            {
                return Ok(await Mediator.Send(new GetUserDetails.Query { UserId = id }, ct));
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
        public async Task<ActionResult<bool>> DeleteUserById([FromRoute] Guid id, CancellationToken ct)
        {
            try
            {
                await Mediator.Send(new DeleteUser.Command { UserId = id }, ct);
                return Ok();
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
        public async Task<ActionResult<UserDTO>> GetUserById(CancellationToken ct)
        {
            try
            {
                User? user = await GetCurrentUserAsync(ct);
                return Ok(await Mediator.Send(new GetUserDetails.Query { UserId = user?.Id }, ct));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        private async Task<User?> GetCurrentUserAsync(CancellationToken ct)
        {
            return await Mediator.Send(new GetLoggedUser.Query { User = HttpContext.User }, ct);
        }
        /**
          * Post: api/admin/ban/{id}
          * Endpoint to ban a user based on its specific id
        */
        [Authorize(Roles = "Admin")]
        [HttpPost("admin/ban/{id}")]
        public async Task<IActionResult> OnPostBanAsync([FromRoute] Guid id, CancellationToken ct)
        {
            try
            {
                User user = await Mediator.Send(new GetUserDetails.Query { UserId = id }, ct);

                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

                return Ok(await Mediator.Send(new GetUserDetails.Query { UserId = user?.Id }, ct));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        /**
          * Post: api/admin/unban/{id}
          * Enpoint to unban a user based on its specific id
        */
        [Authorize(Roles = "Admin")]
        [HttpPost("admin/unban/{id}")]
        public async Task<IActionResult> OnPostUnBanAsync([FromRoute] Guid id, CancellationToken ct)
        {
            try
            {
                User user = await Mediator.Send(new GetUserDetails.Query { UserId = id }, ct);
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                await _userManager.SetLockoutEnabledAsync(user, false);

                return Ok(await Mediator.Send(new GetUserDetails.Query { UserId = user?.Id }, ct));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

    }
}