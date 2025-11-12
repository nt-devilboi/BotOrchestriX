using BotOrchestriX.Abstract;
using BotOrchestriX.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotOrchestriX.BaseCommand;

public class Help(List<InfoCommand> infoCommands, ITelegramBotClient botClient) : IHandler
{
    public string Trigger { get; } = "Что ты можешь";
    public string Desc { get; } = "Get All Commands";

    public Priority Priority { get; } = Priority.SystemCommand;

    public async Task Execute(Update update, ChatContext context)
    {
        await botClient.SendTextMessageAsync(update.Message.Chat.Id, string.Join("\n", infoCommands));
    }
}