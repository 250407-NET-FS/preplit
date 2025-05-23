using Preplit.Domain;
using Preplit.Data;
using MediatR;

namespace Preplit.Services.Categories.Commands
{
    public class CreateCategory
    {
        public class Command : IRequest<Category>
        {
            public required Category Category { get; set; }
        }

        public class Handler(PreplitContext context) : IRequestHandler<Command, Category>
        {
            public async Task<Category> Handle(Command request, CancellationToken ct)
            {
                context.Categories.Add(request.Category);
                await context.SaveChangesAsync(ct);
                return request.Category;
            }
        }
    }
}