using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Preplit.Domain;
using MediatR;

namespace Preplit.Services.Users.Queries
{
    public class GetUserList
    {
        public class Query : IRequest<List<User>> { }

        public class Handler(UserManager<User> userManager) : IRequestHandler<Query, List<User>>
        {
            public async Task<List<User>> Handle(Query request, CancellationToken ct)
            {
                return await userManager.Users.ToListAsync(ct);                
            }
        }
    }
}