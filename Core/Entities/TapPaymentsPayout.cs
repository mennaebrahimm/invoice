namespace invoice.Core.Entities
{
    public class TapPaymentsPayout:BaseEntity
    {
        
        public string PayoutId { get; set; }
        public string Status { get; set; }
        public DateTime PayoutDate { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public string MerchantId { get; set; }

        public string? WalletId { get; set; }
        public string? WalletCountry { get; set; }

        public string? BankId { get; set; }
        public string? BankName { get; set; }
        public string? BankCountry { get; set; }
        public string? SwiftCode { get; set; }

        public string BeneficiaryName { get; set; }
        public string BeneficiaryIban { get; set; }
        public string InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        
    }
}
