namespace AwesomeShop.Services.Payments.Core.Services.Interfaces;

public interface IRabbitMqClientService
{
    void Publish(object message, string routingKey, string exchange);
}