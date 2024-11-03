using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Communication.Models;
using Microsoft.Extensions.Azure;
using Newtonsoft.Json;
using Utils;

namespace Communication.Implementation;

public class UserTaskSenderService : IUserTaskSenderService
{
    private ServiceBusSender _createUserTaskSender;
    private ServiceBusSender _updateUserTaskSender;

    public UserTaskSenderService(IAzureClientFactory<ServiceBusClient> serviceBusSenderFactory)
    {
        var client = serviceBusSenderFactory.CreateClient("Default");
        _createUserTaskSender = client.CreateSender("create-user-task");
        _updateUserTaskSender = client.CreateSender("update-user-task");
    }

    public async Task CreateUserTaskAsync(CreateUserTaskModel model)
    {
        try
        {
            await _createUserTaskSender.SendMessageAsync(new(JsonConvert.SerializeObject(model)));
        }
        catch (Exception e)
        {
            throw new IntakerInternalErrorException(ErrorCodes.CreateUserTaskSendError, e);
        }
    }
    
    public async Task UpdateUserTaskAsync(UpdateUserTaskModel model)
    {
        try
        {
            await _updateUserTaskSender.SendMessageAsync(new(JsonConvert.SerializeObject(model)));
        }
        catch (Exception e)
        {
            throw new IntakerInternalErrorException(ErrorCodes.UpdateUserTaskSendError, e);
        }
    }
}