using Domain;
using Enums;
using Moq;
using Repositories;
using Services.BusinessLogic.Implementation;
using Utils;
using Xunit;

namespace Tests;

public class UserTaskServiceTests
{
    private readonly Mock<IUserTaskRepository> _mockUserTaskRepository;
    private readonly UserTaskService _userTaskService;
    private UserTask _mockedUserTask;

    public UserTaskServiceTests()
    {
        _mockedUserTask = new UserTask
        {
            Id = 1,
            Name = "Test Name",
            Description = "Test Description",
            AssignedTo = "Test User",
            Status = UserTaskStatus.InProgress
        };
        
        _mockUserTaskRepository = new Mock<IUserTaskRepository>();
        _mockUserTaskRepository.Setup(it => it.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult((UserTask?)null));
        _mockUserTaskRepository.Setup(it => it.GetByIdAsync(It.Is<int>(id => id == 1))).ReturnsAsync(_mockedUserTask);
        _mockUserTaskRepository.Setup(it => it.UpdateAsync(It.IsAny<UserTask>())).ReturnsAsync((UserTask x) => x);
        _userTaskService = new UserTaskService(_mockUserTaskRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreate()
    {
        var task = new UserTask
        {
            Name = "Name1",
            Description = "Description1",
            AssignedTo = "User1"
        };

        var result = await _userTaskService.CreateAsync(task);
        
        _mockUserTaskRepository.Verify(x => x.CreateAsync(It.Is<UserTask>(it =>
            it.Name == task.Name 
            && it.Description == task.Description 
            && it.AssignedTo == task.AssignedTo
            && it.Status == UserTaskStatus.NotStarted)), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdate()
    {
        var id = 1;
        var status = UserTaskStatus.Completed;
        
        var result = await _userTaskService.UpdateStatusAsync(id, status);
        
        _mockUserTaskRepository.Verify(x => x.GetByIdAsync(It.Is<int>(it => it == id)), Times.Once);
        Assert.Equal(_mockedUserTask.Id, result.Id);
        Assert.Equal(_mockedUserTask.Name, result.Name);
        Assert.Equal(_mockedUserTask.Description, result.Description);
        Assert.Equal(_mockedUserTask.AssignedTo, result.AssignedTo);
        Assert.Equal(status, result.Status);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldFaile()
    {
        var id = 5;
        var status = UserTaskStatus.Completed;
        
        IntakerNotFoundException ex = await Assert.ThrowsAsync<IntakerNotFoundException>(()=> _userTaskService.UpdateStatusAsync(id, status));
    }
}