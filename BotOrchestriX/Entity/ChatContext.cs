namespace BotOrchestriX.Entity;

public class ChatContext
{
    public Guid Id { get; set; }
    public string State { get; set; } // как вариант можно сделать internal Set чтоб просто так нельзя было менять State.
    public string? Payload { get; set; }
    public long ChatId { get; set; }


    public static ChatContext CreateInAccountContext(long chatId)
    {
        return new ChatContext
        {
            State = BaseContextState.Menu.ToString(),
            Id = Guid.NewGuid(),
            ChatId = chatId
        };
    }
}

internal enum BaseContextState
{
    NotStart,
    Menu
}