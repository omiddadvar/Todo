namespace Todo.Identity.Application.Exceptions;

public class AccountDeactivatedException : ApplicationException
{
    private const string CODE = "Identity.Application.AccountDeactivated";

    public AccountDeactivatedException(string message) : base(CODE, message)
    {
    }
}