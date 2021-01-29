using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAPISample.Bll;
using UserAPISample.Controllers;
using UserAPISample.Model;
using Xunit;

namespace UserAPISample.XUnitTests.Core
{
    public class UserControllerTests
    {
        private static List<User> GetTestUsers()
        {
            var list = new List<User>
            {
                new User
                {
                    Id = 1,
                    Name = "Test One",
                    Username = "Test1",
                    Email = "test1@test.com",
                    Address = new Address
                    {
                        Street = "White Sand1",
                        Suite = "Suit1 879",
                        City = "Big City",
                        Zipcode = "13245-111"
                    },
                    Phone = "123456-000-111"
                },
                new User
                {
                    Id = 2,
                    Name = "Test Second",
                    Username = "Test2",
                    Email = "test2@test.com",
                    Address = new Address
                    {
                        Street = "White Sand2",
                        Suite = "Suit2 879",
                        City = "Big City",
                        Zipcode = "13245-222"
                    },
                    Phone = "123456-000-222"
                }
            };

            return list;
        }

        [Fact]
        public async Task GetUsersAsync_ReturnsAllUsers()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(m => m.GetAsync()).ReturnsAsync(GetTestUsers());
            var controller = new UserController(mockUserService.Object);

            // Act
            var result = await controller.GetUsersAsync().ConfigureAwait(false);

            // Assert
            var returnValue = Assert.IsType<List<User>>(result);
            var user = returnValue.FirstOrDefault();
            Assert.Equal(2, returnValue.Count);
            Assert.Equal("Test One", user.Name);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(99)]
        public async Task GetUserAsync_ReturnsHttpNotFound_ForInvalidUserId(int id)
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(m => m.GetAsync(id))
                .ReturnsAsync(GetTestUsers().Find(u => u.Id == id));
            var controller = new UserController(mockUserService.Object);

            // Act
            var result = await controller.GetUserAsync(id).ConfigureAwait(false);

            // Assert
            var actionResult = Assert.IsType<ActionResult<User>>(result);
            var responseStatus = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal(id, responseStatus.Value);
        }

        [Theory]
        [InlineData(new object[] {2, "Test Second"})]
        public async Task GetUserAsync_ReturnsUser_ForValidUserId(int id, string name)
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(m => m.GetAsync(id))
                .ReturnsAsync(GetTestUsers().Find(u => u.Id == id));
            var controller = new UserController(mockUserService.Object);

            // Act
            var result = await controller.GetUserAsync(id).ConfigureAwait(false);

            // Assert
            var actionResult = Assert.IsType<ActionResult<User>>(result);
            var returnValue = Assert.IsType<User>(actionResult.Value);
            Assert.Equal(id, returnValue.Id);
            Assert.Equal(name, returnValue.Name);
        }

        [Fact]
        public async Task CreateUserAsync_ReturnsBadRequest_GivenInvalidModel()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var controller = new UserController(mockUserService.Object);
            controller.ModelState.AddModelError("error", "some error");

            // Act
            var result = await controller.CreateUserAsync(null).ConfigureAwait(false);

            // Assert
            var actionResult = Assert.IsType<ActionResult<User>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        public static TheoryData<List<User>, User> CreateUserData_For_NoContent =>
            new TheoryData<List<User>, User>
            {
                {
                    new List<User>(GetTestUsers()),
                    new User
                    {
                        Name = "Test Third",
                        Username = "Test3",
                        Email = "test3@test.com",
                        Address = new Address
                        {
                            Street = "White Sand3",
                            Suite = "Suit3 879",
                            City = "Big City",
                            Zipcode = "13245-333"
                        },
                        Phone = "123456-000-333"
                    }
                }
            };

        [Theory]
        [MemberData(nameof(CreateUserData_For_NoContent))]
        public async Task CreateUserAsync_ReturnsCreatedResponse_ForValidUser(List<User> userCollection, User userIn)
        {
            // Arrange
            var newUserFake = new User
            {
                Id = userCollection.Max(u => u.Id) + 1,
                Name = userIn.Name,
                Username = userIn.Username,
                Email = userIn.Email,
                Address = userIn.Address,
                Phone = userIn.Phone
            };

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(m => m.CreateAsync(userIn))
                .ReturnsAsync(newUserFake).Verifiable();
            var controller = new UserController(mockUserService.Object);

            // Act
            var createdUser = await controller.CreateUserAsync(userIn).ConfigureAwait(false);

            // Assert
            var actionResult = Assert.IsType<ActionResult<User>>(createdUser);
            var responseStatus = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            mockUserService.Verify(m => m.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        public static TheoryData<int, User> UpdateUserData_For_BadRequest =>
            new TheoryData<int, User>
            {
                { 27, new User { Id = 3, Name = "Bad Request Name"} },
                { 0, new User { Id = 1, Name = "Bad Request Name"} }
            };

        public static TheoryData<int, User> UpdateUserData_For_NotFound =>
            new TheoryData<int, User>
            {
                { 27, new User { Id = 27, Name = "Not Exists" } }
            };

        public static TheoryData<int, User> UpdateUserData_For_NoContent =>
            new TheoryData<int, User>
            {
                { 2, new User
                        {
                            Id = 2,
                            Name = "New Name",
                            Username = "newusername",
                            Email = "newusername@name.com"
                        }
                }
            };

        [Theory]
        [MemberData(nameof(UpdateUserData_For_BadRequest))]
        public async Task UpdateUserAsync_ReturnsBadRequest_ForNotMatchingIds(int id, User user)
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var controller = new UserController(mockUserService.Object);

            // Act
            var result = await controller.UpdateUserAsync(id, user).ConfigureAwait(false);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [MemberData(nameof(UpdateUserData_For_NotFound))]
        public async Task UpdateUserAsync_ReturnsNotFound_ForNotExistUser(int id, User user)
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(m => m.GetAsync(id))
                .ReturnsAsync(GetTestUsers().Find(u => u.Id == id));
            var controller = new UserController(mockUserService.Object);

            // Act
            var result = await controller.UpdateUserAsync(id, user).ConfigureAwait(false);

            // Assert
            var actionResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(id, actionResult.Value);
        }

        [Theory]
        [MemberData(nameof(UpdateUserData_For_NoContent))]
        public async Task UpdateUserAsync_ReturnsNoContent_ForExistingUser(int id, User user)
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();

            mockUserService.Setup(m => m.GetAsync(id))
                .ReturnsAsync(GetTestUsers().Find(u => u.Id == id));

            mockUserService.Setup(m => m.UpdateAsync(It.IsAny<int>(), It.IsAny<User>()))
                .Returns(Task.CompletedTask).Verifiable();

            var controller = new UserController(mockUserService.Object);

            // Act
            var result = await controller.UpdateUserAsync(id, user).ConfigureAwait(false);

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
            mockUserService.Verify(m => m.UpdateAsync(It.IsAny<int>(), It.IsAny<User>()), Times.Once);
        }

        [Theory]
        [InlineData(27)]
        public async Task DeleteUserAsync_ReturnsNotFound_ForNotExistUser(int id)
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(m => m.GetAsync(id))
                .ReturnsAsync(GetTestUsers().Find(u => u.Id == id));
            var controller = new UserController(mockUserService.Object);

            // Act
            var result = await controller.DeleteUserAsync(id).ConfigureAwait(false);

            // Assert
            var actionResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(id, actionResult.Value);
        }

        [Theory]
        [InlineData(2)]
        public async Task DeleteUserAsync_ReturnsNoContent_ForExistingUser(int id)
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();

            mockUserService.Setup(m => m.GetAsync(id))
                .ReturnsAsync(GetTestUsers().Find(u => u.Id == id));
            mockUserService.Setup(m => m.RemoveAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask).Verifiable();
            var controller = new UserController(mockUserService.Object);

            // Act
            var result = await controller.DeleteUserAsync(id).ConfigureAwait(false);

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
            mockUserService.Verify(m => m.RemoveAsync(It.IsAny<int>()), Times.Once);
        }
    }
}
