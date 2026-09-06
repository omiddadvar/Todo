using Todo.Identity.Domain.Exceptions;

namespace Todo.Identity.Domain.Exceptions;

public class FullnameLastnameLengthException : DomainException
{
    private const string CODE = "Identity.Domain.FullnameLastnameLength";
    public FullnameLastnameLengthException(string message) : base(CODE, message)
    {

    }
}
