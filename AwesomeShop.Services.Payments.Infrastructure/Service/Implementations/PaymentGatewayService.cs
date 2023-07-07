using AwesomeShop.Services.Payments.Core.Services.Interfaces;
using AwesomeShop.Services.Payments.Core.ValueObjects;

namespace AwesomeShop.Services.Payments.Infrastructure.Service.Implementations;

public class PaymentGatewayService : IPaymentGatewayService
{
    public async Task<bool> Process(CreditCardInfoValueObject creditCardInfoValueObject)
    {
        return await Task.FromResult(true);
    }
}