namespace Todo.Identity.Application.Exceptions;
public abstract class ApplicationException : Exception
{
    public string Code { get; }
    public ApplicationException(string code, string message) : base(message)
    {
        Code = code;
    }
}
