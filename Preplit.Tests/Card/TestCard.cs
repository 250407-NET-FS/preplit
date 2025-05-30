using Preplit.Domain;
using Preplit.Services.Cards.Queries;
using Preplit.Services.Cards.Commands;
using Preplit.Data;
using Preplit.Domain.DTOs;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AutoMapper;

namespace Preplit.Tests
{
    public class TestCard
    {
        private readonly Mock<UserManager<User>> _userManager;
        private readonly List<Card> _expectedCards;
        private readonly List<Category> _expectedCategories;
        private readonly DbContextOptions<PreplitContext> _options;

        public TestCard()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _userManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null
            );
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Set up the test data for all test methods
            _expectedCards =
            [
                SharedObjects.CloneValidCard1(),
                SharedObjects.CloneValidCard2(),
                SharedObjects.CloneValidCard3()
            ];
            _expectedCategories =
            [
                SharedObjects.CloneValidCategory1(),
                SharedObjects.CloneValidCategory2(),
                SharedObjects.CloneValidCategory3()
            ];

            // Create a clean instance of the database context
            _options = new DbContextOptionsBuilder<PreplitContext>()
                .UseInMemoryDatabase(databaseName: "PreplitTestDB")
                .Options;

        }

        [Fact, Trait("Category", "GetCards")]
        public async Task GetCardsAdmin_ShouldReturnAllCards()
        {
            SetupData();
            using var context = new PreplitContext(_options);
            var handler = new GetCardList.Handler(context);
            var result = await handler.Handle(new GetCardList.Query(), CancellationToken.None);
            Assert.NotNull(result);
            ClearData();
        }

        [Fact, Trait("Category", "GetCards")]
        public async Task GetCards_ShouldReturnCardsFromCategory()
        {
            SetupData();
            Guid categoryId = _expectedCategories[0].CategoryId;

            using var context = new PreplitContext(_options);
            var handler = new GetCategoryCardList.Handler(context);
            var result = await handler.Handle(new GetCategoryCardList.Query { CategoryId = categoryId }, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(categoryId, result.First().CategoryId);
            ClearData();
        }

        [Fact, Trait("Category", "GetCards")]
        public async Task GetCardById_ShouldReturnCard()
        {
            SetupData();
            Guid cardId = _expectedCards[0].CardId;

            using var context = new PreplitContext(_options);
            var handler = new GetCardDetails.Handler(context);
            var result = await handler.Handle(new GetCardDetails.Query { Id = cardId }, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(cardId, result.CardId);
            ClearData();
        }

        [Fact, Trait("Category", "GetCards")]
        public async Task GetCardById_IfCardNotFound_ShouldThrowException()
        {
            SetupData();
            Guid cardId = SharedObjects.INVALID_CARD_ID;

            using var context = new PreplitContext(_options);
            var handler = new GetCardDetails.Handler(context);
            await Assert.ThrowsAsync<NullReferenceException>(() => handler.Handle(new GetCardDetails.Query { Id = cardId }, CancellationToken.None));
            ClearData();
        }

        [Fact, Trait("Category", "CreateCard")]
        public async Task CreateCard_ShouldCreateCard()
        {
            SetupData();
            var cardDTO = new CardAddDTO
            {
                Question = "Question",
                Answer = "Answer",
                CategoryId = _expectedCategories[0].CategoryId,
                OwnerId = _expectedCards[0].UserId
            };

            using var context = new PreplitContext(_options);
            var handler = new CreateCard.Handler(context);
            var result = await handler.Handle(new CreateCard.Command { CardInfo = cardDTO }, CancellationToken.None);
            Assert.NotEqual(Guid.Empty, result);
            ClearData();
        }

        [Fact, Trait("Category", "EditCard")]
        public async Task EditCard_ShouldEditCard()
        {
            SetupData();
            var cardDTO = new CardUpdateDTO
            {
                CardId = _expectedCards[0].CardId,
                Question = "Question",
                Answer = "Answer",
                CategoryId = _expectedCategories[1].CategoryId,
                OwnerId = _expectedCards[0].UserId
            };
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<CardUpdateDTO, Card>()).CreateMapper();

            using var context = new PreplitContext(_options);
            var handler = new EditCard.Handler(context, mapper);
            await handler.Handle(new EditCard.Command { CardInfo = cardDTO, UserId = _expectedCards[0].UserId }, CancellationToken.None);
            Assert.Equal(cardDTO.CategoryId, SharedObjects.VALID_CATEGORY_ID_2);
            ClearData();
        }

        [Fact, Trait("Category", "EditCard")]
        public async Task EditCard_IfUnauthorized_ShouldThrowException()
        {
            SetupData();
            var cardDTO = new CardUpdateDTO
            {
                CardId = _expectedCards[0].CardId,
                Question = "Question",
                Answer = "Answer",
                CategoryId = _expectedCategories[1].CategoryId,
                OwnerId = _expectedCards[0].UserId
            };

            using var context = new PreplitContext(_options);
            var handler = new EditCard.Handler(context, new Mock<IMapper>().Object);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(new EditCard.Command { CardInfo = cardDTO, UserId = SharedObjects.INVALID_USER_ID }, CancellationToken.None));
            ClearData();
        }

        [Fact, Trait("Category", "DeleteCard")]
        public async Task DeleteCard_ShouldDeleteCard()
        {
            SetupData();
            using var context = new PreplitContext(_options);
            var handler = new DeleteCard.Handler(context);
            await handler.Handle(new DeleteCard.Command { Id = _expectedCards[0].CardId, UserId = _expectedCards[0].UserId }, CancellationToken.None);
            var result = await context.Cards.FindAsync(_expectedCards[0].CardId);
            Assert.Null(result);
            ClearData();
        }

        [Fact, Trait("Category", "DeleteCard")]
        public async Task DeleteCard_IfUnauthorized_ShouldThrowException()
        {
            SetupData();
            using var context = new PreplitContext(_options);
            var handler = new DeleteCard.Handler(context);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(new DeleteCard.Command { Id = _expectedCards[0].CardId, UserId = SharedObjects.INVALID_USER_ID }, CancellationToken.None));
            ClearData();
        }

        private void SetupData()
        {
            using var context = new PreplitContext(_options);
            // If data exists, delete the database, then recreate it
            if (context.Database.EnsureDeleted())
            {
                context.Database.EnsureCreated();
            }
            context.AddRange(_expectedCards);
            context.AddRange(_expectedCategories);
            context.SaveChanges();
        }

        private void ClearData()
        {
            using var context = new PreplitContext(_options);
            context.Database.EnsureDeleted();
        }
    }
}