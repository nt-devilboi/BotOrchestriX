using BotOrchestriX.Entity;

namespace BotOrchestriX.Abstract;

public interface IContextRepository
{
    Task Upsert(ChatContext chatContext);

    Task<ChatContext?> Get(long chatId);
}