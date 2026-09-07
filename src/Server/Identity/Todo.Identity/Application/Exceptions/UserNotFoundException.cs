namespace Todo.Identity.Application.Exceptions;

public class UserNotFoundException : ApplicationException
{
    private const string CODE = "Identity.Application.UserNotFound";

    public UserNotFoundException(string message) : base(CODE, message)
    {
    }
}
