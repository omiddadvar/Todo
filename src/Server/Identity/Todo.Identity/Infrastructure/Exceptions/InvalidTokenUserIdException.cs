namespace Todo.Identity.Infrastructure.Exceptions;

public class InvalidTokenUserIdException : InfrastructureException
{
    private const string CODE = "Identity.Infrastructure.InvalidTokenUserId";

    public InvalidTokenUserIdException(string message) : base(CODE, message)
    {
    }
}

