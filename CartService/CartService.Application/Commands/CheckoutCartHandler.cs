using CartService.Application.DTOs;
using CartService.Application.Interfaces;
using CartService.Application.Services;
using MediatR;

namespace CartService.Application.Commands;

public class CheckoutCartHandler : IRequestHandler<CheckoutCartCommand>
{
    private readonly ICartService _service;
    private readonly ICartCheckoutProducer _producer;

    public CheckoutCartHandler(ICartService service, ICartCheckoutProducer producer)
    {
        _service = service;
        _producer = producer;
    }

    public async Task Handle(CheckoutCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await _service.GetAsync(request.UserId, cancellationToken);
        if (cart == null || cart.Items.Count == 0)
            throw new InvalidOperationException("Cart is empty.");

        var evt = new CartCheckedOutEvent { UserId = cart.UserId, Items = cart.Items };
        await _producer.PublishAsync(evt, cancellationToken);
    }
}
