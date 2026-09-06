namespace Todo.Identity.Domain.Exceptions;

public class PhoneNumberLengthException : DomainException
{
    private const string CODE = "Identity.Domain.PhoneNumberLength";
    public PhoneNumberLengthException(string message) : base(CODE, message)
    {

    }
}