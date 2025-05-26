using Preplit.Domain;
using Preplit.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Preplit.Domain.DTOs;

namespace Preplit.Services.Cards.Queries
{
    public class GetCategoryCardList
    {
        public class Query : IRequest<IEnumerable<CardResponseDTO>>
        { 
            public required Guid? CategoryId { get; set; }
        }

        public class Handler(PreplitContext context) : IRequestHandler<Query, IEnumerable<CardResponseDTO>>
        {
            public async Task<IEnumerable<CardResponseDTO>> Handle(Query request, CancellationToken ct)
            {
                IEnumerable<Card> cardList = await context.Cards.Where(c => c.CategoryId == request.CategoryId).ToListAsync(ct);
                return cardList.Select(c => new CardResponseDTO(c));
            }
        }
    }
}