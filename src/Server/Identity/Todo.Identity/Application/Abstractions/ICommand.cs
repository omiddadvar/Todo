using MediatR;

namespace Todo.Identity.Application.Abstractions;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
