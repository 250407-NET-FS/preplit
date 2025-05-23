using Preplit.Domain;
using Preplit.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Preplit.Services.Categories.Queries
{
    public class GetCategoryList
    {
        public class Query : IRequest<List<Category>> { }

        public class Handler(PreplitContext context) : IRequestHandler<Query, List<Category>>
        {
            public async Task<List<Category>> Handle(Query request, CancellationToken ct) {
                return await context.Categories.ToListAsync(ct);
            }
        }
    }
}