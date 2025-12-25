using BotOrchestriX.Entity;
using Telegram.Bot.Types;

namespace BotOrchestriX.Abstract;

public class Router(string trigger, FlowComponents flowComponents) : Command
{
    public override string Trigger { get; } = trigger;

    public override async Task Execute(Update update, ChatContext context)
    {
        context.State = flowComponents.Start;
        await Task.CompletedTask;
    }
}