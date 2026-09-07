namespace Todo.Identity.Application.Exceptions;

public class PasswordResetFailedException : ApplicationException
{
    private const string CODE = "Identity.Application.PasswordResetFailed";

    public PasswordResetFailedException(string message) : base(CODE, message)
    {
    }
}
