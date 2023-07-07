namespace AwesomeShop.Services.Payments.Core.ValueObjects;

public class CreditCardInfoValueObject
{
    public CreditCardInfoValueObject(string cardNumber, string fullName, string expirationDate, string cvv)
    {
        CardNumber = cardNumber;
        FullName = fullName;
        ExpirationDate = expirationDate;
        Cvv = cvv;
    }

    public string CardNumber { get; set; }
    public string FullName { get; set; }
    public string ExpirationDate { get; set; }
    public string Cvv { get; set; }
}