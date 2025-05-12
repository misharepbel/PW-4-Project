using MediatR;
using OrderService.Application.DTO;
using System;

namespace OrderService.Application.Commands
{
    public sealed record CreateOrderCommand(CreateOrderDto Order) : IRequest<Guid>;
}
