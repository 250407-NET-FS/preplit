using Preplit.Domain;
using Preplit.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Preplit.Domain.DTOs;

namespace Preplit.Services.Categories.Queries
{
    public class GetUserCategoryList
    {
        public class Query : IRequest<IEnumerable<CategoryResponseDTO>>
        { 
            public required Guid UserId { get; set; }
        }

        public class Handler(PreplitContext context) : IRequestHandler<Query, IEnumerable<CategoryResponseDTO>>
        {
            public async Task<IEnumerable<CategoryResponseDTO>> Handle(Query request, CancellationToken ct)
            {
                IEnumerable<Category> categoryList = await context.Categories.Where(c => c.UserId == request.UserId).ToListAsync(ct);
                return categoryList.Select(c => new CategoryResponseDTO(c));
            }
        }
    }
}