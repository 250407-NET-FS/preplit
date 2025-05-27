using Preplit.Domain;
using Preplit.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Preplit.Services.Users.Commands
{
    public class DeleteUser
    {
        public class Command : IRequest
        {
            public Guid? UserId { get; set; }
        }

        public class Handler(UserManager<User> userManager) : IRequestHandler<Command>
        {
            public async Task Handle(Command request, CancellationToken ct)
            {
                User? userToDelete = await userManager.FindByIdAsync(request.UserId.ToString()!) ?? throw new Exception("User not found");
                
                IdentityResult result = await userManager.DeleteAsync(userToDelete);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to delete user");
                }                
            }
        }
    }
}