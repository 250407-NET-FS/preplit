using Preplit.Domain;
using Preplit.Data;
using MediatR;
using AutoMapper;
using Preplit.Domain.DTOs;

namespace Preplit.Services.Cards.Commands
{
    public class EditCard
    {
        public class Command : IRequest {
            public required CardUpdateDTO CardInfo { get; set; }
            public required Guid UserId { get; set; }
        }

        public class Handler(PreplitContext context, IMapper mapper) : IRequestHandler<Command> {
            public async Task Handle(Command request, CancellationToken ct)
            {
                var card = await context.Cards.FindAsync(request.CardInfo.CardId, ct) ?? throw new NullReferenceException("Card not found");
                if (card.UserId != request.UserId)
                {
                    throw new UnauthorizedAccessException("Unauthorized");
                }
                mapper.Map(request.CardInfo, card);
                int res = await context.SaveChangesAsync(ct);

                if (res < 1)
                {
                    throw new Exception("Failed to update card");
                }
            }
        }
    }
}