using Azure.Messaging.ServiceBus;
using Communication.Implementation;
using Communication.Models;
using Domain;
using Enums;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Services.BusinessLogic;
using Xunit;

namespace Tests;

public class UserTaskReceiverServiceTests
{
    private readonly Mock<IUserTaskService> _mockUserTaskService;
    private readonly UserTaskReceiverService _userTaskReceiverService;
    private readonly Mock<ServiceBusReceiver> _mockReceiver;

    public UserTaskReceiverServiceTests()
    {
        Mock<ILogger<UserTaskReceiverService>> mockLogger = new();
        _mockUserTaskService = new Mock<IUserTaskService>();

        var mockServiceBusClientFactory = new Mock<IAzureClientFactory<ServiceBusClient>>();
        _userTaskReceiverService = new UserTaskReceiverService(mockServiceBusClientFactory.Object, MockServiceProvider().Object, mockLogger.Object);
        
        _mockReceiver = new();
        _mockReceiver
            .Setup(receiver => receiver.CompleteMessageAsync(
                It.IsAny<ServiceBusReceivedMessage>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task CreateUserTaskMessageHandlerAsync_ShouldCallCreateAsync()
    {
        var model = new CreateUserTaskModel
        {
            Name = "Task1",
            Description = "Description1",
            AssignedTo = "User1"
        };

        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: BinaryData.FromString(JsonConvert.SerializeObject(model)),
            messageId: "messageId",
            partitionKey: "helloKey",
            correlationId: "correlationId",
            contentType: "contentType",
            replyTo: "replyTo"
        );

        ProcessMessageEventArgs processArgs = new(
            message: message,
            receiver: _mockReceiver.Object,
            cancellationToken: CancellationToken.None);
        
        await _userTaskReceiverService.CreateUserTaskMessageHandlerAsync(processArgs);

        _mockUserTaskService
            .Verify(x => x.CreateAsync(It.Is<UserTask>(it =>
            it.Name == model.Name && it.Description == model.Description && it.AssignedTo == model.AssignedTo)), Times.Once);
    }

    [Fact]
    public async Task UpdateUserTaskMessageHandlerAsync_ShouldCallUpdateStatusAsync()
    {
        var model = new UpdateUserTaskModel
        {
            Id = 1,
            Status = UserTaskStatus.Completed
        };

        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: BinaryData.FromString(JsonConvert.SerializeObject(model)),
            messageId: "messageId",
            partitionKey: "helloKey",
            correlationId: "correlationId",
            contentType: "contentType",
            replyTo: "replyTo"
        );

        ProcessMessageEventArgs processArgs = new(
            message: message,
            receiver: _mockReceiver.Object,
            cancellationToken: CancellationToken.None);

        await _userTaskReceiverService.UpdateUserTaskMessageHandlerAsync(processArgs);

        _mockUserTaskService.Verify(x => x.UpdateStatusAsync(model.Id, model.Status), Times.Once);

        _mockUserTaskService
            .Verify(x => x.UpdateStatusAsync(It.Is<int>(id => id == model.Id),
                It.Is<UserTaskStatus>(status => status == model.Status)), Times.Once);
    }
    
    private Mock<IServiceProvider> MockServiceProvider()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        
        serviceProvider.Setup(it => it.GetService(typeof(IUserTaskService)))
            .Returns(_mockUserTaskService.Object);

        var serviceScope = new Mock<IServiceScope>();
        serviceScope.Setup(it => it.ServiceProvider)
            .Returns(serviceProvider.Object);

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        serviceScopeFactory.Setup(it => it.CreateScope())
            .Returns(serviceScope.Object);

        serviceProvider.Setup(it => it.GetService(typeof(IServiceScopeFactory)))
            .Returns(serviceScopeFactory.Object);
        return serviceProvider;
    }

}
