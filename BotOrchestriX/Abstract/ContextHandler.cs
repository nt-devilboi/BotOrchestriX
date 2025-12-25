using BotOrchestriX.Entity;
using Telegram.Bot.Types;

namespace BotOrchestriX.Abstract;

public abstract class ContextHandler<TPayload> : IContextHandler
    where TPayload : BasePayload
{
    protected abstract Task Handle(Update update,
        DetailContext<TPayload> context);

    protected abstract Task Enter(DetailContext<TPayload> context);

    async Task IContextHandler.Handle(Update update, ChatContext context, IContextFactory contextFactory)
    {
        var detailContext = contextFactory.Create<TPayload>(context);
        await Handle(update, detailContext);
    }


    async Task IContextHandler.Enter(ChatContext context, IContextFactory contextFactory)
    {
        var detailContext = contextFactory.Create<TPayload>(context);
        await Enter(detailContext);
    }
}