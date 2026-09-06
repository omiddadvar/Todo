using Todo.Identity.Domain.Abstractions;
using Todo.Identity.Domain.Exceptions;

namespace Todo.Identity.Domain.ValueObjects;

public class Password : ValueObject
{
    public string Hash { get; private set; }

    private Password(string hash)
    {
        Hash = hash;
    }

    public static Password CreateFromPlainText(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new PasswordEmptyException("Password cannot be empty");

        if (password.Length < 8)
            throw new PasswordLengthException("Password must be at least 8 characters");

        if (password.Length > 100)
            throw new PasswordLengthException("Password must not exceed 100 characters");

        return new Password(password);
    }

    public static Password CreateFromHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new PasswordEmptyException("Password hash cannot be empty");

        return new Password(hash);
    }

    public void SetHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new PasswordEmptyException("Password hash cannot be empty");

        Hash = hash;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Hash;
    }

    // For more security - Hide password
    public override string ToString() => "******"; 
}
