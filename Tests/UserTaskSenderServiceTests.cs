using Azure.Messaging.ServiceBus;
using Communication.Implementation;
using Communication.Models;
using Enums;
using Microsoft.Extensions.Azure;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Tests;

public class UserTaskSenderServiceTests
{
    private readonly Mock<ServiceBusSender> _mockCreateSender;
    private readonly Mock<ServiceBusSender> _mockUpdateSender;
    private readonly UserTaskSenderService _userTaskSenderService;

    public UserTaskSenderServiceTests()
    {
        _mockCreateSender = new Mock<ServiceBusSender>();
        _mockUpdateSender = new Mock<ServiceBusSender>();

        var mockServiceBusClientFactory = new Mock<IAzureClientFactory<ServiceBusClient>>();
        var mockServiceBusClient = new Mock<ServiceBusClient>();

        mockServiceBusClient
            .Setup(it => it.CreateSender("create-user-task"))
            .Returns(_mockCreateSender.Object);
        
        mockServiceBusClient
            .Setup(it => it.CreateSender("update-user-task"))
            .Returns(_mockUpdateSender.Object);

        mockServiceBusClientFactory
            .Setup(it => it.CreateClient("Default"))
            .Returns(mockServiceBusClient.Object);

        _userTaskSenderService = new UserTaskSenderService(mockServiceBusClientFactory.Object);
    }

    [Fact]
    public async Task CreateUserTaskAsync_ShouldSendCreateUserTaskMessage()
    {
        var model = new CreateUserTaskModel
        {
            Name = "Task1",
            Description = "Description1",
            AssignedTo = "User1"
        };

        await _userTaskSenderService.CreateUserTaskAsync(model);

        _mockCreateSender.Verify(x => x.SendMessageAsync(It.Is<ServiceBusMessage>(it =>
            it.Body.ToString() == JsonConvert.SerializeObject(model)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUserTaskAsync_ShouldSendUpdateUserTaskMessage()
    {
        var model = new UpdateUserTaskModel
        {
            Id = 1,
            Status = UserTaskStatus.Completed
        };

        await _userTaskSenderService.UpdateUserTaskAsync(model);

        _mockUpdateSender.Verify(x => x.SendMessageAsync(It.Is<ServiceBusMessage>(it =>
            it.Body.ToString() == JsonConvert.SerializeObject(model)), It.IsAny<CancellationToken>()), Times.Once);
    }
}
