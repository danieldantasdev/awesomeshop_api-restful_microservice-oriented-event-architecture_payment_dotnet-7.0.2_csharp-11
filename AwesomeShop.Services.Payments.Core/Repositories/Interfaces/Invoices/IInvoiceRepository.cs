using AwesomeShop.Services.Payments.Core.Entities;

namespace AwesomeShop.Services.Payments.Core.Repositories.Interfaces;

public interface IInvoiceRepository
{
    Task AddAsync(Invoice invoice);
}