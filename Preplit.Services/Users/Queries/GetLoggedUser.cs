using Microsoft.AspNetCore.Identity;
using Preplit.Domain;
using MediatR;
using System.Security.Claims;

namespace Preplit.Services.Users.Queries
{
    public class GetLoggedUser
    {
        public class Query : IRequest<User>
        {
            public required ClaimsPrincipal User { get; set; }
        }

        public class Handler(UserManager<User> userManager) : IRequestHandler<Query, User>
        {
            public async Task<User> Handle(Query request, CancellationToken ct)
            {
                return (await userManager.GetUserAsync(request.User))!;
            }
        }
    }
}