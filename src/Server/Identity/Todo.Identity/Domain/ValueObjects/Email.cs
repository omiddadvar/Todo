using System.Text.RegularExpressions;
using Todo.Identity.Domain.Abstractions;
using Todo.Identity.Domain.Exceptions;

namespace Todo.Identity.Domain.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new EmailEmptyException("Email cannot be empty");

        if (!IsValidEmail(email))
            throw new EmailFormatException("Invalid email format");

        return new Email(email.ToLowerInvariant());
    }

    private static bool IsValidEmail(string email)
    {
        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email?.Value;
}
