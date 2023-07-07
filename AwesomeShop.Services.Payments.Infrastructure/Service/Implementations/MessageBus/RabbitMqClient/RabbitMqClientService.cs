using System.Text;
using AwesomeShop.Services.Payments.Core.Services.Interfaces;
using AwesomeShop.Services.Payments.Infrastructure.Service.Implementations.MessageBus.Connections;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;

namespace AwesomeShop.Services.Payments.Infrastructure.Service.Implementations.MessageBus;

public class RabbitMqClientService : IRabbitMqClientService
{
    private readonly IConnection _connection;

    public RabbitMqClientService(ProducerConnection producerConnection)
    {
        _connection = producerConnection.Connection;
    }

    public void Publish(object message, string routingKey, string exchange)
    {
        var channel = _connection.CreateModel();

        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        var payload = JsonConvert.SerializeObject(message, settings);
        var body = Encoding.UTF8.GetBytes(payload);

        channel.ExchangeDeclare(exchange, "topic", true);
        channel.BasicPublish(exchange, routingKey, null, body);
    }
}