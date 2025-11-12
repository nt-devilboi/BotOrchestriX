using BotOrchestriX.Abstract;
using BotOrchestriX.Entity;
using Microsoft.EntityFrameworkCore;

namespace BotOrchestriX.Infrastructure;

public class ContextRepository(ChatDb telegramDbContext) : IContextRepository {
    public async Task Upsert(ChatContext chatContext)
    {
        var context = await telegramDbContext.ChatContexts.FindAsync(chatContext.Id);

        if (context == null) telegramDbContext.ChatContexts.Add(chatContext);
        else telegramDbContext.Entry(context).CurrentValues.SetValues(chatContext);

        await telegramDbContext.SaveChangesAsync();
    }

    public async Task<ChatContext?> Get(long chatId)
    {
        var chatContext = await telegramDbContext.ChatContexts.FirstOrDefaultAsync(x => x.ChatId == chatId);

        return chatContext;
    }
}