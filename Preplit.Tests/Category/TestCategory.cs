using Preplit.Domain;
using Preplit.Services.Categories.Queries;
using Preplit.Services.Categories.Commands;
using Preplit.Data;
using Preplit.Domain.DTOs;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AutoMapper;

namespace Preplit.Tests
{
    public class TestCategory
    {
        private readonly List<Card> _expectedCards;
        private readonly List<Category> _expectedCategories;
        private readonly DbContextOptions<PreplitContext> _options;
        public TestCategory()
        {
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

        [Fact, Trait("Category", "GetCategories")]
        public async Task GetCategoriesAdmin_ShouldReturnAllCategories()
        {
            SetupData();
            using var context = new PreplitContext(_options);
            var handler = new GetCategoryList.Handler(context);
            var result = await handler.Handle(new GetCategoryList.Query(), CancellationToken.None);
            Assert.NotNull(result);
            ClearData();
        }

        [Fact, Trait("Category", "GetCategories")]
        public async Task GetCategories_ShouldReturnCategoriesFromUser()
        {
            SetupData();
            using var context = new PreplitContext(_options);
            var handler = new GetUserCategoryList.Handler(context);
            var result = await handler.Handle(new GetUserCategoryList.Query { UserId = _expectedCards[0].UserId }, CancellationToken.None);
            Assert.NotNull(result);
            ClearData();
        }

        [Fact, Trait("Category", "GetCategories")]
        public async Task GetCategoryById_ShouldReturnCategory()
        {
            SetupData();
            Guid categoryId = _expectedCategories[0].CategoryId;
            using var context = new PreplitContext(_options);
            var handler = new GetCategoryDetails.Handler(context);
            var result = await handler.Handle(new GetCategoryDetails.Query { Id = categoryId }, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(categoryId, result.CategoryId);
            ClearData();
        }

        [Fact, Trait("Category", "GetCategories")]
        public async Task GetCategoryById_IfCategoryNotFound_ShouldThrowException()
        {
            SetupData();
            Guid categoryId = SharedObjects.INVALID_CATEGORY_ID;
            using var context = new PreplitContext(_options);
            var handler = new GetCategoryDetails.Handler(context);
            await Assert.ThrowsAsync<NullReferenceException>(() => handler.Handle(new GetCategoryDetails.Query { Id = categoryId }, CancellationToken.None));
            ClearData();
        }

        [Fact, Trait("Category", "CreateCategory")]
        public async Task CreateCategory_ShouldCreateCategory()
        {
            SetupData();
            var categoryDTO = new CategoryAddDTO
            {
                Name = "Category",
                UserId = _expectedCards[0].UserId
            };
            using var context = new PreplitContext(_options);
            var handler = new CreateCategory.Handler(context);
            var result = await handler.Handle(new CreateCategory.Command { CategoryInfo = categoryDTO }, CancellationToken.None);
            Assert.NotEqual(Guid.Empty, result);
            ClearData();
        }

        [Fact, Trait("Category", "EditCategory")]
        public async Task EditCategory_ShouldEditCategory()
        {
            SetupData();
            var categoryDTO = new CategoryUpdateDTO
            {
                CategoryId = _expectedCategories[0].CategoryId,
                Name = "Category",
                UserId = _expectedCards[0].UserId
            };

            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<CategoryUpdateDTO, Category>()).CreateMapper();

            using var context = new PreplitContext(_options);
            var handler = new EditCategory.Handler(context, mapper);
            await handler.Handle(new EditCategory.Command { CategoryInfo = categoryDTO, UserId = _expectedCards[0].UserId }, CancellationToken.None);
            var result = await context.Categories.FindAsync(_expectedCategories[0].CategoryId);
            Assert.Equal(categoryDTO.Name, result!.Name);
            ClearData();
        }

        [Fact, Trait("Category", "EditCategory")]
        public async Task EditCategory_IfUnauthorized_ShouldThrowException()
        {
            SetupData();
            var categoryDTO = new CategoryUpdateDTO
            {
                CategoryId = _expectedCategories[0].CategoryId,
                Name = "Category",
                UserId = _expectedCards[0].UserId
            };

            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<CategoryUpdateDTO, Category>()).CreateMapper();
            using var context = new PreplitContext(_options);
            var handler = new EditCategory.Handler(context, mapper);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(new EditCategory.Command { CategoryInfo = categoryDTO, UserId = SharedObjects.INVALID_USER_ID }, CancellationToken.None));
            ClearData();
        }

        [Fact, Trait("Category", "DeleteCategory")]
        public async Task DeleteCategory_ShouldDeleteCategory()
        {
            SetupData();
            using var context = new PreplitContext(_options);
            var handler = new DeleteCategory.Handler(context);
            await handler.Handle(new DeleteCategory.Command { Id = _expectedCategories[0].CategoryId, UserId = _expectedCards[0].UserId }, CancellationToken.None);
            var result = await context.Categories.FindAsync(_expectedCategories[0].CategoryId);
            Assert.Null(result);
            ClearData();
        }

        [Fact, Trait("Category", "DeleteCategory")]
        public async Task DeleteCategory_IfUnauthorized_ShouldThrowException()
        {
            SetupData();
            using var context = new PreplitContext(_options);
            var handler = new DeleteCategory.Handler(context);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(new DeleteCategory.Command { Id = _expectedCategories[0].CategoryId, UserId = SharedObjects.INVALID_USER_ID }, CancellationToken.None));
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