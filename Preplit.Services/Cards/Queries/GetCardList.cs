using Preplit.Domain;
using Preplit.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Preplit.Domain.DTOs;

namespace Preplit.Services.Cards.Queries
{
    public class GetCardList
    {
        public class Query : IRequest<IEnumerable<CardResponseDTO>> { }

        public class Handler(PreplitContext context) : IRequestHandler<Query, IEnumerable<CardResponseDTO>>
        {
            public async Task<IEnumerable<CardResponseDTO>> Handle(Query request, CancellationToken ct)
            {
                IEnumerable<Card> cardList = await context.Cards.ToListAsync(ct);
                return cardList.Select(c => new CardResponseDTO(c));
            }
        }
    }
}