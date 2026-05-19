using Azure.Messaging.ServiceBus;
using System.Text.Json;
using Synapse.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Synapse.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Synapse.Infrastructure.Services;

public class ServiceBus : IMessageBus
{
    private readonly ServiceBusSender _sender;

    public ServiceBus(IOptions<ServiceBusSettings> options)
    {
        var settings = options.Value;

        if(string.IsNullOrEmpty(settings.ConnectionString))
            throw new ArgumentException("Service Bus connection string is not configured.");

        var client = new ServiceBusClient(settings.ConnectionString);
        _sender = client.CreateSender(settings.QueueName);
    }

    public async Task PublishAsync<T>(T message)
    {
        var json = JsonSerializer.Serialize(message);

        try
        {
            await _sender.SendMessageAsync(new ServiceBusMessage(json));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ServiceBus error: {ex.Message}");
            throw;
        }
    }
}