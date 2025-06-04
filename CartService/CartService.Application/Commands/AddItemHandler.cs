using CartService.Application.Services;
using MediatR;

namespace CartService.Application.Commands;

public class AddItemHandler : IRequestHandler<AddItemCommand>
{
    private readonly ICartService _service;
    public AddItemHandler(ICartService service) => _service = service;

    public async Task Handle(AddItemCommand request, CancellationToken cancellationToken)
        => await _service.AddItemAsync(request.UserId, request.Item, cancellationToken);
}
