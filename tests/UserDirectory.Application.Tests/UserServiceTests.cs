using Moq;
using UserDirectory.Application.Common;
using UserDirectory.Application.Users;
using UserDirectory.Domain.Entities;
using Xunit;

namespace UserDirectory.Application.Tests;

public sealed class UserServiceTests
{
    [Fact]
    public async Task CreateAsyncAddsUserAndReturnsResponse()
    {
        var repository = new Mock<IUserRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var service = new UserService(repository.Object, unitOfWork.Object);

        var result = await service.CreateAsync(
            new CreateUserRequest(" Jane Doe ", 40, " Melbourne ", " VIC ", " 3000 "));

        repository.Verify(x => x.AddAsync(It.Is<User>(u =>
            u.Name == "Jane Doe" && u.City == "Melbourne" && u.Pincode == "3000"),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal("Jane Doe", result.Name);
        Assert.Equal(40, result.Age);
    }

    [Fact]
    public async Task GetByIdAsyncWhenMissingReturnsNull()
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        var service = new UserService(repository.Object, Mock.Of<IUnitOfWork>());

        var result = await service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsyncWhenFoundUpdatesAndSaves()
    {
        var user = new User("Old Name", 30, "Sydney", "NSW", "2000");
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        var unitOfWork = new Mock<IUnitOfWork>();
        var service = new UserService(repository.Object, unitOfWork.Object);

        var result = await service.UpdateAsync(
            1,
            new UpdateUserRequest("New Name", 31, "Melbourne", "VIC", "3000"));

        Assert.NotNull(result);
        Assert.Equal("New Name", result!.Name);
        Assert.Equal("Melbourne", result.City);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsyncWhenFoundRemovesAndSaves()
    {
        var user = new User("John Doe", 25, "Geelong", "VIC", "3220");
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        var unitOfWork = new Mock<IUnitOfWork>();
        var service = new UserService(repository.Object, unitOfWork.Object);

        var result = await service.DeleteAsync(1);

        Assert.True(result);
        repository.Verify(x => x.Remove(user), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
