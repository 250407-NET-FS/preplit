using Preplit.Domain;
using Preplit.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Preplit.Services.Cards.Queries
{
    public class GetCardDetails
    {
        public class Query : IRequest<Card>
        {
            public required Guid Id { get; set; }
        }

        public class Handler(PreplitContext context) : IRequestHandler<Query, Card>
        {
            public async Task<Card> Handle(Query request, CancellationToken ct)
            {
                return await context.Cards.FindAsync([request.Id, ct], cancellationToken: ct) ?? throw new NullReferenceException("Card not found!");
            }
        }
    }
}