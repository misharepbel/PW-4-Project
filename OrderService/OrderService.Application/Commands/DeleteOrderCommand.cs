using MediatR;
using System;

namespace OrderService.Application.Commands
{
    public sealed record DeleteOrderCommand(Guid OrderId) : IRequest;
}
