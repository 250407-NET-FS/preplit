using Preplit.Domain;
using Preplit.Data;
using MediatR;
using Preplit.Domain.DTOs;
using System.Data.Common;

namespace Preplit.Services.Cards.Commands
{
    public class CreateCard
    {
        public class Command : IRequest<Guid>
        {
            public required CardAddDTO CardInfo { get; set; }
        }

        public class Handler(PreplitContext context) : IRequestHandler<Command, Guid>
        {
            public async Task<Guid> Handle(Command request, CancellationToken ct)
            {
                Card newCard = new (request.CardInfo.Question!, request.CardInfo.Answer!, request.CardInfo.CategoryId, request.CardInfo.OwnerId);
                if (await context.Categories.FindAsync(newCard.CategoryId, ct) is null)
                {
                    throw new Exception("Category does not exist");
                }
                context.Cards.Add(newCard);
                int res = await context.SaveChangesAsync(ct);
                if (res < 1)
                {
                    throw new Exception("Failed to insert card");
                }
                return newCard.CardId;
            }
        }
    }
}