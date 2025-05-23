using Preplit.Domain;
using Preplit.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Preplit.Services.Categories.Queries
{
    public class GetCategoryDetails
    {
        public class Query : IRequest<Category>
        {
            public required Guid Id { get; set; }
        }

        public class Handler(PreplitContext context) : IRequestHandler<Query, Category>
        {
            public async Task<Category> Handle(Query request, CancellationToken ct)
            {
                return await context.Categories.FindAsync([request.Id, ct], cancellationToken: ct) ?? throw new NullReferenceException("Category not found!");
            }
        }
    }
}