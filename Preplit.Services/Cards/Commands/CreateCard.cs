using Preplit.Domain;
using Preplit.Data;
using MediatR;

namespace Preplit.Services.Cards.Commands
{
    public class CreateCard
    {
        public class Command : IRequest<Card>
        {
            public required Card Card { get; set; }
        }

        public class Handler(PreplitContext context) : IRequestHandler<Command, Card>
        {
            public async Task<Card> Handle(Command request, CancellationToken ct)
            {
                context.Cards.Add(request.Card);
                await context.SaveChangesAsync(ct);
                return request.Card;
            }
        }
    }
}