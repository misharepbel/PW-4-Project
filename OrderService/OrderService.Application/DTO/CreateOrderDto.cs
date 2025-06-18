namespace OrderService.Application.DTO
{
    public sealed class CreateOrderDto
    {
        public Guid UserId { get; init; }
        public List<CreateOrderItemDto> Items { get; init; } = [];
        public string DeliveryLocation { get; init; } = string.Empty;
        public string PaymentMethod { get; init; } = string.Empty;
    }
}