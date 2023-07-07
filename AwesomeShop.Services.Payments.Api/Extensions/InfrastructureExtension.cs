using AwesomeShop.Services.Payments.Core.Entities;
using AwesomeShop.Services.Payments.Core.Entities.Interfaces;
using AwesomeShop.Services.Payments.Core.Repositories.Interfaces;
using AwesomeShop.Services.Payments.Core.Repositories.Interfaces.Mongo;
using AwesomeShop.Services.Payments.Core.Services.Interfaces;
using AwesomeShop.Services.Payments.Infrastructure.Persistence.Repositories.Implementations.Invoices;
using AwesomeShop.Services.Payments.Infrastructure.Persistence.Repositories.Implementations.Mongo;
using AwesomeShop.Services.Payments.Infrastructure.Persistence.Repositories.Options;
using AwesomeShop.Services.Payments.Infrastructure.Service.Implementations;
using AwesomeShop.Services.Payments.Infrastructure.Service.Implementations.MessageBus;
using AwesomeShop.Services.Payments.Infrastructure.Service.Implementations.MessageBus.Connections;
using MongoDB.Bson;
using MongoDB.Driver;
using RabbitMQ.Client;

namespace AwesomeShop.Services.Payments.Api.Extensions;

public static class InfrastructureExtension
{
    public static IServiceCollection AddServicesExtension(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IPaymentGatewayService, PaymentGatewayService>();
        return serviceCollection;
    }

    public static IServiceCollection AddRepositoriesExtension(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IInvoiceRepository, InvoiceRepository>();
        serviceCollection.AddMongoRepository<Invoice>("invoices");
        return serviceCollection;
    }

    private static IServiceCollection AddMongoRepository<T>(this IServiceCollection serviceCollection,
        string collection) where T : IEntityBase
    {
        serviceCollection.AddScoped<IMongoRepository<T>>(f =>
        {
            var mongoDatabase = f.GetRequiredService<IMongoDatabase>();

            return new MongoRepository<T>(mongoDatabase, collection);
        });

        return serviceCollection;
    }

    public static IServiceCollection AddMongoDbExtension(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(sp =>
        {
            var configuration = sp.GetService<IConfiguration>();
            var options = new MongoDbOption();

            configuration?.GetSection("MongoDb").Bind(options);

            return options;
        });

        serviceCollection.AddSingleton<IMongoClient>(sp =>
        {
            var options = sp.GetService<MongoDbOption>();

            return new MongoClient(options.ConnectionString);
        });

        serviceCollection.AddTransient(sp =>
        {
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;

            var options = sp.GetService<MongoDbOption>();
            var mongoClient = sp.GetService<IMongoClient>();

            return mongoClient?.GetDatabase(options?.Database);
        });

        return serviceCollection;
    }

    public static IServiceCollection AddRabbitMqExtension(this IServiceCollection serviceCollection)
    {
        var connectionFactory = new ConnectionFactory()
        {
            HostName = "localhost"
        };

        var connection = connectionFactory.CreateConnection("payments-service-producer");

        serviceCollection.AddSingleton(new ProducerConnection(connection));
        serviceCollection.AddSingleton<IRabbitMqClientService, RabbitMqClientService>();
        // serviceCollection.AddTransient<IEventProcessor, EventProcessor>();

        return serviceCollection;
    }
}