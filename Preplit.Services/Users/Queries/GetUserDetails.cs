using Microsoft.AspNetCore.Identity;
using Preplit.Domain;
using MediatR;

namespace Preplit.Services.Users.Queries
{
    public class GetUserDetails
    {
        public class Query : IRequest<User>
        {
            public Guid? UserId { get; set; }
        }

        public class Handler(UserManager<User> userManager) : IRequestHandler<Query, User>
        {
            public async Task<User> Handle(Query request, CancellationToken ct)
            {
                return await userManager.FindByIdAsync(request.UserId.ToString()!) ?? throw new Exception("User not found!");
            }
        }
    }
}