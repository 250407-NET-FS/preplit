using Preplit.Data;
using MediatR;

namespace Preplit.Services.Categories.Commands
{
    public class DeleteCategory
    {
        public class Command : IRequest
        {
            public required Guid Id { get; set; }
            public required Guid UserId { get; set; }
        }

        public class Handler(PreplitContext context) : IRequestHandler<Command>
        {
            public async Task Handle(Command request, CancellationToken ct)
            {
                var category = await context.Categories.FindAsync([request.Id, ct], cancellationToken: ct) ?? throw new NullReferenceException("Cannot find category");
                var cardList = context.Cards.Select(c => c.CategoryId == category.CategoryId);
                if (category.UserId != request.UserId)
                {
                    throw new ArgumentException("Unauthorized");
                }
                context.Remove(category);
                int res = await context.SaveChangesAsync(ct);
                if (res < 1)
                {
                    throw new Exception("Failed to delete category");
                }
                // Remove any cards belonging to the removed category
                context.Remove(cardList);
            }
        }
    }
}