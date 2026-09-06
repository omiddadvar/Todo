namespace Todo.Identity.Domain.Exceptions;

public class FullnameLastnameEmptyException : DomainException
{
    private const string CODE = "Identity.Domain.FullnameLastnameEmpty";
    public FullnameLastnameEmptyException(string message) : base(CODE, message)
    {

    }
}
