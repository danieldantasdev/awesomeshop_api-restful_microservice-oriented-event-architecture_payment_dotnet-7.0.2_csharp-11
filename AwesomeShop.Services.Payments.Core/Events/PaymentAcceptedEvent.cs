namespace AwesomeShop.Services.Payments.Core.Events;

public class PaymentAcceptedEvent
{
    public PaymentAcceptedEvent(Guid id, string fullName, string email)
    {
        Id = id;
        FullName = fullName;
        Email = email;
    }

    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
}