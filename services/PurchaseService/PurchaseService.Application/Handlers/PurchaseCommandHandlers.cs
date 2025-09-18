using MediatR;
using AutoMapper;
using PurchaseService.Application.Commands;
using PurchaseService.Application.DTOs;
using PurchaseService.Domain.Entities;
using PurchaseService.Application.Repositories;
using PurchaseService.Domain.Services;
using PurchaseService.Domain.ValueObjects;

namespace PurchaseService.Application.Handlers;

public class CreatePurchaseCommandHandler : IRequestHandler<CreatePurchaseCommand, PurchaseDto>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly ISubscriptionCalculator _subscriptionCalculator;
    private readonly IMapper _mapper;

    public CreatePurchaseCommandHandler(
        IPurchaseRepository purchaseRepository,
        ISubscriptionCalculator subscriptionCalculator,
        IMapper mapper)
    {
        _purchaseRepository = purchaseRepository;
        _subscriptionCalculator = subscriptionCalculator;
        _mapper = mapper;
    }

    public async Task<PurchaseDto> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
    {
        // Create payment method
        var paymentMethodType = Enum.Parse<PaymentMethodType>(request.PaymentMethodType, true);
        var paymentMethod = new PaymentMethod(
            paymentMethodType,
            request.PaymentProvider,
            request.ExternalTransactionId,
            request.Metadata);

        // Create money
        var amount = new Money(request.Amount, request.Currency);

        // Create subscription period if provided
        SubscriptionPeriod? subscriptionPeriod = null;
        if (request.Subscription != null)
        {
            var subscriptionType = Enum.Parse<SubscriptionType>(request.Subscription.Type, true);
            subscriptionPeriod = new SubscriptionPeriod(
                request.Subscription.StartDate,
                subscriptionType,
                request.Subscription.Duration);
        }

        // Create purchase
        var purchase = new Purchase(
            request.UserId,
            amount,
            paymentMethod,
            request.ProductId,
            request.ProductName,
            subscriptionPeriod,
            request.Metadata);

        // Save to repository
        await _purchaseRepository.AddAsync(purchase, cancellationToken);
        await _purchaseRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PurchaseDto>(purchase);
    }
}

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, PurchaseDto>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IMapper _mapper;

    public ProcessPaymentCommandHandler(
        IPurchaseRepository purchaseRepository,
        IPaymentProcessor paymentProcessor,
        IMapper mapper)
    {
        _purchaseRepository = purchaseRepository;
        _paymentProcessor = paymentProcessor;
        _mapper = mapper;
    }

    public async Task<PurchaseDto> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var purchase = await _purchaseRepository.GetByIdAsync(request.PurchaseId, cancellationToken);
        if (purchase == null)
            throw new ArgumentException($"Purchase with ID {request.PurchaseId} not found");

        // Start processing
        purchase.StartProcessing();
        await _purchaseRepository.UpdateAsync(purchase, cancellationToken);

        try
        {
            // Process payment
            var paymentResult = await _paymentProcessor.ProcessPaymentAsync(purchase, cancellationToken);

            if (paymentResult.IsSuccessful)
            {
                purchase.Complete();
                
                // Update external transaction ID if provided
                if (!string.IsNullOrEmpty(paymentResult.ExternalTransactionId))
                {
                    foreach (var metadata in paymentResult.Metadata)
                    {
                        purchase.AddMetadata(metadata.Key, metadata.Value);
                    }
                }
            }
            else
            {
                purchase.Fail(paymentResult.FailureReason ?? "Payment processing failed");
            }

            await _purchaseRepository.UpdateAsync(purchase, cancellationToken);
        }
        catch (Exception ex)
        {
            purchase.Fail($"Payment processing error: {ex.Message}");
            await _purchaseRepository.UpdateAsync(purchase, cancellationToken);
            throw;
        }

        return _mapper.Map<PurchaseDto>(purchase);
    }
}

public class CancelPurchaseCommandHandler : IRequestHandler<CancelPurchaseCommand, PurchaseDto>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IMapper _mapper;

    public CancelPurchaseCommandHandler(IPurchaseRepository purchaseRepository, IMapper mapper)
    {
        _purchaseRepository = purchaseRepository;
        _mapper = mapper;
    }

    public async Task<PurchaseDto> Handle(CancelPurchaseCommand request, CancellationToken cancellationToken)
    {
        var purchase = await _purchaseRepository.GetByIdAsync(request.PurchaseId, cancellationToken);
        if (purchase == null)
            throw new ArgumentException($"Purchase with ID {request.PurchaseId} not found");

        purchase.Cancel(request.Reason);
        await _purchaseRepository.UpdateAsync(purchase, cancellationToken);

        return _mapper.Map<PurchaseDto>(purchase);
    }
}

public class RefundPurchaseCommandHandler : IRequestHandler<RefundPurchaseCommand, PurchaseDto>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IMapper _mapper;

    public RefundPurchaseCommandHandler(
        IPurchaseRepository purchaseRepository,
        IPaymentProcessor paymentProcessor,
        IMapper mapper)
    {
        _purchaseRepository = purchaseRepository;
        _paymentProcessor = paymentProcessor;
        _mapper = mapper;
    }

    public async Task<PurchaseDto> Handle(RefundPurchaseCommand request, CancellationToken cancellationToken)
    {
        var purchase = await _purchaseRepository.GetByIdAsync(request.PurchaseId, cancellationToken);
        if (purchase == null)
            throw new ArgumentException($"Purchase with ID {request.PurchaseId} not found");

        var refundAmount = request.RefundAmount.HasValue 
            ? new Money(request.RefundAmount.Value, purchase.Amount.Currency)
            : purchase.Amount;

        // Process refund through payment processor
        var refundResult = await _paymentProcessor.ProcessRefundAsync(purchase, refundAmount, request.Reason, cancellationToken);

        if (refundResult.IsSuccessful)
        {
            purchase.ProcessRefund(refundAmount, request.Reason);
            
            // Set external refund ID if provided
            if (!string.IsNullOrEmpty(refundResult.ExternalRefundId) && purchase.Refund != null)
            {
                purchase.Refund.SetExternalRefundId(refundResult.ExternalRefundId);
            }
        }
        else
        {
            throw new InvalidOperationException($"Refund failed: {refundResult.FailureReason}");
        }

        await _purchaseRepository.UpdateAsync(purchase, cancellationToken);

        return _mapper.Map<PurchaseDto>(purchase);
    }
}