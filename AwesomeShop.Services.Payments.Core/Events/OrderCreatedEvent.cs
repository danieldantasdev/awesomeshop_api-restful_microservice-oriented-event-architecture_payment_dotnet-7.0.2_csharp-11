using AwesomeShop.Services.Payments.Core.ValueObjects;

namespace AwesomeShop.Services.Payments.Core.Events;

public class OrderCreatedEvent
{
    public OrderCreatedEvent(Guid id, decimal totalPrice, PaymentInfoValueObject paymentInfo, string fullName, string email)
    {
        Id = id;
        TotalPrice = totalPrice;
        PaymentInfo = paymentInfo;
        FullName = fullName;
        Email = email;
    }

    public Guid Id { get; set; }
    public decimal TotalPrice { get; set; }
    public PaymentInfoValueObject PaymentInfo { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
}