using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Preplit.Domain;
using MediatR;

namespace Preplit.Services.Users.Queries
{
    public class GetUserList
    {
        public class Query : IRequest<IEnumerable<User>> { }

        public class Handler(UserManager<User> userManager) : IRequestHandler<Query, IEnumerable<User>>
        {
            public async Task<IEnumerable<User>> Handle(Query request, CancellationToken ct)
            {
                return await userManager.Users.ToListAsync(ct);                
            }
        }
    }
}