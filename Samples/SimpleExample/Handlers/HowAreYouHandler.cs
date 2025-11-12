using BotOrchestriX.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SimpleExample.Handlers;

public class HowAreYouHandler(ITelegramBotClient botClient) : ContextHandler<GreetingPayload, HelloFlow>
{
    protected override async Task Handle(Update update, DetailContext<GreetingPayload, HelloFlow> context)
    {
        await botClient.SendTextMessageAsync(context.ChatId, "Okay, bye");
        context.Reset();
    }

    protected override async Task Enter(DetailContext<GreetingPayload, HelloFlow> context)
    {
        if (context.TryGetPayload(out var payload))
            await botClient.SendTextMessageAsync(context.ChatId, $"How are you {payload.Name}");
    }
}