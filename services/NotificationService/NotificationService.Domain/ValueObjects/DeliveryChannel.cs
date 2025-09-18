namespace NotificationService.Domain.ValueObjects;

public enum DeliveryChannelType
{
    InApp,
    Push,
    Email,
    SMS
}

public class DeliveryChannel : IEquatable<DeliveryChannel>
{
    public DeliveryChannelType Type { get; }
    public string? Address { get; }
    public bool IsEnabled { get; }
    public Dictionary<string, string> Settings { get; }

    public DeliveryChannel(DeliveryChannelType type, string? address = null, bool isEnabled = true, Dictionary<string, string>? settings = null)
    {
        Type = type;
        Address = address?.Trim();
        IsEnabled = isEnabled;
        Settings = settings ?? new Dictionary<string, string>();

        ValidateChannel();
    }

    private void ValidateChannel()
    {
        switch (Type)
        {
            case DeliveryChannelType.Email:
                if (string.IsNullOrWhiteSpace(Address) || !IsValidEmail(Address))
                    throw new ArgumentException("Valid email address is required for email channel");
                break;
            case DeliveryChannelType.SMS:
                if (string.IsNullOrWhiteSpace(Address) || !IsValidPhoneNumber(Address))
                    throw new ArgumentException("Valid phone number is required for SMS channel");
                break;
        }
    }

    public static DeliveryChannel InApp() => new(DeliveryChannelType.InApp);
    public static DeliveryChannel Push() => new(DeliveryChannelType.Push);
    public static DeliveryChannel Email(string emailAddress) => new(DeliveryChannelType.Email, emailAddress);
    public static DeliveryChannel SMS(string phoneNumber) => new(DeliveryChannelType.SMS, phoneNumber);

    public bool RequiresAddress => Type is DeliveryChannelType.Email or DeliveryChannelType.SMS;
    public bool IsInstantDelivery => Type is DeliveryChannelType.InApp or DeliveryChannelType.Push;

    public DeliveryChannel WithSetting(string key, string value)
    {
        var newSettings = new Dictionary<string, string>(Settings) { [key] = value };
        return new DeliveryChannel(Type, Address, IsEnabled, newSettings);
    }

    public DeliveryChannel Enable() => new(Type, Address, true, Settings);
    public DeliveryChannel Disable() => new(Type, Address, false, Settings);

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        return !string.IsNullOrWhiteSpace(phoneNumber) &&
               phoneNumber.All(c => char.IsDigit(c) || c == '+' || c == '-' || c == ' ' || c == '(' || c == ')') &&
               phoneNumber.Length >= 10 && phoneNumber.Length <= 15;
    }

    public bool Equals(DeliveryChannel? other)
    {
        return other is not null &&
               Type == other.Type &&
               Address == other.Address &&
               IsEnabled == other.IsEnabled;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as DeliveryChannel);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Address, IsEnabled);
    }

    public override string ToString() => $"{Type}" + (Address != null ? $" ({Address})" : "");
}