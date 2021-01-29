using MongoDB.Driver;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserAPISample.Dal.Implementations;
using UserAPISample.Dal.Interfaces;
using UserAPISample.Model;
using Xunit;

namespace UserAPISample.XUnitTests.Dal
{
    public class UserRepositoryTests
    {
        private readonly Mock<IMongoCollection<User>> _mockCollection;
        private readonly Mock<IMongoDBContext> _mockContext;
        private readonly Mock<IAsyncCursor<User>> _mockUserCursor;

        public UserRepositoryTests()
        {
            _mockCollection = new Mock<IMongoCollection<User>>();
            _mockCollection.Object.InsertMany(GetTestUsers());
            _mockContext = new Mock<IMongoDBContext>();
            _mockUserCursor = new Mock<IAsyncCursor<User>>();
            _mockUserCursor.SetupSequence(m => m.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            _mockUserCursor.SetupSequence(m => m.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));
        }

        [Fact]
        public async Task GetAsync_ReturnsAllUser_NoFilter()
        {
            //Arrange
            _mockUserCursor.Setup(m => m.Current).Returns(GetTestUsers());

            //Mock FindAsync
            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<FindOptions<User, User>>(),
                    It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(_mockUserCursor.Object).Verifiable();

            //Mock GetCollection
            _mockContext.Setup(c => c.GetCollection<User>()).Returns(_mockCollection.Object);

            var userRepo = new UserRepository(_mockContext.Object);

            //Act
            var result = await userRepo.GetAsync().ConfigureAwait(false);

            //Assert 
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            //Verify if FindAsync is called once 
            _mockCollection.Verify(c =>
                c.FindAsync(It.IsAny<FilterDefinition<User>>(),
                It.IsAny<FindOptions<User>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(2)]
        public async Task GetAsync_ReturnsUser_ForValidUserId(int id)
        {
            //Arrange
            _mockUserCursor.Setup(m => m.Current).Returns(GetTestUsers().Where(u => u.Id == id));

            //Mock FindAsync
            _mockCollection.Setup(c =>
                c.FindAsync(It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<FindOptions<User, User>>(),
                    It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(_mockUserCursor.Object).Verifiable();

            //Mock GetCollection
            _mockContext.Setup(c => c.GetCollection<User>()).Returns(_mockCollection.Object);

            var userRepo = new UserRepository(_mockContext.Object);

            //Act
            var result = await userRepo.GetAsync(1).ConfigureAwait(false);

            //Assert 
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);

            //Verify if FindAsync is called once 
            _mockCollection.Verify(c =>
                c.FindAsync(It.IsAny<FilterDefinition<User>>(),
                It.IsAny<FindOptions<User>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        public static TheoryData<User> InsertUserData =>
            new TheoryData<User>
            {
                        { new User
                                {
                                    Name = "New Name 3",
                                    Username = "newusername3",
                                    Email = "newusername3@name.com"
                                }
                        }
            };

        [Theory]
        [MemberData(nameof(InsertUserData))]
        public async Task InsertAsync_ReturnsUser_ForValidUser(User user)
        {
            var nextId = GetTestUsers().Max(u => u.Id) + 1;

            //Arrange
            _mockUserCursor.Setup(m => m.Current).Returns(GetTestUsers());

            //Mock FindAsync
            _mockCollection.Setup(c =>
                c.InsertOne(user,
                    It.IsAny<InsertOneOptions>(),
                    It.IsAny<CancellationToken>())
                )
                .Verifiable();

            //Mock GetCollection
            _mockContext.Setup(c => c.GetCollection<User>()).Returns(_mockCollection.Object);

            var mockUserRepository = new Mock<UserRepository>(_mockContext.Object);
            mockUserRepository.Setup(m => m.NextIdNumber()).Returns(nextId);

            //Act
            var result = await mockUserRepository.Object.InsertAsync(user).ConfigureAwait(false);

            //Assert 
            Assert.NotNull(result);
            Assert.Equal(nextId, result.Id);
            Assert.Equal(user.Name, result.Name);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.Email, result.Email);

            //Verify if InsertOne is called once 
            _mockCollection.Verify(c =>
                c.InsertOne(user,
                    It.IsAny<InsertOneOptions>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        public static TheoryData<int, User> UpdateUserData =>
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
        [MemberData(nameof(UpdateUserData))]
        public async Task UpdateAsync_ReturnsUser_ForValidUser(int id, User user)
        {
            //Arrange
            _mockUserCursor.Setup(m => m.Current).Returns(GetTestUsers());

            //Mock FindAsync
            _mockCollection.Setup(c =>
                c.ReplaceOneAsync(It.IsAny<FilterDefinition<User>>(),
                    user,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(It.IsAny<ReplaceOneResult>()).Verifiable();

            //Mock GetCollection
            _mockContext.Setup(c => c.GetCollection<User>()).Returns(_mockCollection.Object);

            var userRepo = new UserRepository(_mockContext.Object);

            //Act
            await userRepo.UpdateAsync(id, user).ConfigureAwait(false);

            //Assert 

            //Verify if ReplaceOneAsync is called once 
            _mockCollection.Verify(c =>
                c.ReplaceOneAsync(It.IsAny<FilterDefinition<User>>(),
                    user,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(2)]
        public async Task RemoveAsync_ReturnsUser_ForValidUserId(int id)
        {
            //Arrange
            _mockUserCursor.Setup(m => m.Current).Returns(GetTestUsers().Where(u => u.Id != id));

            //Mock FindAsync
            _mockCollection.Setup(c =>
                c.DeleteOneAsync(It.IsAny<FilterDefinition<User>>(),
                    It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(It.IsAny<DeleteResult>()).Verifiable();

            //Mock GetCollection
            _mockContext.Setup(c => c.GetCollection<User>()).Returns(_mockCollection.Object);

            var userRepo = new UserRepository(_mockContext.Object);

            //Act
            await userRepo.RemoveAsync(id).ConfigureAwait(false);

            //Assert 

            //Verify if DeleteOneAsync is called once 
            _mockCollection.Verify(c =>
                c.DeleteOneAsync(It.IsAny<FilterDefinition<User>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

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
    }
}
