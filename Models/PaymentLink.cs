namespace invoice.Models
{
    public class PaymentLink
    {
        public int Id { get; set; }

        public string   Link { get; set; }

        public double Value { get; set; }

        public string PaymentsNumber {  get; set; }

        public string Description {  get; set; }
        public string Message {  get; set; }
        public String Image { get; set; }
        public String TermsAndConditions { get; set; }


        public bool IsDelete {  get; set; }




    }
}
