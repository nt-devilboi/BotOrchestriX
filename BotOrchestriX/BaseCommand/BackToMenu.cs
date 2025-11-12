using BotOrchestriX.Abstract;
using BotOrchestriX.Entity;
using Telegram.Bot.Types;

namespace BotOrchestriX.BaseCommand;

public class BackToMenu : Command
{
    public override string Trigger { get; } = "Menu";

    public override Priority Priority { get; } = Priority.SystemCommand;

    public override async Task Execute(Update update, ChatContext context)
    {
        context.State = BaseContextState.Menu.ToString();
        await Task.CompletedTask;
    }
}