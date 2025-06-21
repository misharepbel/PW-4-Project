using MediatR;
using OrderService.Domain.Enums;
using System;

namespace OrderService.Application.Commands
{
    public sealed record UpdateOrderStatusCommand(Guid OrderId, OrderStatus NewStatus) : IRequest;
}
