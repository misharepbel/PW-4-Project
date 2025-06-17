using System.ComponentModel.DataAnnotations;

namespace OrderService.Application.DTO
{
    public sealed class OrderDto
    {
        [Key]
        public Guid Id { get; init; } = Guid.NewGuid();
        public Guid UserId { get; init; }
        public DateTime OrderDate { get; init; }
        public string Status { get; init; } = default!;
        public List<OrderItemDto> Items { get; init; } = [];
        public string DeliveryLocation { get; init; } = string.Empty;
        public string PaymentMethod { get; init; } = string.Empty;
        public decimal Total => Items.Sum(i => i.UnitPrice * i.Quantity);
    }
}