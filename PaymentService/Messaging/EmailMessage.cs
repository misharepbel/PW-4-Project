namespace PaymentService.Messaging;

public record EmailMessage(string To, string Subject, string Body);
