using Preplit.Data;
using MediatR;

namespace Preplit.Services.Categories.Commands
{
    public class Command : IRequest
    {
        public required Guid Id { get; set; }
    }

    public class Handler(PreplitContext context) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken ct)
        {
            var category = await context.Categories.FindAsync([request.Id, ct], cancellationToken: ct) ?? throw new NullReferenceException("Cannot find category");
            context.Remove(category);
            await context.SaveChangesAsync(ct);
        }
    }
}