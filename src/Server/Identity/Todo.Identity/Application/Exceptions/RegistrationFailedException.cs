namespace Todo.Identity.Application.Exceptions;

public class RegistrationFailedException : ApplicationException
{
    private const string CODE = "Identity.Application.RegistrationFailed";

    public RegistrationFailedException(string message) : base(CODE, message)
    {
    }
}
