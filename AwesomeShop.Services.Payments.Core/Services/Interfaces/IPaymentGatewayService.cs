using AwesomeShop.Services.Payments.Core.ValueObjects;

namespace AwesomeShop.Services.Payments.Core.Services.Interfaces;

public interface IPaymentGatewayService
{
    Task<bool> Process(CreditCardInfoValueObject creditCardInfoValueObject);
}