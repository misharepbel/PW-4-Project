using System.Text.Json.Serialization;

namespace OrderService.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus
{
    New,
    Paid
}
