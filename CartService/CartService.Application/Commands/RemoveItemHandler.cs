using CartService.Application.Services;
using MediatR;

namespace CartService.Application.Commands;

public class RemoveItemHandler : IRequestHandler<RemoveItemCommand>
{
    private readonly ICartService _service;
    public RemoveItemHandler(ICartService service) => _service = service;

    public async Task Handle(RemoveItemCommand request, CancellationToken cancellationToken)
        => await _service.RemoveItemAsync(request.UserId, request.ProductId, cancellationToken);
}
