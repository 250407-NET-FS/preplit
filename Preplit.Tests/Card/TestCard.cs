using Preplit.Domain;
using Preplit.Services.Cards.Queries;
using Preplit.Services.Cards.Commands;
using Preplit.Data;
using Preplit.Domain.DTOs;
using Moq;
using MockQueryable.Moq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MockQueryable;

namespace Preplit.Tests
{
    public class TestCard
    {
        private readonly Mock<PreplitContext> _mockContext;
        private readonly Mock<DbSet<Card>> _mockCardDbSet;

        public TestCard()
        {
            // Create a clean instance of the database context
            _mockContext = new Mock<PreplitContext>(new DbContextOptions<PreplitContext>());
            // Set up the mock db set for the test methods
            IEnumerable<Card> expectedCards = [
                SharedObjects.CloneValidCard1(),
                SharedObjects.CloneValidCard2(),
                SharedObjects.CloneValidCard3(),
                SharedObjects.CloneValidCard4()
            ];
            _mockCardDbSet = expectedCards.BuildMock().BuildMockDbSet();
        }

        [Fact, Trait("Category", "GetCards")]
        public async Task GetCardsAdmin_ShouldReturnAllCards()
        {
            _mockContext.Setup(c => c.Cards).Returns(_mockCardDbSet.Object);

            var handler = new GetCardList.Handler(_mockContext.Object);
            var result = await handler.Handle(new Mock<GetCardList.Query>().Object, CancellationToken.None);
            Assert.NotNull(result);

        }

        [Fact, Trait("Category", "GetCards")]
        public async Task GetCards_ShouldReturnCardsFromCategory()
        {
            Category category = SharedObjects.CloneValidCategory1();
            Guid categoryId = category.CategoryId;

            var categoryDbSet = new List<Category> { category }.BuildMock().BuildMockDbSet();

            _mockContext.Setup(c => c.Categories).Returns(categoryDbSet.Object);
            _mockContext.Setup(c => c.Cards).Returns(_mockCardDbSet.Object);

            var handler = new GetCategoryCardList.Handler(_mockContext.Object);
            var result = await handler.Handle(new GetCategoryCardList.Query { CategoryId = categoryId }, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(categoryId, result.First().CategoryId);
        }

        [Fact, Trait("Category", "GetCards")]
        public async Task GetCardById_ShouldReturnCard()
        {
            Card card = SharedObjects.CloneValidCard1();
            Guid cardId = card.CardId;

            _mockContext.Setup(c => c.Cards).Returns(_mockCardDbSet.Object);
            _mockContext.Setup(c => c.Cards.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(card).Verifiable();

            var handler = new GetCardDetails.Handler(_mockContext.Object);

            var result = await handler.Handle(new GetCardDetails.Query { Id = cardId }, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(cardId, result.CardId);
        }

        [Fact, Trait("Category", "GetCards")]
        public async Task GetCardById_IfCardNotFound_ShouldThrowException()
        {
            Guid cardId = SharedObjects.INVALID_CARD_ID;

            _mockContext.Setup(c => c.Cards).Returns(_mockCardDbSet.Object);
            var handler = new GetCardDetails.Handler(_mockContext.Object);
            await Assert.ThrowsAsync<NullReferenceException>(() => handler.Handle(new GetCardDetails.Query { Id = cardId }, CancellationToken.None));
        }

        [Fact, Trait("Category", "CreateCard")]
        public async Task CreateCard_ShouldCreateCard()
        {
            var cardDTO = new CardAddDTO
            {
                Question = "Question",
                Answer = "Answer",
                CategoryId = SharedObjects.VALID_CATEGORY_ID_1,
                OwnerId = SharedObjects.VALID_USER_ID_1
            };

            var categoryDbSet = new List<Category> { SharedObjects.CloneValidCategory1() }.BuildMock().BuildMockDbSet();

            _mockContext.Setup(c => c.Cards).Returns(_mockCardDbSet.Object);
            _mockContext.Setup(c => c.Categories).Returns(categoryDbSet.Object);
            _mockContext.Setup(c => c.Categories.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(SharedObjects.CloneValidCategory1());
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            var handler = new CreateCard.Handler(_mockContext.Object);
            var result = await handler.Handle(new CreateCard.Command { CardInfo = cardDTO }, CancellationToken.None);
            Assert.NotEqual(Guid.Empty, result);
            _mockContext.Verify(c => c.Cards.Add(It.IsAny<Card>()), Times.Once);
        }

        [Fact, Trait("Category", "CreateCard")]
        public async Task CreateCard_IfNoCategoryFound_ShouldThrowException()
        {
            var cardDTO = new CardAddDTO
            {
                Question = "Question",
                Answer = "Answer",
                CategoryId = SharedObjects.INVALID_CATEGORY_ID,
                OwnerId = SharedObjects.VALID_USER_ID_1
            };
            _mockContext.Setup(c => c.Cards).Returns(_mockCardDbSet.Object);
            _mockContext.Setup(c => c.Categories.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Category)null!);

            var handler = new CreateCard.Handler(_mockContext.Object);
            var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(new CreateCard.Command { CardInfo = cardDTO }, CancellationToken.None));
            Assert.Equal("Category does not exist", ex.Message);
        }

        [Fact, Trait("Category", "CreateCard")]
        public async Task CreateCard_IfInsertFailed_ShouldThrowException()
        {
            var cardDTO = new CardAddDTO
            {
                Question = "Question",
                Answer = "Answer",
                CategoryId = SharedObjects.VALID_CATEGORY_ID_1,
                OwnerId = SharedObjects.VALID_USER_ID_1
            };

            var categoryDbSet = new List<Category> { SharedObjects.CloneValidCategory1() }.BuildMock().BuildMockDbSet();

            _mockContext.Setup(c => c.Cards).Returns(_mockCardDbSet.Object);
            _mockContext.Setup(c => c.Categories).Returns(categoryDbSet.Object);
            _mockContext.Setup(c => c.Categories.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(SharedObjects.CloneValidCategory1());
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(0));

            var handler = new CreateCard.Handler(_mockContext.Object);
            var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(new CreateCard.Command { CardInfo = cardDTO }, CancellationToken.None));
            Assert.Equal("Failed to insert card", ex.Message);
            _mockContext.Verify(c => c.Cards.Add(It.IsAny<Card>()), Times.Once);
        }

        [Fact, Trait("Category", "EditCard")]
        public async Task EditCard_ShouldEditCard()
        {
            var cardDTO = new CardUpdateDTO
            {
                CardId = SharedObjects.VALID_CARD_ID_1,
                Question = "Question",
                Answer = "Answer",
                CategoryId = SharedObjects.VALID_CATEGORY_ID_2,
                OwnerId = SharedObjects.VALID_USER_ID_1
            };
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<CardUpdateDTO, Card>()).CreateMapper();

            _mockContext.Setup(c => c.Cards).Returns(_mockCardDbSet.Object);
            _mockContext.Setup(c => c.Cards.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(SharedObjects.CloneValidCard1());
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            var handler = new EditCard.Handler(_mockContext.Object, mapper);
            await handler.Handle(new EditCard.Command { CardInfo = cardDTO, UserId = SharedObjects.VALID_USER_ID_1 }, CancellationToken.None);

            _mockContext.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Fact, Trait("Category", "EditCard")]
        public async Task EditCard_IfUnauthorized_ShouldThrowException()
        {
            var cardDTO = new CardUpdateDTO
            {
                CardId = SharedObjects.VALID_CARD_ID_1,
                Question = "Question",
                Answer = "Answer",
                CategoryId = SharedObjects.VALID_CATEGORY_ID_2,
                OwnerId = SharedObjects.VALID_USER_ID_1
            };

            _mockContext.Setup(c => c.Cards).Returns(_mockCardDbSet.Object);
            _mockContext.Setup(c => c.Cards.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(SharedObjects.CloneValidCard1());

            var handler = new EditCard.Handler(_mockContext.Object, new Mock<IMapper>().Object);
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(new EditCard.Command { CardInfo = cardDTO, UserId = SharedObjects.INVALID_USER_ID }, CancellationToken.None));
            Assert.Equal("Unauthorized", ex.Message);
        }

        [Fact, Trait("Category", "EditCard")]
        public async Task EditCard_IfEditFailed_ShouldThrowException()
        {
            var cardDTO = new CardUpdateDTO
            {
                CardId = SharedObjects.VALID_CARD_ID_1,
                Question = "Question",
                Answer = "Answer",
                CategoryId = SharedObjects.VALID_CATEGORY_ID_2,
                OwnerId = SharedObjects.VALID_USER_ID_1
            };
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<CardUpdateDTO, Card>()).CreateMapper();

            _mockContext.Setup(c => c.Cards).Returns(_mockCardDbSet.Object);
            _mockContext.Setup(c => c.Cards.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(SharedObjects.CloneValidCard1());
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(0));

            var handler = new EditCard.Handler(_mockContext.Object, mapper);
            var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(new EditCard.Command { CardInfo = cardDTO, UserId = SharedObjects.VALID_USER_ID_1 }, CancellationToken.None));
            Assert.Equal("Failed to update card", ex.Message);
        }

        [Fact, Trait("Category", "DeleteCard")]
        public async Task DeleteCard_ShouldDeleteCard()
        {
            Card card = SharedObjects.CloneValidCard1();
            _mockContext.Setup(c => c.Cards).Returns(_mockCardDbSet.Object);
            _mockContext.Setup(c => c.Cards.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(card);
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(1));

            var handler = new DeleteCard.Handler(_mockContext.Object);
            await handler.Handle(new DeleteCard.Command { Id = SharedObjects.VALID_CARD_ID_1, UserId = SharedObjects.VALID_USER_ID_1 }, CancellationToken.None);

            _mockContext.Verify(c => c.Remove(It.IsAny<Card>()), Times.Once);
        }

        [Fact, Trait("Category", "DeleteCard")]
        public async Task DeleteCard_IfUnauthorized_ShouldThrowException()
        {
            Card card = SharedObjects.CloneValidCard1();
            _mockContext.Setup(c => c.Cards).Returns(_mockCardDbSet.Object);
            _mockContext.Setup(c => c.Cards.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(card);

            var handler = new DeleteCard.Handler(_mockContext.Object);
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(new DeleteCard.Command { Id = SharedObjects.VALID_CARD_ID_1, UserId = SharedObjects.INVALID_USER_ID }, CancellationToken.None));
            Assert.Equal("Unauthorized", ex.Message);
        }

        [Fact, Trait("Category", "DeleteCard")]
        public async Task DeleteCard_IfDeleteFailed_ShouldThrowException()
        {
            Card card = SharedObjects.CloneValidCard1();
            _mockContext.Setup(c => c.Cards).Returns(_mockCardDbSet.Object);
            _mockContext.Setup(c => c.Cards.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(card);
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(0));

            var handler = new DeleteCard.Handler(_mockContext.Object);
            var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(new DeleteCard.Command { Id = SharedObjects.VALID_CARD_ID_1, UserId = SharedObjects.VALID_USER_ID_1 }, CancellationToken.None));
            Assert.Equal("Failed to delete card", ex.Message);
        }
    }
}