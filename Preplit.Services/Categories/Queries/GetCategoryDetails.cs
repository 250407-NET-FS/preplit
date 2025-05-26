using Preplit.Domain;
using Preplit.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Preplit.Domain.DTOs;

namespace Preplit.Services.Categories.Queries
{
    public class GetCategoryDetails
    {
        public class Query : IRequest<CategoryResponseDTO>
        {
            public required Guid Id { get; set; }
        }

        public class Handler(PreplitContext context) : IRequestHandler<Query, CategoryResponseDTO>
        {
            public async Task<CategoryResponseDTO> Handle(Query request, CancellationToken ct)
            {
                Category category = await context.Categories.FindAsync([request.Id, ct], cancellationToken: ct) ?? throw new NullReferenceException("Category not found!");
                return new CategoryResponseDTO(category);
            }
        }
    }
}