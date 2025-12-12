using invoice.Core.DTO.Invoice;
using invoice.Core.Entities;

namespace invoice.Repo

{
    public interface IInvoiceRepository : IRepository<Invoice>
    {

        Task<IEnumerable<InvoiceSummaryDto>> GetGroupedByStatusAsync(string userId);
        Task<IEnumerable<InvoiceSummaryWithDateDto>> GetGroupedByStatusAndDateAsync(string userId);

    }
}
