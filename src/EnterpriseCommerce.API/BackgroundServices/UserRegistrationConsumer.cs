using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace EnterpriseCommerce.Infrastructure.BackgroundServices;

public class UserRegistrationConsumer
    : BackgroundService
{
    private readonly ILogger<UserRegistrationConsumer>
        _logger;

    public UserRegistrationConsumer(
        ILogger<UserRegistrationConsumer> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        var connection =
            await factory.CreateConnectionAsync(
                stoppingToken);

        var channel =
            await connection.CreateChannelAsync(
                cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: "user-registration",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        var consumer =
            new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();

            var message =
                Encoding.UTF8.GetString(body);

            _logger.LogInformation(
                "User registration event received: {Message}",
                message);

            await Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(
            queue: "user-registration",
            autoAck: true,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }
}