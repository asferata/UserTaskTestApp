using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Communication.Models;
using Domain;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Services.BusinessLogic;
using Utils;

namespace Communication.Implementation;

public class UserTaskReceiverService(IAzureClientFactory<ServiceBusClient> serviceBusSenderFactory,
    IServiceProvider serviceProvider,
    ILogger<UserTaskReceiverService> logger)
{
    private ServiceBusProcessor? _createUserTaskProcessor;
    private ServiceBusProcessor? _updateUserTaskProcessor;

    public async Task Start()
    {
        var client = serviceBusSenderFactory.CreateClient("Default");

        await InitCreateUserTaskProcessor(client);
        await InitUpdateUserTaskProcessor(client);
    }

    private async Task InitCreateUserTaskProcessor(ServiceBusClient client)
    {
        _createUserTaskProcessor = client
            .CreateProcessor("create-user-task");
        
        _createUserTaskProcessor.ProcessMessageAsync += CreateUserTaskMessageHandlerAsync;
        _createUserTaskProcessor.ProcessErrorAsync += CreateUserTaskErrorMessageHandlerAsync;
        await _createUserTaskProcessor.StartProcessingAsync();
    }

    private async Task InitUpdateUserTaskProcessor(ServiceBusClient client)
    {
        _updateUserTaskProcessor = client
            .CreateProcessor("update-user-task");
        
        _updateUserTaskProcessor.ProcessMessageAsync += UpdateUserTaskMessageHandlerAsync;
        _updateUserTaskProcessor.ProcessErrorAsync += UpdateUserTaskErrorMessageHandlerAsync;
        await _updateUserTaskProcessor.StartProcessingAsync();
    }

    public async Task CreateUserTaskMessageHandlerAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var body = args.Message.Body.ToString();
            var userTask = JsonConvert.DeserializeObject<CreateUserTaskModel>(body)!;
            using var scope = serviceProvider.CreateScope();
            var userTaskService = scope.ServiceProvider.GetRequiredService<IUserTaskService>();
            await userTaskService.CreateAsync(ToEntity(userTask));
            
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred during resolving CreateUserTask event");
        }
    }

    public async Task CreateUserTaskErrorMessageHandlerAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(args.Exception, "Error occurred during receiving CreateUserTask event");
    }

    public async Task UpdateUserTaskMessageHandlerAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var body = args.Message.Body.ToString();
            var userTask = JsonConvert.DeserializeObject<UpdateUserTaskModel>(body)!;
            using var scope = serviceProvider.CreateScope();
            var userTaskService = scope.ServiceProvider.GetRequiredService<IUserTaskService>();
            await userTaskService.UpdateStatusAsync(userTask.Id, userTask.Status);
            
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred during resolving UpdateUserTask event");

        }
    }

    public async Task UpdateUserTaskErrorMessageHandlerAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(args.Exception, "Error occurred during receiving UpdateUserTask event");
    }

    private static UserTask ToEntity(CreateUserTaskModel model)
    {
        return new()
        {
            Name = model.Name,
            Description = model.Description,
            AssignedTo = model.AssignedTo
        };
    }
}