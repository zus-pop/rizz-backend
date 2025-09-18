using PurchaseService.Domain.Services;
using PurchaseService.Domain.ValueObjects;

namespace PurchaseService.Infrastructure.Services;

public class SubscriptionCalculator : ISubscriptionCalculator
{
    // This would typically be injected from configuration or a pricing service
    private static readonly Dictionary<string, PricingTier> ProductPricing = new()
    {
        { "premium", new PricingTier { MonthlyPrice = 9.99m, YearlyPrice = 99.99m } },
        { "pro", new PricingTier { MonthlyPrice = 19.99m, YearlyPrice = 199.99m } },
        { "enterprise", new PricingTier { MonthlyPrice = 49.99m, YearlyPrice = 499.99m } }
    };

    // Trial periods configuration
    private static readonly Dictionary<string, int> TrialPeriods = new()
    {
        { "premium", 7 }, // 7 days
        { "pro", 14 },    // 14 days
        { "enterprise", 30 } // 30 days
    };

    public Money CalculateSubscriptionPrice(string productId, SubscriptionPeriod period, string currency)
    {
        if (!ProductPricing.TryGetValue(productId.ToLowerInvariant(), out var pricing))
            throw new ArgumentException($"Unknown product ID: {productId}", nameof(productId));

        var durationInDays = period.Duration;
        
        // Calculate based on monthly pricing as base
        var monthlyPrice = pricing.MonthlyPrice;
        var yearlyPrice = pricing.YearlyPrice;
        
        decimal baseAmount = durationInDays switch
        {
            // Daily pricing
            <= 31 when durationInDays <= 7 => monthlyPrice / 30 * durationInDays,
            // Weekly pricing  
            <= 31 when durationInDays <= 28 => monthlyPrice / 4 * (durationInDays / 7m),
            // Monthly pricing
            <= 365 when durationInDays <= 31 => monthlyPrice,
            // Yearly pricing (with discount)
            365 => yearlyPrice,
            // Multiple months
            > 31 and <= 365 => monthlyPrice * (durationInDays / 30m),
            // Multiple years
            > 365 => yearlyPrice * (durationInDays / 365m),
            _ => monthlyPrice
        };

        return new Money(Math.Round(baseAmount, 2), currency);
    }

    public SubscriptionPeriod CalculateNextBillingPeriod(SubscriptionPeriod currentPeriod)
    {
        var nextStartDate = currentPeriod.EndDate;
        var nextEndDate = nextStartDate.AddDays(currentPeriod.Duration);
        return new SubscriptionPeriod(nextStartDate, nextEndDate);
    }

    public Money CalculateProration(Money fullAmount, SubscriptionPeriod fullPeriod, DateTime startDate)
    {
        if (startDate < fullPeriod.StartDate || startDate > fullPeriod.EndDate)
            return fullAmount; // Full amount if outside period

        var totalDays = (fullPeriod.EndDate - fullPeriod.StartDate).TotalDays;
        var remainingDays = (fullPeriod.EndDate - startDate).TotalDays;

        if (remainingDays <= 0)
            return new Money(0, fullAmount.Currency);

        var prorationPercentage = remainingDays / totalDays;
        var proratedAmount = fullAmount.Amount * (decimal)prorationPercentage;

        return new Money(Math.Round(proratedAmount, 2), fullAmount.Currency);
    }

    public bool IsEligibleForTrial(int userId, string productId)
    {
        // This would typically check a database to see if user has already used trial
        // For now, simulate some logic
        
        if (!TrialPeriods.ContainsKey(productId.ToLowerInvariant()))
            return false;

        // Simple simulation - users with even IDs are eligible
        return userId % 2 == 0;
    }

    public SubscriptionPeriod CreateTrialPeriod(string productId)
    {
        if (!TrialPeriods.TryGetValue(productId.ToLowerInvariant(), out var trialDays))
            throw new ArgumentException($"No trial available for product: {productId}", nameof(productId));

        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(trialDays);
        
        return new SubscriptionPeriod(startDate, endDate);
    }

    // Additional helper methods for extended functionality
    public SubscriptionPeriod CalculateSubscriptionPeriod(DateTime startDate, TimeSpan duration)
    {
        var endDate = startDate.Add(duration);
        return new SubscriptionPeriod(startDate, endDate);
    }

    public SubscriptionPeriod CalculateSubscriptionPeriod(DateTime startDate, int durationInDays)
    {
        var endDate = startDate.AddDays(durationInDays);
        return new SubscriptionPeriod(startDate, endDate);
    }

    public Money CalculateProRatedAmount(Money originalAmount, SubscriptionPeriod originalPeriod, DateTime cancelDate)
    {
        return CalculateProration(originalAmount, originalPeriod, cancelDate);
    }

    public bool IsSubscriptionActive(SubscriptionPeriod subscriptionPeriod, DateTime? checkDate = null)
    {
        var dateToCheck = checkDate ?? DateTime.UtcNow;
        return dateToCheck >= subscriptionPeriod.StartDate && dateToCheck <= subscriptionPeriod.EndDate;
    }

    public TimeSpan GetRemainingDuration(SubscriptionPeriod subscriptionPeriod, DateTime? fromDate = null)
    {
        var fromDateTime = fromDate ?? DateTime.UtcNow;
        
        if (fromDateTime >= subscriptionPeriod.EndDate)
        {
            return TimeSpan.Zero;
        }

        if (fromDateTime <= subscriptionPeriod.StartDate)
        {
            return subscriptionPeriod.EndDate - subscriptionPeriod.StartDate;
        }

        return subscriptionPeriod.EndDate - fromDateTime;
    }

    public SubscriptionPeriod ExtendSubscription(SubscriptionPeriod currentPeriod, TimeSpan extensionDuration)
    {
        var newEndDate = currentPeriod.EndDate.Add(extensionDuration);
        return new SubscriptionPeriod(currentPeriod.StartDate, newEndDate);
    }

    public SubscriptionPeriod ExtendSubscription(SubscriptionPeriod currentPeriod, int extensionDays)
    {
        var newEndDate = currentPeriod.EndDate.AddDays(extensionDays);
        return new SubscriptionPeriod(currentPeriod.StartDate, newEndDate);
    }

    public Money CalculateUpgradeAmount(
        Money currentAmount, 
        Money newAmount, 
        SubscriptionPeriod currentPeriod, 
        DateTime upgradeDate)
    {
        if (upgradeDate <= currentPeriod.StartDate)
        {
            return newAmount; // Full new amount if upgraded before start
        }

        if (upgradeDate >= currentPeriod.EndDate)
        {
            return newAmount; // Full new amount if upgraded after end
        }

        var totalDays = (currentPeriod.EndDate - currentPeriod.StartDate).TotalDays;
        var remainingDays = (currentPeriod.EndDate - upgradeDate).TotalDays;

        if (remainingDays <= 0)
        {
            return newAmount;
        }

        var remainingPercentage = remainingDays / totalDays;
        
        // Calculate the unused portion of the current subscription
        var unusedCurrentAmount = currentAmount.Amount * (decimal)remainingPercentage;
        
        // Calculate the cost for the remaining period at the new rate
        var newPeriodAmount = newAmount.Amount * (decimal)remainingPercentage;
        
        // The upgrade amount is the difference
        var upgradeAmount = newPeriodAmount - unusedCurrentAmount;

        return new Money(Math.Max(0, Math.Round(upgradeAmount, 2)), newAmount.Currency);
    }

    public IEnumerable<SubscriptionPeriod> GenerateRecurringPeriods(
        DateTime startDate, 
        TimeSpan recurringInterval, 
        int numberOfPeriods)
    {
        var periods = new List<SubscriptionPeriod>();
        var currentStartDate = startDate;

        for (int i = 0; i < numberOfPeriods; i++)
        {
            var currentEndDate = currentStartDate.Add(recurringInterval);
            periods.Add(new SubscriptionPeriod(currentStartDate, currentEndDate));
            currentStartDate = currentEndDate;
        }

        return periods;
    }
}

internal class PricingTier
{
    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }
}