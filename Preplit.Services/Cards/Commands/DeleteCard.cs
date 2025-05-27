using Preplit.Data;
using MediatR;

namespace Preplit.Services.Cards.Commands
{
    public class DeleteCard
    {
        public class Command : IRequest
        {
            public required Guid Id { get; set; }
            public required Guid UserId { get; set; }
        }

        public class Handler(PreplitContext context) : IRequestHandler<Command>
        {
            public async Task Handle(Command request, CancellationToken ct)
            {
                var card = await context.Cards.FindAsync([request.Id, ct], cancellationToken: ct) ?? throw new NullReferenceException("Cannot find card");
                if (card.UserId != request.UserId)
                {
                    throw new ArgumentException("Unauthorized");
                }
                context.Remove(card);
                int res = await context.SaveChangesAsync(ct);
                if (res < 1)
                {
                    throw new Exception("Failed to delete card");
                }
            }
        }
    }
}