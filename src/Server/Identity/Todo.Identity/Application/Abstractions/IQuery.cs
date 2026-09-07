using MediatR;

namespace Todo.Identity.Application.Abstractions;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}