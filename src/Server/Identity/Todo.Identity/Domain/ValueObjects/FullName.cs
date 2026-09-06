using Todo.Identity.Domain.Abstractions;
using Todo.Identity.Domain.Exceptions;

namespace Todo.Identity.Domain.ValueObjects;

public class FullName : ValueObject
{
    public string FirstName { get; }
    public string LastName { get; }

    private FullName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static FullName Create(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new FullnameFirstnameEmptyException("First name cannot be empty");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new FullnameLastnameEmptyException("Last name cannot be empty");

        if (firstName.Length > 100)
            throw new FullnameFirstnameLengthException("First name must not exceed 100 characters");

        if (lastName.Length > 100)
            throw new FullnameLastnameLengthException("Last name must not exceed 100 characters");

        return new FullName(firstName.Trim(), lastName.Trim());
    }

    public string Full => $"{FirstName} {LastName}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }

    public override string ToString() => Full;
}
