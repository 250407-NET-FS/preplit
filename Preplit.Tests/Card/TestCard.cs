using Xunit;
using Preplit.Domain;
using Preplit.Services.Cards.Queries;
using Preplit.Services.Cards.Commands;
using Preplit.Data;
using Preplit.Domain.DTOs;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Preplit.API;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Preplit.Tests
{
    public class TestCard
    {
        private readonly Mock<UserManager<User>> _userManager;

        public TestCard()
        {
            #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _userManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null
            );
            #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            
        }

        [Fact]
        public async Task GetCardsAdmin_ShouldReturnAllCards()
        {
            List<Card> expectedCards =
            [
                SharedObjects.CloneValidCard1(),
                SharedObjects.CloneValidCard2(),
                SharedObjects.CloneValidCard3()
            ];
            var expectedCardResponses = expectedCards.Select(c => new CardResponseDTO(c));

            var options = new DbContextOptionsBuilder<PreplitContext>()
                .UseInMemoryDatabase(databaseName: "PreplitTestDB")
                .Options;
            // Insert the test data
            using (var context = new PreplitContext(options))
            {
                context.Cards.AddRange(expectedCards);
                context.SaveChanges();
            }

            // Use a clean instance of the context to assert the result
            using (var context = new PreplitContext(options))
            {
                var handler = new GetCardList.Handler(context);
                var result = await handler.Handle(new GetCardList.Query(), CancellationToken.None);
                Assert.NotNull(result);
            }
        }
    }
}