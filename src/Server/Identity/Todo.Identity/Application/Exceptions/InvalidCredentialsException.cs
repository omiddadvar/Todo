namespace Todo.Identity.Application.Exceptions;


public class InvalidCredentialsException : ApplicationException
{
    private const string CODE = "Identity.Application.InvalidCredentials";

    public InvalidCredentialsException(string message) : base(CODE, message)
    {
    }
}
