using BotOrchestriX.Abstract;
using BotOrchestriX.Entity;
using Telegram.Bot.Types;

namespace BotOrchestriX.BaseCommand;

public class StartHandler : Command
{
    public override string Trigger { get; } = "/start";

    public override async Task Execute(Update update, ChatContext context)
    {
        var chatId = update.Message.Chat.Id;
        context.Start(chatId);
    }
}