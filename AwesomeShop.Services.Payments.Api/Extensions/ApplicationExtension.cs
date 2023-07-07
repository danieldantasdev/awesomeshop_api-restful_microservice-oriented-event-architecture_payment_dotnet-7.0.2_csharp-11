using AwesomeShop.Services.Payments.Application.Subscribers;

namespace AwesomeShop.Services.Payments.Api.Extensions;

public static class ApplicationExtension
{
    public static IServiceCollection AddSubscribersExtension(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHostedService<OrderCreatedSubscriber>();

        return serviceCollection;
    }
}