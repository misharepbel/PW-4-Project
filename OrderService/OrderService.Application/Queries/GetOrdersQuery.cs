using MediatR;
using OrderService.Application.DTO;
using System.Collections.Generic;

namespace OrderService.Application.Queries
{
    public sealed record GetOrdersQuery() : IRequest<List<OrderDto>>;
}
