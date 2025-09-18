namespace UserService.Domain.ValueObjects
{
    public class PhoneNumber
    {
        public string Value { get; private set; }

        private PhoneNumber(string value)
        {
            Value = value;
        }

        public static PhoneNumber Create(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

            var cleanedNumber = CleanPhoneNumber(phoneNumber);
            
            if (!IsValidPhoneNumber(cleanedNumber))
                throw new ArgumentException("Invalid phone number format", nameof(phoneNumber));

            return new PhoneNumber(cleanedNumber);
        }

        private static string CleanPhoneNumber(string phoneNumber)
        {
            return new string(phoneNumber.Where(char.IsDigit).ToArray());
        }

        private static bool IsValidPhoneNumber(string phoneNumber)
        {
            return phoneNumber.Length >= 10 && phoneNumber.Length <= 15;
        }

        public override string ToString() => Value;

        public override bool Equals(object? obj)
        {
            if (obj is PhoneNumber other)
                return Value == other.Value;
            return false;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
    }
}