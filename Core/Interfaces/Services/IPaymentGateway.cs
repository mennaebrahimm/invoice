using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.DTO.PaymentResponse.TapPayments;
using invoice.Core.Enums;

namespace invoice.Core.Interfaces.Services
{
    public interface IPaymentGateway
    {
        PaymentType PaymentType { get; }
        Task<GeneralResponse<PaymentSessionResponse>> CreatePaymentSessionAsync(PaymentCreateDTO dto);
        Task<GeneralResponse<PaymentStatusResponse>> GetPaymentStatusAsync(string paymentId);
        Task<GeneralResponse<bool>> HandleWebhookAsync(WebhookPayloadDTO payload);
        Task<GeneralResponse<bool>> CancelPaymentAsync(string paymentId);
        Task<GeneralResponse<object>> CreateLeadRetailerAsync(CreateLeadDTO dto, string userId);
        Task<object> CreateAccountRetailerAsync(string leadId, string userId);

        Task<string?> CreateConnectUrlAsync(string leadId, string userId);
        Task<GeneralResponse<string>> CreateLeadAsync(CreateLeadDTO dto, string userId);  //x
        Task<GeneralResponse<string>> CreatePaymentAsync(CreateChargeDTO dto);
        Task<GeneralResponse<object>> WebhookAsync(PayoutWebhookDTO dto);


    }
}