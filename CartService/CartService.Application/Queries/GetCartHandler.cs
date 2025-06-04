using CartService.Application.DTOs;
using CartService.Application.Services;
using MediatR;

namespace CartService.Application.Queries;

public class GetCartHandler : IRequestHandler<GetCartQuery, CartDto?>
{
    private readonly ICartService _service;
    public GetCartHandler(ICartService service) => _service = service;

    public Task<CartDto?> Handle(GetCartQuery request, CancellationToken cancellationToken)
        => _service.GetAsync(request.UserId, cancellationToken);
}
