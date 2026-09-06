namespace Todo.Identity.Domain.Exceptions;

public class FullnameFirstnameLengthException : DomainException
{
    private const string CODE = "Identity.Domain.FullnameFirstnameLength";
    public FullnameFirstnameLengthException(string message) : base(CODE, message)
    {

    }
}
