namespace Todo.Identity.Application.Exceptions;

public class EmailAlreadyExistsException : ApplicationException
{
    private const string CODE = "Identity.Application.EmailAlreadyExists";

    public EmailAlreadyExistsException(string message) : base(CODE, message)
    {
    }
}
