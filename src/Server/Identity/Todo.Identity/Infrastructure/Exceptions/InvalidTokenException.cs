namespace Todo.Identity.Infrastructure.Exceptions;

public class InvalidTokenException : InfrastructureException
{
    private const string CODE = "Identity.Infrastructure.InvalidToken";

    public InvalidTokenException(string message) : base(CODE, message)
    {
    }
}
