using PurchaseService.Domain.ValueObjects;

namespace PurchaseService.Domain.Services;

public interface ISubscriptionCalculator
{
    Money CalculateSubscriptionPrice(string productId, SubscriptionPeriod period, string currency);
    SubscriptionPeriod CalculateNextBillingPeriod(SubscriptionPeriod currentPeriod);
    Money CalculateProration(Money fullAmount, SubscriptionPeriod fullPeriod, DateTime startDate);
    bool IsEligibleForTrial(int userId, string productId);
    SubscriptionPeriod CreateTrialPeriod(string productId);
}

public class SubscriptionCalculator : ISubscriptionCalculator
{
    // This would typically be injected from configuration or a pricing service
    private static readonly Dictionary<string, PricingTier> ProductPricing = new()
    {
        { "premium", new PricingTier { MonthlyPrice = 9.99m, YearlyPrice = 99.99m } },
        { "pro", new PricingTier { MonthlyPrice = 19.99m, YearlyPrice = 199.99m } },
        { "enterprise", new PricingTier { MonthlyPrice = 49.99m, YearlyPrice = 499.99m } }
    };

    public Money CalculateSubscriptionPrice(string productId, SubscriptionPeriod period, string currency)
    {
        if (!ProductPricing.TryGetValue(productId.ToLowerInvariant(), out var pricing))
            throw new ArgumentException($"Unknown product ID: {productId}", nameof(productId));

        var baseAmount = period.Type switch
        {
            SubscriptionType.Monthly when period.Duration == 1 => pricing.MonthlyPrice,
            SubscriptionType.Yearly when period.Duration == 1 => pricing.YearlyPrice,
            SubscriptionType.Monthly => pricing.MonthlyPrice * period.Duration,
            SubscriptionType.Yearly => pricing.YearlyPrice * period.Duration,
            SubscriptionType.Daily => pricing.MonthlyPrice / 30 * period.Duration,
            SubscriptionType.Weekly => pricing.MonthlyPrice / 4 * period.Duration,
            _ => throw new ArgumentException($"Unsupported subscription type: {period.Type}")
        };

        return new Money(baseAmount, currency);
    }

    public SubscriptionPeriod CalculateNextBillingPeriod(SubscriptionPeriod currentPeriod)
    {
        return new SubscriptionPeriod(currentPeriod.EndDate, currentPeriod.Type, currentPeriod.Duration);
    }

    public Money CalculateProration(Money fullAmount, SubscriptionPeriod fullPeriod, DateTime startDate)
    {
        if (startDate < fullPeriod.StartDate || startDate > fullPeriod.EndDate)
            throw new ArgumentException("Start date must be within the subscription period");

        var totalDays = fullPeriod.TotalDuration.TotalDays;
        var remainingDays = (fullPeriod.EndDate - startDate).TotalDays;
        var proratedMultiplier = remainingDays / totalDays;

        return fullAmount.Multiply((decimal)proratedMultiplier);
    }

    public bool IsEligibleForTrial(int userId, string productId)
    {
        // This would typically check against a database of previous trials
        // For now, we'll assume everyone is eligible
        return true;
    }

    public SubscriptionPeriod CreateTrialPeriod(string productId)
    {
        // Standard 7-day trial period
        return SubscriptionPeriod.Daily(DateTime.UtcNow, 7);
    }
}

public class PricingTier
{
    public decimal MonthlyPrice { get; init; }
    public decimal YearlyPrice { get; init; }
}