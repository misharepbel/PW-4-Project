using MediatR;
using OrderService.Application.DTO;
using System;
using System.Collections.Generic;

namespace OrderService.Application.Queries
{
    public sealed record GetOrdersByUserIdQuery(Guid UserId) : IRequest<List<OrderDto>>;
}
