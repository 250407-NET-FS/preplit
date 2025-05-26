using Preplit.Domain;
using Preplit.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Preplit.Services.Categories.Queries;
using Preplit.Services.Categories.Commands;

namespace Preplit.API
{
    [ApiController]
    [Route("api/Categories")]
    public class CategoryController(UserManager<User> userManager) : ApiController
    {
        private readonly UserManager<User> _userManager = userManager;

        /**
          * Get: api/Categories
          * Endoint to retrieve all Categories Admin Only
        */
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponseDTO>>> GetCategoriesAdmin(CancellationToken ct)
        {
            try
            {
                return Ok(await Mediator.Send(new GetCategoryList.Query(), ct));
            }
            catch (Exception err)
            {
                return NotFound(err.Message);
            }
        }
        /**
          * Get: api/Categories/{id}
          * Endoint to retrieve a specific Category based on its id
        */
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponseDTO>> GetCategoryById(Guid id, CancellationToken ct) {
            try
            {
                return Ok(await Mediator.Send(new GetCategoryDetails.Query { Id = id }, ct));
            }
            catch (Exception err)
            {
                return NotFound(err.Message);
            }
        }
        /**
          * Post: api/Categories
          * Endoint to create a Category using the request body's dto
        */
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateCategory([FromBody] CategoryAddDTO CategoryInfo, CancellationToken ct) {
            try
            {
                // Explicitly check the model state to make sure the dto comforms with the request
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(await Mediator.Send(new CreateCategory.Command { CategoryInfo = CategoryInfo }, ct));
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }
        /**
          * Put: api/Categories
          * Endpoint to update Category attributes based on what is not null
        */
        [Authorize]
        [HttpPut]
        public async Task<ActionResult> UpdateCategory([FromBody] CategoryUpdateDTO CategoryInfo, CancellationToken ct)
        {
            try
            {
                User? user = await GetCurrentUserAsync(); // owner only
                await Mediator.Send(new EditCategory.Command { CategoryInfo = CategoryInfo, UserId = user!.Id }, ct);
                return NoContent();
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }
        /**
          * Delete: api/Categories/{id}
          * Endpoint to delete a Category by its id
        */
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory([FromRoute] Guid id, CancellationToken ct)
        {
            try
            {
                User? user = await GetCurrentUserAsync(); // owner only
                await Mediator.Send(new DeleteCategory.Command { Id = id, UserId = user!.Id }, ct);
                return Ok();
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        private async Task<User?> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }
    }
}