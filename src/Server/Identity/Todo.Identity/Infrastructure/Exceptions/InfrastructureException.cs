namespace Todo.Identity.Infrastructure.Exceptions;

public abstract class InfrastructureException : Exception
{
    public string Code { get; }
    public InfrastructureException(string code, string message) : base(message)
    {
        Code = code;
    }
}
