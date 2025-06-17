using MediatR;
using OrderService.Application.DTO;
using System;

namespace OrderService.Application.Queries
{
    public sealed record GetOrderByIdQuery(Guid OrderId, Guid UserId, bool IsAdmin) : IRequest<OrderDto?>;
}
