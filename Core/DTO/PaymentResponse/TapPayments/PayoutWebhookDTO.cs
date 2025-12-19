using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace invoice.Core.DTO.PaymentResponse.TapPayments
{
    public class PayoutWebhookDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("date")]
        public long Date { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("merchant_id")]
        public string Merchant_Id { get; set; }

        [JsonPropertyName("wallet")]
        public WalletDTO? Wallet { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class WalletDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("legacy_id")]
        public string? LegacyId { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("bank")]
        public Bank_webhookDto? Bank { get; set; }
    }

    public class Bank_webhookDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("swift")]
        public string? Swift { get; set; }

        [JsonPropertyName("beneficiary")]
        public BeneficiaryDto Beneficiary { get; set; }
    }

    public class BeneficiaryDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("iban")]
        public string Iban { get; set; }
    }
}
