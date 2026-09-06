namespace Todo.Identity.Domain.Exceptions;

public class PhoneNumberEmptyException : DomainException
{
    private const string CODE = "Identity.Domain.PhoneNumberEmpty";
    public PhoneNumberEmptyException(string message) : base(CODE, message)
    {

    }
}