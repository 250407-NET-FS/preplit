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
        private readonly Mock<PreplitContext> _mockContext;
        private readonly Mock<DbSet<Card>> _mockCardDbSet;
        private readonly Mock<DbSet<Category>> _mockCategoryDbSet;

        public TestCard()
        {
            // Create a clean instance of the database context
            _mockContext = new Mock<PreplitContext>();
            _mockCardDbSet = new Mock<DbSet<Card>>();
            _mockCategoryDbSet = new Mock<DbSet<Category>>();
        }

        [Fact, Trait("Category", "GetCards")]
        public async Task GetCardsAdmin_ShouldReturnAllCards()
        {
            List<Card> expectedCards = [
                SharedObjects.CloneValidCard1(),
                SharedObjects.CloneValidCard2(),
                SharedObjects.CloneValidCard3(),
                SharedObjects.CloneValidCard4()
            ];

            _mockCardDbSet.As<IQueryable<Card>>().Setup(m => m.ToList()).Returns(expectedCards);

            _mockContext.Setup(c => c.Cards).Returns(_mockCardDbSet.Object);

            var handler = new GetCardList.Handler(_mockContext.Object);
            var result = await handler.Handle(new GetCardList.Query(), CancellationToken.None);
            Assert.NotNull(result);

        }

        [Fact, Trait("Category", "GetCards")]
        public async Task GetCards_ShouldReturnCardsFromCategory()
        {
            List<Card> expectedCards = [
                SharedObjects.CloneValidCard1(),
                SharedObjects.CloneValidCard2(),
                SharedObjects.CloneValidCard3(),
                SharedObjects.CloneValidCard4()
            ];
            Category category = SharedObjects.CloneValidCategory1();
            Guid categoryId = category.CategoryId;

            _mockCardDbSet.As<IQueryable<Card>>().Setup(m => m).Returns(expectedCards.AsQueryable());
            _mockCategoryDbSet.As<IQueryable<Category>>().Setup(m => m).Returns(new List<Category> { category }.AsQueryable());

            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);
            _mockContext.Setup(c => c.Cards.Where(c => c.CategoryId == categoryId)).Returns(new List<Card> { SharedObjects.CloneValidCard1() }.AsQueryable());

            var handler = new GetCategoryCardList.Handler(_mockContext.Object);
            var result = await handler.Handle(new GetCategoryCardList.Query { CategoryId = categoryId }, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(categoryId, result.First().CategoryId);
        }

        [Fact, Trait("Category", "GetCards")]
        public async Task GetCardById_ShouldReturnCard()
        {
            Card card = SharedObjects.CloneValidCard1();
            _mockCardDbSet.As<IQueryable<Card>>().Setup(m => m).Returns(new List<Card> { card }.AsQueryable());

            _mockContext.Setup(c => c.Cards).Returns(_mockCardDbSet.Object);

            var handler = new GetCardDetails.Handler(_mockContext.Object);
            Guid cardId = card.CardId;
            var result = await handler.Handle(new GetCardDetails.Query { Id = cardId }, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(cardId, result.CardId);
        }

        [Fact, Trait("Category", "GetCards")]
        public async Task GetCardById_IfCardNotFound_ShouldThrowException()
        {
            Guid cardId = SharedObjects.INVALID_CARD_ID;

            _mockContext.Setup(c => c.Cards.Where(c => c.CardId == cardId)).Returns(new List<Card>().AsQueryable());
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

            _mockCategoryDbSet.As<IQueryable<Category>>()
            .Setup(m => m)
            .Returns(
                new List<Category> { SharedObjects.CloneValidCategory1() }
                .AsQueryable()
            );

            _mockContext.Setup(c => c.Cards).Returns(_mockCardDbSet.Object);
            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(1));

            var handler = new CreateCard.Handler(_mockContext.Object);
            var result = await handler.Handle(new CreateCard.Command { CardInfo = cardDTO }, CancellationToken.None);
            Assert.NotEqual(Guid.Empty, result);
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
            _mockCardDbSet.As<IQueryable<Card>>().Setup(m => m).Returns(new List<Card> { SharedObjects.CloneValidCard1() }.AsQueryable());
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<CardUpdateDTO, Card>()).CreateMapper();

            _mockContext.Setup(c => c.Cards).Returns(_mockCardDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(1));

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
            _mockCardDbSet.As<IQueryable<Card>>().Setup(m => m).Returns(new List<Card> { SharedObjects.CloneValidCard1() }.AsQueryable());

            _mockContext.Setup(c => c.Cards).Returns(_mockCardDbSet.Object);

            var handler = new EditCard.Handler(_mockContext.Object, new Mock<IMapper>().Object);
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(new EditCard.Command { CardInfo = cardDTO, UserId = SharedObjects.INVALID_USER_ID }, CancellationToken.None));
            Assert.Equal("Unauthorized", ex.Message);
        }

        [Fact, Trait("Category", "DeleteCard")]
        public async Task DeleteCard_ShouldDeleteCard()
        {
            Card card = SharedObjects.CloneValidCard1();
            _mockCardDbSet.As<IQueryable<Card>>().Setup(m => m).Returns(new List<Card> { card }.AsQueryable());
            _mockContext.Setup(c => c.Cards).Returns(_mockCardDbSet.Object);

            var handler = new DeleteCard.Handler(_mockContext.Object);
            await handler.Handle(new DeleteCard.Command { Id = SharedObjects.VALID_CARD_ID_1, UserId = SharedObjects.VALID_USER_ID_1 }, CancellationToken.None);
            
            _mockContext.Verify(c => c.Cards.Remove(It.IsAny<Card>()), Times.Once);
        }

        [Fact, Trait("Category", "DeleteCard")]
        public async Task DeleteCard_IfUnauthorized_ShouldThrowException()
        {
            Card card = SharedObjects.CloneValidCard1();
            _mockCardDbSet.As<IQueryable<Card>>().Setup(m => m).Returns(new List<Card> { card }.AsQueryable());
            _mockContext.Setup(c => c.Cards).Returns(_mockCardDbSet.Object);

            var handler = new DeleteCard.Handler(_mockContext.Object);
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(new DeleteCard.Command { Id = SharedObjects.VALID_CARD_ID_1, UserId = SharedObjects.INVALID_USER_ID }, CancellationToken.None));
            Assert.Equal("Unauthorized", ex.Message);
        }
    }
}