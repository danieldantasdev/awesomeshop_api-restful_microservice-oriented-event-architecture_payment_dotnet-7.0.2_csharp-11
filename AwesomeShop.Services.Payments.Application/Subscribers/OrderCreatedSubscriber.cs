using System.Text;
using AwesomeShop.Services.Payments.Core.Entities;
using AwesomeShop.Services.Payments.Core.Events;
using AwesomeShop.Services.Payments.Core.Repositories.Interfaces;
using AwesomeShop.Services.Payments.Core.Services.Interfaces;
using AwesomeShop.Services.Payments.Core.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace AwesomeShop.Services.Payments.Application.Subscribers;

public class OrderCreatedSubscriber : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _connection;
    private readonly IModel _model;
    private const string Queue = "payment-service/order-created";
    private const string Exchange = "payment-service";

    public OrderCreatedSubscriber(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        var connectionFactory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        _connection = connectionFactory.CreateConnection("payment-service-order-created-consumer");

        _model = _connection.CreateModel();

        _model.ExchangeDeclare(Exchange, "topic", true);
        _model.QueueDeclare(Queue, false, false, false, null);
        _model.QueueBind(Queue, Exchange, Queue);

        _model.QueueBind(Queue, "order-service", "order-created");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_model);

        consumer.Received += async (sender, eventArgs) =>
        {
            var contentArray = eventArgs.Body.ToArray();
            var contentString = Encoding.UTF8.GetString(contentArray);
            var message = JsonConvert.DeserializeObject<OrderCreatedEvent>(contentString);

            Console.WriteLine($"Message OrderCreated received with Id {message?.Id}");

            var result = message != null && await ProcessPayment(message);

            if (result)
            {
                _model.BasicAck(eventArgs.DeliveryTag, false);

                var paymentAccepted = new PaymentAcceptedEvent(message!.Id, message.FullName, message.Email);
                var payload = JsonConvert.SerializeObject(paymentAccepted);
                var byteArray = Encoding.UTF8.GetBytes(payload);

                Console.WriteLine("PaymentAccepted Published");

                _model.BasicPublish(Exchange, "payment-accepted", null, byteArray);
            }
        };

        _model.BasicConsume(Queue, false, consumer);

        return Task.CompletedTask;
    }

    private async Task<bool> ProcessPayment(OrderCreatedEvent orderCreated)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var paymentService = scope.ServiceProvider.GetService<IPaymentGatewayService>();

            var result = await paymentService!
                .Process(new CreditCardInfoValueObject(
                    orderCreated.PaymentInfo.CardNumber,
                    orderCreated.PaymentInfo.FullName,
                    orderCreated.PaymentInfo.ExpirationDate,
                    orderCreated.PaymentInfo.Cvv));

            var invoiceRepository = scope.ServiceProvider.GetService<IInvoiceRepository>();

            await invoiceRepository!.AddAsync(new Invoice(orderCreated.TotalPrice, orderCreated.Id,
                orderCreated.PaymentInfo.CardNumber));

            return result;
        }
    }
}