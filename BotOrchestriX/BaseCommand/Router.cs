using BotOrchestriX.Entity;
using Telegram.Bot.Types;

namespace BotOrchestriX.Abstract;

public class Router<TContext>(string trigger) : Command where TContext : struct, Enum
{
    public override string Trigger { get; } = trigger;

    public override Task Execute(Update update, ChatContext context)
    {
        var firstState = Enum.GetValues<TContext>()[0];
        context.State = firstState.ToString();
        return Task.CompletedTask;
    }
}