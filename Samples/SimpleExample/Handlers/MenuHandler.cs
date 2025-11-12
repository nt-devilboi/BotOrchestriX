using BotOrchestriX.Abstract;
using BotOrchestriX.Entity;
using Telegram.Bot;

namespace SimpleExample.Handlers;

public class MenuHandler(
    ITelegramBotClient botClient) : IStrategyMenu
{
    public async Task Handle(ChatContext context)
    {
        await botClient.SendTextMessageAsync(context.ChatId, "You are in menu. Say `Hello`");
    }
}