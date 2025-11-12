using BotOrchestriX.Abstract;
using BotOrchestriX.Entity;

namespace BotOrchestriX;

internal interface IContextFactory
{
    public DetailContext<TPayload, TState> Create<TPayload, TState>(ChatContext context)
        where TState : struct, Enum where TPayload : BasePayload;
}