using BotOrchestriX.Abstract;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SimpleExample.Handlers;

public class HiHandler(ITelegramBotClient botClient) : ContextHandler<GreetingPayload, HelloFlow>
{
    protected override Task Handle(Update update, DetailContext<GreetingPayload, HelloFlow> context)
    {
        var name = update.Message!.Text;

        var payload = new GreetingPayload(name);
        context.UpdatePayload(payload);

        context.State.Continue();
        return Task.CompletedTask;
    }

    protected override async Task Enter(DetailContext<GreetingPayload, HelloFlow> context)
    {
        await botClient.SendTextMessageAsync(context.ChatId, "Hi, What is your name");
    }
}

public record GreetingPayload(string Name) : BasePayload;