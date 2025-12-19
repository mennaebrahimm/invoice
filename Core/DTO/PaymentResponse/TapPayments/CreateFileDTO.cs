namespace invoice.Core.DTO.PaymentResponse.TapPayments
{
    public class CreateFileDTO
    {
        public string Purpose { get; set; }
        public string Title { get; set; } = "logo";
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddYears(1);
        public bool FileLinkCreate { get; set; } = true;

     
        public IFormFile File { get; set; }
    }
}
