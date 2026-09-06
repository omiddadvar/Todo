namespace Todo.Identity.Domain.Exceptions;

public class PasswordEmptyException : DomainException
{
    private const string CODE = "Identity.Domain.PasswordEmpty"; 
    public PasswordEmptyException(string message) : base(CODE , message)
    {
        
    }
}
