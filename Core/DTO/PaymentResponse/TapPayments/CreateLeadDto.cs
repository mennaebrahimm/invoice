using invoice.Core.DTO.User;

namespace invoice.Core.DTO.PaymentResponse.TapPayments
{

    public class CreateLeadDto
    {
        public string Country { get; set; }
        public BrandDto Brand { get; set; }
        public EntityDto Entity { get; set; }
        public List<UserDto> Users { get; set; }
        public WalletDto Wallet { get; set; }
        //public MarketplaceDto Marketplace { get; set; }
        //public PostDto Post { get; set; }
    }
    public class BrandDto
    {
        public List<LocalizedTextDto> Name { get; set; }
        public string Logo { get; set; }  //file
        public List<ChannelServiceDto> Channel_Services { get; set; }
    }

    public class ChannelServiceDto
    {
        public string Channel { get; set; }
        public string Address { get; set; }
    }
    public class EntityDto
    {
        public List<LocalizedTextDto> Legal_Name { get; set; }
        public LicenseDto License { get; set; }
    }

    public class LicenseDto
    {
        public string Number { get; set; }
        public string Unified_Nunber { get; set; } // Tap typo
        public string Country { get; set; }
        public string Name { get; set; }
        //public List<LicenseDocumentDto> Documents { get; set; }
    }

    //public class LicenseDocumentDto
    //{
    //    public string Name { get; set; }
    //    public string Number { get; set; }
    //    public string Issuing_Country { get; set; }
    //    public List<string> Images { get; set; }
    //}
    public class UserDto
    {
        public List<UserNameDto> Name { get; set; }
        public ContactDto Contact { get; set; }
        public IdentificationDto Identification { get; set; }
        public BirthDto Birth { get; set; }
        public bool Primary { get; set; }
    }

    public class UserNameDto
    {
        public string First { get; set; }
        public string Middle { get; set; }
        public string Last { get; set; }
        public string Lang { get; set; }= "en"; 
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
        public string Country_Code { get; set; }
        public string Number { get; set; }
        public bool Primary { get; set; }
    }
    public class IdentificationDto
    {
        public string Number { get; set; }
        public string Type { get; set; }
        public string Country { get; set; }
        public string Nationality { get; set; }
    }

    public class BirthDto
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Date { get; set; }
    }
    public class WalletDto
    {
        public List<LocalizedTextDto> Name { get; set; }
        public LinkedFinancialAccountDto Linked_Financial_Account { get; set; }
    }

    public class LinkedFinancialAccountDto
    {
        public BankDto Bank { get; set; }
    }

    public class BankDto
    {
        public BankAccountDto Account { get; set; }
        public List<BankDocumentDto> Documents { get; set; }
    }

    public class BankAccountDto
    {
        public string Name { get; set; }
        public string Iban { get; set; }
    }

    public class BankDocumentDto
    {
        public string Name { get; set; }
        public string Issuing_Country { get; set; }
        public List<string> Images { get; set; }  //?
        public string Number { get; set; }
        public string Issuing_Date { get; set; }
    }
    public class LocalizedTextDto
    {
        public string Text { get; set; }
        public string Lang { get; set; } = "en";
    }
    //public class MarketplaceDto
    //{
    //    public string Id { get; set; }
    //}

    //public class PostDto
    //{
    //    public string Url { get; set; }
    //}

}
