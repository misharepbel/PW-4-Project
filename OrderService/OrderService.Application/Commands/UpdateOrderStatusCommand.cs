using MediatR;
using System;

namespace OrderService.Application.Commands
{
    public sealed record UpdateOrderStatusCommand(Guid OrderId, string NewStatus) : IRequest;
}
