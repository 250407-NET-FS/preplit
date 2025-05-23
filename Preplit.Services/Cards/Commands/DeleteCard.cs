using Preplit.Data;
using MediatR;

namespace Preplit.Services.Cards.Commands
{
    public class Command : IRequest
    {
        public required Guid Id { get; set; }
    }

    public class Handler(PreplitContext context) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken ct)
        {
            var card = await context.Cards.FindAsync([request.Id, ct], cancellationToken: ct) ?? throw new NullReferenceException("Cannot find card");
            context.Remove(card);
            await context.SaveChangesAsync(ct);
        }
    }
}