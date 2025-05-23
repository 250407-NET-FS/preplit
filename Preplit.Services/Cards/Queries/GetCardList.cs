using Preplit.Domain;
using Preplit.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Preplit.Services.Cards.Queries
{
    public class GetCardList
    {
        public class Query : IRequest<List<Card>> { }

        public class Handler(PreplitContext context) : IRequestHandler<Query, List<Card>>
        {
            public async Task<List<Card>> Handle(Query request, CancellationToken ct) {
                return await context.Cards.ToListAsync(ct);
            }
        }
    }
}