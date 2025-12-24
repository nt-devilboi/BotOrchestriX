using BotOrchestriX.Entity;
using Microsoft.EntityFrameworkCore;

namespace BotOrchestriX.Infrastructure;

public abstract class ChatDb : DbContext
{
    protected ChatDb(DbContextOptions options) : base(options)
    {
    }

    protected ChatDb()
    {
    }

    public DbSet<ChatContext> ChatContexts { get; set; }
}