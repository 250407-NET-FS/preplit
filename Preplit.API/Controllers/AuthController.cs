using Preplit.Domain;
using Preplit.Services.Users.Queries;
using Preplit.Services.Users.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Preplit.Domain.DTOs;

namespace Preplit.API
{
    // We need to designate this as an API Controller
    // And we should probably set a top level route
    // hint: If you use the [EntityName]Controller convention, we can essentially
    // parameterize the route name
    [ApiController]
    [Route("api/auth")]
    public class AuthUserController(UserManager<User> userManager) : ApiController
    {
        private readonly UserManager<User> _userManager = userManager;
        /**
          * Post: api/auth/register
          * Enpoint to register (create) a user using registration details
        */
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto newUser)
        {
            //If the LoginDto doesnt conform to our model binding rules, just kick it back
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Using the DTO's info to construct a new User model object to stick
            //into the database
            var user = new User
            {
                UserName = newUser.Email,
                Email = newUser.Email,
                FullName = newUser.FullName!
            };

            //We are going to attempt to add the user to the db, if they are added we will return
            //an Ok with a success message
            //If not, we return some error
            var result = await _userManager.CreateAsync(user, newUser.Password!);

            // result above is of type IdentityResult
            // It contains info about an AspNetCore.Identity related database operation
            //In this case, did we succeed in creating a new user record on our database
            //If not, we run the code below. If we did succeed, we just return the Ok with a message.
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { message = "Registration Successful" });
        }
        /**
          * Post: api/auth/login
          * Endpoint to authenticate a user a generate a user token using login details
        */
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto existingUser, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByEmailAsync(existingUser.Email!);

            if (user is null || !await _userManager.CheckPasswordAsync(user, existingUser.Password!))
            {
                return Unauthorized("Invalid Credentials");
            }
            var token = await Mediator.Send(new GenerateToken.Command { User = user }, ct);
            return Ok(new { token });
        }
    }
}