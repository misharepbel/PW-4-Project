using CartService.Application.DTOs;
using CartService.Application.Services;
using MediatR;

namespace CartService.Application.Queries;

public class GetAllCartsHandler : IRequestHandler<GetAllCartsQuery, List<CartDto>>
{
    private readonly ICartService _service;
    public GetAllCartsHandler(ICartService service) => _service = service;

    public Task<List<CartDto>> Handle(GetAllCartsQuery request, CancellationToken cancellationToken)
        => _service.GetAllAsync(cancellationToken);
}
