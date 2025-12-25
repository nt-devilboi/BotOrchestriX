using BotOrchestriX.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SimpleExample.Handlers;

public class HowAreYouHandler(ITelegramBotClient botClient) : ContextHandler<GreetingPayload>
{
    protected override async Task Handle(Update update, DetailContext<GreetingPayload> context)
    {
        await botClient.SendMessage(context.ChatId, "Okay, bye");
        context.Reset();
    }

    protected override async Task Enter(DetailContext<GreetingPayload> context)
    {
        if (context.TryGetPayload(out var payload))
            await botClient.SendMessage(context.ChatId, $"How are you {payload.Name}");
    }
}