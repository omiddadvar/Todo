using Microsoft.AspNetCore.Identity;
using Todo.Identity.Application.Abstractions;
using Todo.Identity.Application.Exceptions;
using Todo.Identity.Domain.Abstractions;
using Todo.Identity.Domain.Entities;

namespace Todo.Identity.Application.Commands;

public static class Logout
{
    public record Command : ICommand<Result>
    {
        public Guid UserId { get; set; }
    }

    public class Handler(
        UserManager<User> userManager,
        IUnitOfWork unitOfWork) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                throw new UserNotFoundException($"User with ID {request.UserId} not found");

            user.RevokeRefreshToken();
            await userManager.UpdateAsync(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}