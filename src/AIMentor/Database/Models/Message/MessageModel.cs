namespace AIMentor.Database.Models.Message;

public class MessageModel
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public required string Role { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }

    public int SessionId { get; set; }
    public Session.SessionModel? Session { get; set; }
}
