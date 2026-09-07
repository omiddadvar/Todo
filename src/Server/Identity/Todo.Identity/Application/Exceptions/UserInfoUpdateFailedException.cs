namespace Todo.Identity.Application.Exceptions;

public class UserInfoUpdateFailedException : ApplicationException
{
    private const string CODE = "Identity.Application.UserInfoUpdateFailed";

    public UserInfoUpdateFailedException(string message) : base(CODE, message)
    {
    }
}
