namespace NotificationService.Messaging;

public record EmailMessage(string To, string Subject, string Body);
