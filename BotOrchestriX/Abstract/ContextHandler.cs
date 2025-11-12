using BotOrchestriX.Entity;
using Telegram.Bot.Types;

namespace BotOrchestriX.Abstract;

public abstract class ContextHandler<TPayload, TState> : IContextHandler
    where TPayload : BasePayload where TState : struct, Enum
{
    protected abstract Task Handle(Update update,
        DetailContext<TPayload, TState> context);

    protected abstract Task Enter(DetailContext<TPayload, TState> context);

    async Task IContextHandler.Handle(Update update, ChatContext context, IContextFactory contextFactory)
    {
        var detailContext = contextFactory.Create<TPayload, TState>(context);
        await Handle(update, detailContext);
    }


    async Task IContextHandler.Enter(ChatContext context, IContextFactory contextFactory)
    {
        var detailContext = contextFactory.Create<TPayload, TState>(context);
        await Enter(detailContext);
    }
}