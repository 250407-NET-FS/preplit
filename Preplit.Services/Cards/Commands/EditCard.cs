using Preplit.Domain;
using Preplit.Data;
using MediatR;
using AutoMapper;

namespace Preplit.Services.Categories.Commands
{
    public class EditCard
    {
        public class Command : IRequest {
            public required Card Card { get; set; }
        }

        public class Handler(PreplitContext context, IMapper mapper) : IRequestHandler<Command> {
            public async Task Handle(Command request, CancellationToken ct)
            {
                var card = await context.Cards.FindAsync([request.Card.CardId, ct], cancellationToken: ct);
                mapper.Map(request.Card, card);
                await context.SaveChangesAsync(ct);
            }
        }
    }
}