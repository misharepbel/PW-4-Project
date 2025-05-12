namespace OrderService.Application.DTO
{
    public sealed class CreateOrderDto
    {
        public Guid UserId { get; init; }
        public List<CreateOrderItemDto> Items { get; init; } = [];
    }
}