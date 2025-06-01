using Preplit.Domain;
using Preplit.Services.Categories.Queries;
using Preplit.Services.Categories.Commands;
using Preplit.Data;
using Preplit.Domain.DTOs;
using Moq;
using MockQueryable;
using MockQueryable.Moq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;


namespace Preplit.Tests
{
    public class TestCategory
    {
        private readonly Mock<PreplitContext> _mockContext;
        private readonly Mock<DbSet<Category>> _mockCategoryDbSet;
        public TestCategory()
        {
            // Create a clean instance of the database context
            _mockContext = new Mock<PreplitContext>(new DbContextOptions<PreplitContext>());
            // Set up the mock db set for the test methods
            IEnumerable<Category> expectedCategories = [
                SharedObjects.CloneValidCategory1(),
                SharedObjects.CloneValidCategory2(),
                SharedObjects.CloneValidCategory3(),
                SharedObjects.CloneValidCategory4()
            ];
            _mockCategoryDbSet = expectedCategories.BuildMock().BuildMockDbSet();
        }

        [Fact, Trait("Category", "GetCategories")]
        public async Task GetCategoriesAdmin_ShouldReturnAllCategories()
        {
            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);

            var handler = new GetCategoryList.Handler(_mockContext.Object);
            var result = await handler.Handle(new GetCategoryList.Query(), CancellationToken.None);
            Assert.NotNull(result);
        }

        [Fact, Trait("Category", "GetCategories")]
        public async Task GetCategories_ShouldReturnCategoriesFromUser()
        {
            Guid userId = SharedObjects.VALID_USER_ID_1;
            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);

            var handler = new GetUserCategoryList.Handler(_mockContext.Object);
            var result = await handler.Handle(new GetUserCategoryList.Query { UserId = userId }, CancellationToken.None);
            Assert.NotNull(result);
        }

        [Fact, Trait("Category", "GetCategories")]
        public async Task GetCategoryById_ShouldReturnCategory()
        {
            Category category = SharedObjects.CloneValidCategory1();
            Guid categoryId = category.CategoryId;

            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);
            _mockContext.Setup(c => c.Categories.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(category);

            var handler = new GetCategoryDetails.Handler(_mockContext.Object);

            var result = await handler.Handle(new GetCategoryDetails.Query { Id = categoryId }, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(categoryId, result.CategoryId);
        }

        [Fact, Trait("Category", "GetCategories")]
        public async Task GetCategoryById_IfCategoryNotFound_ShouldThrowException()
        {
            Guid categoryId = SharedObjects.INVALID_CATEGORY_ID;

            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);

            var handler = new GetCategoryDetails.Handler(_mockContext.Object);
            var ex = await Assert.ThrowsAsync<NullReferenceException>(() => handler.Handle(new GetCategoryDetails.Query { Id = categoryId }, CancellationToken.None));
            Assert.Equal("Category not found", ex.Message);
        }

        [Fact, Trait("Category", "CreateCategory")]
        public async Task CreateCategory_ShouldCreateCategory()
        {
            var categoryDTO = new CategoryAddDTO
            {
                Name = "Category",
                UserId = SharedObjects.VALID_USER_ID_1
            };
            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(1));

            var handler = new CreateCategory.Handler(_mockContext.Object);
            var result = await handler.Handle(new CreateCategory.Command { CategoryInfo = categoryDTO }, CancellationToken.None);
            Assert.NotEqual(Guid.Empty, result);
        }

        [Fact, Trait("Category", "CreateCategory")]
        public async Task CreateCategory_IfInsertFailed_ShouldThrowException()
        {
            var categoryDTO = new CategoryAddDTO
            {
                Name = "Category",
                UserId = SharedObjects.VALID_USER_ID_1
            };
            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(0));

            var handler = new CreateCategory.Handler(_mockContext.Object);
            var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(new CreateCategory.Command { CategoryInfo = categoryDTO }, CancellationToken.None));
            Assert.Equal("Failed to insert category", ex.Message);
        }

        [Fact, Trait("Category", "EditCategory")]
        public async Task EditCategory_ShouldEditCategory()
        {
            var categoryDTO = new CategoryUpdateDTO
            {
                CategoryId = SharedObjects.VALID_CATEGORY_ID_1,
                Name = "Category",
                UserId = SharedObjects.VALID_USER_ID_1
            };
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<CategoryUpdateDTO, Category>()).CreateMapper();

            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);
            _mockContext.Setup(c => c.Categories.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(SharedObjects.CloneValidCategory1());
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(1));

            var handler = new EditCategory.Handler(_mockContext.Object, mapper);
            await handler.Handle(new EditCategory.Command { CategoryInfo = categoryDTO, UserId = SharedObjects.VALID_USER_ID_1 }, CancellationToken.None);

            _mockContext.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Fact, Trait("Category", "EditCategory")]
        public async Task EditCategory_IfUnauthorized_ShouldThrowException()
        {
            var categoryDTO = new CategoryUpdateDTO
            {
                CategoryId = SharedObjects.VALID_CATEGORY_ID_1,
                Name = "Category",
                UserId = SharedObjects.VALID_USER_ID_1
            };
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<CategoryUpdateDTO, Category>()).CreateMapper();
            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);
            _mockContext.Setup(c => c.Categories.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(SharedObjects.CloneValidCategory1());

            var handler = new EditCategory.Handler(_mockContext.Object, mapper);
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(new EditCategory.Command { CategoryInfo = categoryDTO, UserId = SharedObjects.INVALID_USER_ID }, CancellationToken.None));
            Assert.Equal("Unauthorized", ex.Message);
        }

        [Fact, Trait("Category", "EditCategory")]
        public async Task EditCategory_IfUpdateFailed_ShouldThrowException()
        {
            var categoryDTO = new CategoryUpdateDTO
            {
                CategoryId = SharedObjects.VALID_CATEGORY_ID_1,
                Name = "Category",
                UserId = SharedObjects.VALID_USER_ID_1
            };
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<CategoryUpdateDTO, Category>()).CreateMapper();
            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);
            _mockContext.Setup(c => c.Categories.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(SharedObjects.CloneValidCategory1());
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(0));

            var handler = new EditCategory.Handler(_mockContext.Object, mapper);
            var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(new EditCategory.Command { CategoryInfo = categoryDTO, UserId = SharedObjects.VALID_USER_ID_1 }, CancellationToken.None));
            Assert.Equal("Failed to update category", ex.Message);
        }

        [Fact, Trait("Category", "DeleteCategory")]
        public async Task DeleteCategory_ShouldDeleteCategory()
        {
            Category category = SharedObjects.CloneValidCategory1();
            var cardDbSet = new List<Card> { SharedObjects.CloneValidCard1(), SharedObjects.CloneValidCard2() }.BuildMock().BuildMockDbSet();

            _mockContext.Setup(c => c.Cards).Returns(cardDbSet.Object);
            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);
            _mockContext.Setup(c => c.Categories.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(category);
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(1));

            var handler = new DeleteCategory.Handler(_mockContext.Object);
            await handler.Handle(new DeleteCategory.Command { Id = SharedObjects.VALID_CATEGORY_ID_1, UserId = SharedObjects.VALID_USER_ID_1 }, CancellationToken.None);

            _mockContext.Verify(c => c.Remove(It.IsAny<Category>()), Times.Once);
        }

        [Fact, Trait("Category", "DeleteCategory")]
        public async Task DeleteCategory_IfUnauthorized_ShouldThrowException()
        {
            Category category = SharedObjects.CloneValidCategory1();
            var cardDbSet = new List<Card> { SharedObjects.CloneValidCard1(), SharedObjects.CloneValidCard2() }.BuildMock().BuildMockDbSet();

            _mockContext.Setup(c => c.Cards).Returns(cardDbSet.Object);
            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);
            _mockContext.Setup(c => c.Categories.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(category);

            var handler = new DeleteCategory.Handler(_mockContext.Object);
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(new DeleteCategory.Command { Id = SharedObjects.VALID_CATEGORY_ID_1, UserId = SharedObjects.INVALID_USER_ID }, CancellationToken.None));
            Assert.Equal("Unauthorized", ex.Message);
        }

        [Fact, Trait("Category", "DeleteCategory")]
        public async Task DeleteCategory_IfDeleteFailed_ShouldThrowException()
        {
            Category category = SharedObjects.CloneValidCategory1();
            var cardDbSet = new List<Card> { SharedObjects.CloneValidCard1(), SharedObjects.CloneValidCard2() }.BuildMock().BuildMockDbSet();

            _mockContext.Setup(c => c.Cards).Returns(cardDbSet.Object);
            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);
            _mockContext.Setup(c => c.Categories.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(category);
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(0));
        
            var handler = new DeleteCategory.Handler(_mockContext.Object);
            var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(new DeleteCategory.Command { Id = SharedObjects.VALID_CATEGORY_ID_1, UserId = SharedObjects.VALID_USER_ID_1 }, CancellationToken.None));
            Assert.Equal("Failed to delete category", ex.Message);
        }
    }
}