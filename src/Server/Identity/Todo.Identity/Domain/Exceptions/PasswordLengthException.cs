namespace Todo.Identity.Domain.Exceptions;

public class PasswordLengthException : DomainException
{
    private const string CODE = "Identity.Domain.PasswordLength";
    public PasswordLengthException(string message) : base(CODE, message)
    {

    }
}
