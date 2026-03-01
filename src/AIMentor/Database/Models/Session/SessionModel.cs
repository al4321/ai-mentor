namespace AIMentor.Database.Models.Session;

public class SessionModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public List<Message.MessageModel> Messages { get; } = [];
}
