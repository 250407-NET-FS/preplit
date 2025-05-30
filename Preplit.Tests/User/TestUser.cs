using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Preplit.Domain;
using Preplit.Services.Users.Queries;
using Preplit.Services.Users.Commands;
using Xunit;

namespace Preplit.Tests
{
    public class TestUser
    {
        private readonly Mock<UserManager<User>> _userManager;
        private readonly Mock<IConfiguration> _mockConfig;

        public TestUser()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _userManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null
            );
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Setup Configuration mock
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["Jwt:Key"]).Returns("your-256-bit-secret-key-here-need-to-hit-256");
            _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
            _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("test-audience");
            _mockConfig.Setup(c => c["Jwt:ExpireDays"]).Returns("7");
        }

        // [Fact, Trait("Category", "GetUsers")]
        // public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        // {

        //     var users = new List<User>
        //     {
        //         new() { Id = SharedObjects.VALID_USER_ID_1, UserName = "user1", Email = "user1@test.com" },
        //         new() { Id = SharedObjects.VALID_USER_ID_2, UserName = "user2", Email = "user2@test.com" }
        //     }.AsQueryable();

        //     var mockDbSet = new Mock<DbSet<User>>();
        //     var handler = new GetUserList.Handler(_userManager.Object);
        //     mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
        //     mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
        //     mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
        //     mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

        //     _userManager.Setup(x => x.Users).Returns(mockDbSet.Object);

        //     var result = await handler.Handle(new GetUserList.Query(), CancellationToken.None);
        //     Assert.NotNull(result);
        //     Assert.Equal(2, result.Count());
        // }

        [Fact, Trait("Category", "GetUsers")]
        public async Task GetLoggedInUser_shouldReturnUser()
        {
            var user = new User { Id = Guid.NewGuid(), UserName = "testuser" };
            var claimsPrincipal = new ClaimsPrincipal();
            var handler = new GetLoggedUser.Handler(_userManager.Object);

            _userManager.Setup(x => x.GetUserAsync(claimsPrincipal))
                .ReturnsAsync(user);

            var result = await handler.Handle(new GetLoggedUser.Query { User = claimsPrincipal }, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
        }

        [Fact, Trait("Category", "GetUsers")]
        public async Task GetUserById_ShouldReturnUser()
        {
            var userId = SharedObjects.VALID_USER_ID_1;
            var expectedUser = new User { Id = userId, UserName = "testuser" };
            var handler = new GetUserDetails.Handler(_userManager.Object);

            _userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(expectedUser);

            var result = await handler.Handle(new GetUserDetails.Query { UserId = userId }, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);

        }

        [Fact, Trait("Category", "Generate Token")]
        public async Task GenerateToken_ShouldReturnValidJwtToken()
        {
            var user = new User
            {
                Id = SharedObjects.VALID_USER_ID_1,
                UserName = "testuser",
                Email = "test@test.com"
            };

            var roles = new List<string> { "User" };
            var handler = new GenerateToken.Handler(_userManager.Object, _mockConfig.Object);
            _userManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(roles);

            var token = await handler.Handle(new GenerateToken.Command { User = user }, CancellationToken.None);

            Assert.NotNull(token);
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);

            Assert.Equal(user.UserName, jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value);
            Assert.Equal(user.Id.ToString(), jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            Assert.Equal(user.Email, jwtToken.Claims.First(c => c.Type == ClaimTypes.Email).Value);
            Assert.Equal(roles[0], jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        }

        [Fact, Trait("Category", "Delete Users")]
        public async Task DeleteUser_ShouldDeleteUser()
        {
            var userId = SharedObjects.VALID_USER_ID_1;
            var user = new User { Id = userId, UserName = "testuser" };
            var handler = new DeleteUser.Handler(_userManager.Object);

            _userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);
            _userManager.Setup(x => x.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            await handler.Handle(new DeleteUser.Command { UserId = userId }, CancellationToken.None);

            _userManager.Verify(x => x.DeleteAsync(user), Times.Once);
        }

        [Fact, Trait("Category", "Delete Users")]
        public async Task DeleteUser_WhenUserNotFound_ShouldThrowException()
        {
            var userId = SharedObjects.VALID_USER_ID_1;
            var handler = new DeleteUser.Handler(_userManager.Object);
            _userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((User?)null);

            var exception = await Assert.ThrowsAsync<NullReferenceException>(
                () => handler.Handle(new DeleteUser.Command { UserId = userId }, CancellationToken.None));
            Assert.Equal("User not found", exception.Message);
        }

        [Fact, Trait("Category", "Delete Users")]
        public async Task DeleteUser_WhenDeleteFails_ShouldThrowException()
        {
            var userId = SharedObjects.VALID_USER_ID_1;
            var user = new User { Id = userId, UserName = "testuser" };
            var handler = new DeleteUser.Handler(_userManager.Object);

            _userManager.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);
            _userManager.Setup(x => x.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Delete failed" }));


            var exception = await Assert.ThrowsAsync<Exception>(
                () => handler.Handle(new DeleteUser.Command { UserId = userId }, CancellationToken.None));
            Assert.Equal("Failed to delete user", exception.Message);
        }
    }
}