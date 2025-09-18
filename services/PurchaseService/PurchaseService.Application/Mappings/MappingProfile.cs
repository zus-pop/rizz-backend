using AutoMapper;
using PurchaseService.Application.DTOs;
using PurchaseService.Domain.Entities;
using PurchaseService.Domain.ValueObjects;

namespace PurchaseService.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Purchase mappings
        CreateMap<Purchase, PurchaseDto>()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Amount.Currency))
            .ForMember(dest => dest.PaymentMethodType, opt => opt.MapFrom(src => src.PaymentMethod.Type.ToString()))
            .ForMember(dest => dest.PaymentProvider, opt => opt.MapFrom(src => src.PaymentMethod.Provider))
            .ForMember(dest => dest.ExternalTransactionId, opt => opt.MapFrom(src => src.PaymentMethod.ExternalTransactionId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Status.ToString()))
            .ForMember(dest => dest.StatusReason, opt => opt.MapFrom(src => src.Status.Reason))
            .ForMember(dest => dest.Subscription, opt => opt.MapFrom(src => src.SubscriptionPeriod))
            .ForMember(dest => dest.Refund, opt => opt.MapFrom(src => src.Refund));

        // Subscription mappings
        CreateMap<SubscriptionPeriod, SubscriptionDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.IsExpired))
            .ForMember(dest => dest.RemainingTime, opt => opt.MapFrom(src => src.RemainingTime));

        // Refund mappings
        CreateMap<Refund, RefundDto>()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Amount.Currency));

        // Command to domain mappings
        CreateMap<CreatePurchaseDto, Purchase>()
            .ConstructUsing((src, context) =>
            {
                var paymentMethodType = Enum.Parse<PaymentMethodType>(src.PaymentMethodType, true);
                var paymentMethod = new PaymentMethod(
                    paymentMethodType,
                    src.PaymentProvider,
                    src.ExternalTransactionId,
                    src.Metadata);

                var amount = new Money(src.Amount, src.Currency);

                SubscriptionPeriod? subscriptionPeriod = null;
                if (src.Subscription != null)
                {
                    var subscriptionType = Enum.Parse<SubscriptionType>(src.Subscription.Type, true);
                    subscriptionPeriod = new SubscriptionPeriod(
                        src.Subscription.StartDate,
                        subscriptionType,
                        src.Subscription.Duration);
                }

                return new Purchase(
                    src.UserId,
                    amount,
                    paymentMethod,
                    src.ProductId,
                    src.ProductName,
                    subscriptionPeriod,
                    src.Metadata);
            });

        // PaymentMethod mappings
        CreateMap<PaymentMethod, PaymentMethodDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

        CreateMap<PaymentMethodDto, PaymentMethod>()
            .ConstructUsing(src => new PaymentMethod(
                Enum.Parse<PaymentMethodType>(src.Type, true), 
                src.Provider, 
                src.ExternalTransactionId, 
                src.Metadata));
    }
}