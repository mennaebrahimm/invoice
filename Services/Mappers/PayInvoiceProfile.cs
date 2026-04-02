using AutoMapper;
using invoice.Core.DTO.PayInvoice;
using invoice.Core.Entities;

namespace invoice.Services.Mappers
{
    public class PayInvoiceProfile : Profile
    {
        public PayInvoiceProfile()
        {
            CreateMap<PayInvoice, PayInvoiceReadDTO>().ReverseMap();
            CreateMap<PayInvoiceCreateDTO, PayInvoice>();
            CreateMap<PayInvoiceUpdateDTO, PayInvoice>();
            CreateMap<PayInvoice, PayInvoiceReadDTO>()
            .ForMember(dest => dest.PaymentMethodName,
               opt => opt.MapFrom(src => src.PaymentMethod.Name));
        }
    }
}