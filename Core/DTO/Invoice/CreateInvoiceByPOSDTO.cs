using invoice.Core.DTO.PayInvoice;

namespace invoice.Core.DTO.Invoice
{
    public class CreateInvoiceByPOSDTO
    {
        public InvoiceCreateDTO Invoice { get; set; }
        public PayInvoiceCreateDTO Payment { get; set; }
    
}
}
