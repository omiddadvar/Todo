namespace Todo.Identity.Application.Exceptions;


public class ConfirmPasswordNotCorrectException : ApplicationException
{
    private const string CODE = "Identity.Application.ConfirmPasswordNotCorrect";

    public ConfirmPasswordNotCorrectException(string message) : base(CODE, message)
    {
    }
}
