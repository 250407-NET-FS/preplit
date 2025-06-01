using Preplit.Domain;
using Preplit.Domain.DTOs;
using Preplit.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Preplit.Services.Cards.Queries
{
    public class GetCardDetails
    {
        public class Query : IRequest<CardResponseDTO>
        {
            public required Guid Id { get; set; }
        }

        public class Handler(PreplitContext context) : IRequestHandler<Query, CardResponseDTO>
        {
            public async Task<CardResponseDTO> Handle(Query request, CancellationToken ct)
            {
                Card card = await context.Cards.FindAsync(request.Id, ct) ?? throw new NullReferenceException("Card not found!");
                return new CardResponseDTO(card);
            }
        }
    }
}