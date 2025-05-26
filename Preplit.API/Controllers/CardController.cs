using Preplit.Domain;
using Preplit.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Preplit.Services.Cards.Queries;
using Preplit.Services.Cards.Commands;
using Preplit.Services.Categories.Commands;

namespace Preplit.API
{
    [ApiController]
    [Route("api/cards")]
    public class CardController(UserManager<User> userManager) : ApiController
    {
        private readonly UserManager<User> _userManager = userManager;

        /**
          * Get: api/cards
          * Endoint to retrieve all cards Admin Only
        */
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CardResponseDTO>>> GetCardsAdmin(CancellationToken ct)
        {
            try
            {
                return Ok(await Mediator.Send(new GetCardList.Query(), ct));
            }
            catch (Exception err)
            {
                return NotFound(err.Message);
            }
        }
        /**
          * Get: api/cards/{id}
          * Endoint to retrieve a specific card based on its id
        */
        [HttpGet("{id}")]
        public async Task<ActionResult<CardResponseDTO>> GetCardById(Guid id, CancellationToken ct) {
            try
            {
                return Ok(await Mediator.Send(new GetCardDetails.Query { Id = id }, ct));
            }
            catch (Exception err)
            {
                return NotFound(err.Message);
            }
        }
        /**
          * Post: api/cards
          * Endoint to create a card using the request body's dto
        */
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateCard([FromBody] CardAddDTO cardInfo, CancellationToken ct) {
            try
            {
                // Explicitly check the model state to make sure the dto comforms with the request
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(await Mediator.Send(new CreateCard.Command { CardInfo = cardInfo }, ct));
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }
        /**
          * Put: api/cards
          * Endpoint to update card attributes based on what is not null
        */
        [Authorize]
        [HttpPut]
        public async Task<ActionResult> UpdateCard([FromBody] CardUpdateDTO cardInfo, CancellationToken ct)
        {
            try
            {
                User? user = await GetCurrentUserAsync(); // owner only
                await Mediator.Send(new EditCard.Command { CardInfo = cardInfo, UserId = user!.Id }, ct);
                return NoContent();
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }
        /**
          * Delete: api/cards/{id}
          * Endpoint to delete a card by its id
        */
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCard([FromRoute] Guid id, CancellationToken ct)
        {
            try
            {
                User? user = await GetCurrentUserAsync(); // owner only
                await Mediator.Send(new DeleteCard.Command { Id = id, UserId = user!.Id }, ct);
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