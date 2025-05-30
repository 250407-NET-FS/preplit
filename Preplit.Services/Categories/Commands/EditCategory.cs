using Preplit.Domain;
using Preplit.Data;
using MediatR;
using AutoMapper;
using Preplit.Domain.DTOs;

namespace Preplit.Services.Categories.Commands
{
    public class EditCategory
    {
        public class Command : IRequest
        {
            public required CategoryUpdateDTO CategoryInfo { get; set; }
            public required Guid UserId { get; set; }
        }

        public class Handler(PreplitContext context, IMapper mapper) : IRequestHandler<Command> {
            public async Task Handle(Command request, CancellationToken ct)
            {
                var category = await context.Categories.FindAsync([request.CategoryInfo.CategoryId, ct], cancellationToken: ct) ?? throw new NullReferenceException("Category not found");
                if (category.UserId != request.UserId)
                {
                    throw new UnauthorizedAccessException("Unauthorized");
                }

                mapper.Map(request.CategoryInfo, category);
                int res = await context.SaveChangesAsync(ct);
                if (res < 1)
                {
                    throw new Exception("Failed to update category");
                }
            }
        }
    }
}