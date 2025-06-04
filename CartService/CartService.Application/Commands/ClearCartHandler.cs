using CartService.Application.Services;
using MediatR;

namespace CartService.Application.Commands;

public class ClearCartHandler : IRequestHandler<ClearCartCommand>
{
    private readonly ICartService _service;
    public ClearCartHandler(ICartService service) => _service = service;

    public async Task Handle(ClearCartCommand request, CancellationToken cancellationToken)
        => await _service.ClearAsync(request.UserId, cancellationToken);
}
