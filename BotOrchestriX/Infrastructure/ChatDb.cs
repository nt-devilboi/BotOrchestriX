using BotOrchestriX.Entity;
using Microsoft.EntityFrameworkCore;

namespace BotOrchestriX.Infrastructure;

public abstract class ChatDb : DbContext
{
    public DbSet<ChatContext> ChatContexts { get; set; }
}