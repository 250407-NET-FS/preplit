using Preplit.Domain;
using Preplit.Data;
using MediatR;
using AutoMapper;

namespace Preplit.Services.Categories.Commands
{
    public class EditCategory
    {
        public class Command : IRequest {
            public required Category Category { get; set; }
        }

        public class Handler(PreplitContext context, IMapper mapper) : IRequestHandler<Command> {
            public async Task Handle(Command request, CancellationToken ct)
            {
                var category = await context.Categories.FindAsync([request.Category.CategoryId, ct], cancellationToken: ct);
                mapper.Map(request.Category, category);
                await context.SaveChangesAsync(ct);
            }
        }
    }
}