using invoice.Core.DTO.User;
using System.Text.Json.Serialization;

namespace invoice.Core.DTO.PaymentResponse.TapPayments
{
    public class CreateLeadDTO
    {
        public string country { get; set; }
        public Brand brand { get; set; }
        public Entity entity { get; set; }
        public List<User> users { get; set; }
        public Wallet wallet { get; set; }
    }

 

    public class Brand
    {
        public List<LocalizedTextDto> name { get; set; }  
        //public string logo { get; set; }  //for testing
        public IFormFile logo { get; set; }
        public List<ChannelService> channel_services { get; set; }
    }

    public class Name
    {
        public string first { get; set; }
        public string middle { get; set; }
        public string last { get; set; }
        public string lang { get; set; }
    }

    public class ChannelService
    {
        public string channel { get; set; }
        public string address { get; set; }
    }

    public class Entity
    {
        public List<LocalizedTextDto> legal_name { get; set; } 
        [JsonPropertyName("license")]
        public  License license { get; set; }
    }
    public class LocalizedTextDto
    {
        public string text { get; set; }
        public string lang { get; set; } = "en";
    }

    public class License
    {
        [JsonPropertyName("number")]
        public string number { get; set; }
        public string unified_nunber { get; set; }
        public string country { get; set; }
        public string name { get; set; }
    }

    public class User
    {
        public List<Name> name { get; set; }
        public ContactDto Contact { get; set; }
        public Identification identification { get; set; }
        public BirthDto Birth { get; set; }
        public string primary { get; set; }
    }
    public class ContactDto
    {
        public List<EmailDto> Email { get; set; }
        public List<PhoneDto> Phone { get; set; }
    }

    public class EmailDto
    {
        public string Address { get; set; }
        public bool Primary { get; set; }
    }

    public class PhoneDto
    {
        [JsonPropertyName("country_code")]
        public string Country_Code { get; set; }
        public string Number { get; set; }
        public bool Primary { get; set; }
    }
    public class Identification
    {
        public string number { get; set; }
        public string type { get; set; }
        public string country { get; set; }
        public string nationality { get; set; }
    }
    public class BirthDto
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Date { get; set; }
    }
    public class Wallet
    {
        public List<LocalizedTextDto> name { get; set; }
        public LinkedFinancialAccount linked_financial_account { get; set; }
    }

    public class LinkedFinancialAccount
    {
        public Bank bank { get; set; }
    }

    public class Bank
    {
        public Account account { get; set; }
        public List<BankDocumentDto> Documents { get; set; }
    }
    public class BankDocumentDto
    {
        public string Name { get; set; }
        public string Issuing_Country { get; set; }
       // public List<string> Images { get; set; }  //?
        public string Number { get; set; }
        public string Issuing_Date { get; set; }
    }
    public class Account
    {
        public string name { get; set; }
        public string iban { get; set; }
    }

  

}




