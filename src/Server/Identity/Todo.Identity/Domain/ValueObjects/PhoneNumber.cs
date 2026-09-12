using Todo.Identity.Domain.Abstractions;
using Todo.Identity.Domain.Exceptions;

namespace Todo.Identity.Domain.ValueObjects;

public class PhoneNumber : ValueObject
{
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new PhoneNumberEmptyException("Phone number cannot be empty");

        var cleaned = new string(phoneNumber.Where(char.IsDigit).ToArray());

        if (cleaned.Length < 10)
            throw new PhoneNumberLengthException("Phone number must have at least 10 digits");

        if (cleaned.Length > 15)
            throw new PhoneNumberLengthException("Phone number must not exceed 15 digits");

        return new PhoneNumber(cleaned);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}