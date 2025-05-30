using Preplit.Domain;
using Preplit.Services.Categories.Queries;
using Preplit.Services.Categories.Commands;
using Preplit.Data;
using Preplit.Domain.DTOs;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using System.Threading.Tasks;

namespace Preplit.Tests
{
    public class TestCategory
    {
        private readonly Mock<PreplitContext> _mockContext;
        private readonly Mock<DbSet<Card>> _mockCardDbSet;
        private readonly Mock<DbSet<Category>> _mockCategoryDbSet;
        public TestCategory()
        {
            // Create a clean instance of the database context
            _mockContext = new Mock<PreplitContext>();
            _mockCardDbSet = new Mock<DbSet<Card>>();
            _mockCategoryDbSet = new Mock<DbSet<Category>>();
        }

        [Fact, Trait("Category", "GetCategories")]
        public async Task GetCategoriesAdmin_ShouldReturnAllCategories()
        {
            List<Category> expectedCategories = [
                SharedObjects.CloneValidCategory1(),
                SharedObjects.CloneValidCategory2(),
                SharedObjects.CloneValidCategory3(),
                SharedObjects.CloneValidCategory4()
            ];
            _mockCategoryDbSet.As<IQueryable<Category>>().Setup(m => m).Returns(expectedCategories.AsQueryable());

            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);
            
            var handler = new GetCategoryList.Handler(_mockContext.Object);
            var result = await handler.Handle(new GetCategoryList.Query(), CancellationToken.None);
            Assert.NotNull(result);
        }

        [Fact, Trait("Category", "GetCategories")]
        public async Task GetCategories_ShouldReturnCategoriesFromUser()
        {
            List<Category> expectedCategories = [
                SharedObjects.CloneValidCategory1(),
                SharedObjects.CloneValidCategory2(),
                SharedObjects.CloneValidCategory3(),
                SharedObjects.CloneValidCategory4()
            ];
            _mockCategoryDbSet.As<IQueryable<Category>>().Setup(m => m).Returns(expectedCategories.AsQueryable());

            _mockContext.Setup(c => c.Categories.Where(c => c.UserId == SharedObjects.VALID_USER_ID_1)).Returns(new List<Category> { SharedObjects.CloneValidCategory1(), SharedObjects.CloneValidCategory2() }.AsQueryable());

            var handler = new GetUserCategoryList.Handler(_mockContext.Object);
            var result = await handler.Handle(new GetUserCategoryList.Query { UserId = SharedObjects.VALID_USER_ID_1 }, CancellationToken.None);
            Assert.NotNull(result);
        }

        [Fact, Trait("Category", "GetCategories")]
        public async Task GetCategoryById_ShouldReturnCategory()
        {
            Category category = SharedObjects.CloneValidCategory1();
            _mockCategoryDbSet.As<IQueryable<Category>>().Setup(m => m).Returns(new List<Category> { category }.AsQueryable());

            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);

            var handler = new GetCategoryDetails.Handler(_mockContext.Object);
            Guid categoryId = category.CategoryId;
            var result = await handler.Handle(new GetCategoryDetails.Query { Id = categoryId }, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(categoryId, result.CategoryId);
        }

        [Fact, Trait("Category", "GetCategories")]
        public async Task GetCategoryById_IfCategoryNotFound_ShouldThrowException()
        {
            Guid categoryId = SharedObjects.INVALID_CATEGORY_ID;
            _mockContext.Setup(c => c.Categories.Where(c => c.CategoryId == categoryId)).Returns(new List<Category>().AsQueryable());

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
            _mockCategoryDbSet.As<IQueryable<Category>>().Setup(m => m).Returns(new List<Category>().AsQueryable());
            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(1));
            
            var handler = new CreateCategory.Handler(_mockContext.Object);
            var result = await handler.Handle(new CreateCategory.Command { CategoryInfo = categoryDTO }, CancellationToken.None);
            Assert.NotEqual(Guid.Empty, result);
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
            _mockCategoryDbSet.As<IQueryable<Category>>().Setup(m => m).Returns(new List<Category> { SharedObjects.CloneValidCategory1() }.AsQueryable());
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<CategoryUpdateDTO, Category>()).CreateMapper();

            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);
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
            _mockCategoryDbSet.As<IQueryable<Category>>().Setup(m => m).Returns(new List<Category> { SharedObjects.CloneValidCategory1() }.AsQueryable());
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<CategoryUpdateDTO, Category>()).CreateMapper();
            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);
            var handler = new EditCategory.Handler(_mockContext.Object, mapper);
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(new EditCategory.Command { CategoryInfo = categoryDTO, UserId = SharedObjects.INVALID_USER_ID }, CancellationToken.None));
            Assert.Equal("Unauthorized", ex.Message);
        }

        [Fact, Trait("Category", "DeleteCategory")]
        public async Task DeleteCategory_ShouldDeleteCategory()
        {
            Category category = SharedObjects.CloneValidCategory1();
            _mockCategoryDbSet.As<IQueryable<Category>>().Setup(m => m).Returns(new List<Category> { category }.AsQueryable());

            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(1));

            var handler = new DeleteCategory.Handler(_mockContext.Object);
            await handler.Handle(new DeleteCategory.Command { Id = SharedObjects.VALID_CATEGORY_ID_1, UserId = SharedObjects.VALID_USER_ID_1 }, CancellationToken.None);
            
            _mockContext.Verify(c => c.Categories.Remove(It.IsAny<Category>()), Times.Once);
        }

        [Fact, Trait("Category", "DeleteCategory")]
        public async Task DeleteCategory_IfUnauthorized_ShouldThrowException()
        {
            Category category = SharedObjects.CloneValidCategory1();
            _mockCategoryDbSet.As<IQueryable<Category>>().Setup(m => m).Returns(new List<Category> { category }.AsQueryable());
            _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);
            var handler = new DeleteCategory.Handler(_mockContext.Object);
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(new DeleteCategory.Command { Id = SharedObjects.VALID_CATEGORY_ID_1, UserId = SharedObjects.INVALID_USER_ID }, CancellationToken.None));
            Assert.Equal("Unauthorized", ex.Message);
        }
    }
}