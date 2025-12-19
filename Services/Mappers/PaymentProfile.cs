using AutoMapper;
using invoice.Core.DTO.Payment;
using invoice.Core.DTO.PaymentResponse;
using invoice.Core.DTO.PaymentResponse.TapPayments;
using invoice.Core.Entities;
using Stripe;

namespace invoice.Services.Mappers
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Payment, PaymentReadDTO>().ReverseMap();
            CreateMap<PaymentCreateDTO, Payment>();
            CreateMap<PaymentUpdateDTO, Payment>();

            CreateMap<PaymentSessionResponse, PaymentReadDTO>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.PaymentType.ToString()))
                .ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.PaymentUrl))
                .ForMember(dest => dest.GatewaySessionId, opt => opt.MapFrom(src => src.SessionId))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.PaymentStatus))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.PaymentType))
                .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.InvoiceId))
                .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => src.ExpiresAt));

            #region Tap Payments
            CreateMap<PayoutWebhookDTO, TapPaymentsPayout>()
            .ForMember(dest => dest.PayoutId,
                opt => opt.MapFrom(src => src.Id))

            .ForMember(dest => dest.PayoutDate,
                opt => opt.MapFrom(src =>
                    DateTimeOffset
                        .FromUnixTimeMilliseconds(src.Date)
                        .UtcDateTime))

            .ForMember(dest => dest.MerchantId,
                opt => opt.MapFrom(src => src.Merchant_Id))

            // Wallet
            .ForMember(dest => dest.WalletId,
                opt => opt.MapFrom(src => src.Wallet.Id))

            .ForMember(dest => dest.WalletCountry,
                opt => opt.MapFrom(src => src.Wallet.Country))

            // Bank
            .ForMember(dest => dest.BankId,
                opt => opt.MapFrom(src => src.Wallet.Bank.Id))

            .ForMember(dest => dest.BankName,
                opt => opt.MapFrom(src => src.Wallet.Bank.Name))

            .ForMember(dest => dest.BankCountry,
                opt => opt.MapFrom(src => src.Wallet.Bank.Country))

            .ForMember(dest => dest.SwiftCode,
                opt => opt.MapFrom(src => src.Wallet.Bank.Swift))

            // Beneficiary
            .ForMember(dest => dest.BeneficiaryName,
                opt => opt.MapFrom(src => src.Wallet.Bank.Beneficiary.Name))

            .ForMember(dest => dest.BeneficiaryIban,
                opt => opt.MapFrom(src => src.Wallet.Bank.Beneficiary.Iban))

            // Ignore system fields
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
        #endregion
    }
    }

