using EnterpriseCommerce.Application.Interfaces;
using FluentValidation.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace EnterpriseCommerce.Infrastructure.Messaging
{
    public class RabbitMqProducer : IMessageBroker
    {
        public async void Publish<T>(T message)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            using var connection =
                await factory.CreateConnectionAsync();

            using var channel =
                await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "user-registration",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var json =
                JsonSerializer.Serialize(message);

            var body =
                Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "user-registration",
                body: body);
        }
    }
}
