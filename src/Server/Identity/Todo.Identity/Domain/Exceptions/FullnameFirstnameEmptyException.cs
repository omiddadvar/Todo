namespace Todo.Identity.Domain.Exceptions;

public class FullnameFirstnameEmptyException : DomainException
{
    private const string CODE = "Identity.Domain.FullnameFirstnameEmpty";
    public FullnameFirstnameEmptyException(string message) : base(CODE, message)
    {

    }
}
