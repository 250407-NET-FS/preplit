using Preplit.Domain;
using Preplit.Data;
using MediatR;
using Preplit.Domain.DTOs;

namespace Preplit.Services.Categories.Commands
{
    public class CreateCategory
    {
        public class Command : IRequest<Guid>
        {
            public required CategoryAddDTO CategoryInfo { get; set; }
        }

        public class Handler(PreplitContext context) : IRequestHandler<Command, Guid>
        {
            public async Task<Guid> Handle(Command request, CancellationToken ct)
            {
                Category newCategory = new(request.CategoryInfo.Name, request.CategoryInfo.UserId);
                context.Categories.Add(newCategory);
                int res = await context.SaveChangesAsync(ct);
                
                if (res < 1)
                {
                    throw new Exception("Failed to insert category");
                }

                return newCategory.CategoryId;
            }
        }
    }
}