namespace Todo.Identity.Messages.Events;

public class UserPasswordResetRequestedEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string ResetLink { get; set; }
    public string ResetToken { get; set; }
    public DateTime RequestedAt { get; set; }
    public string? ClientIP { get; set; }
}