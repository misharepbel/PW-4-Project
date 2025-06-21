namespace OrderService.Application.Interfaces;

public record EmailMessage(string To, string Subject, string Body);
