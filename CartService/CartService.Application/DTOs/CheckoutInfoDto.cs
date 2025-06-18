namespace CartService.Application.DTOs;

public class CheckoutInfoDto
{
    public string DeliveryLocation { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
}
