using AwesomeShop.Services.Payments.Core.Entities;
using AwesomeShop.Services.Payments.Core.Repositories.Interfaces;
using AwesomeShop.Services.Payments.Core.Repositories.Interfaces.Mongo;

namespace AwesomeShop.Services.Payments.Infrastructure.Persistence.Repositories.Implementations.Invoices;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly IMongoRepository<Invoice> _mongoRepository;

    public InvoiceRepository(IMongoRepository<Invoice> mongoRepository)
    {
        _mongoRepository = mongoRepository;
    }

    public async Task AddAsync(Invoice invoice)
    {
        await _mongoRepository.AddAsync(invoice);
    }
}