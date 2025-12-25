using BotOrchestriX.Abstract;
using BotOrchestriX.Entity;

namespace BotOrchestriX;

internal interface IContextFactory
{
    public DetailContext<TPayload> Create<TPayload>(ChatContext context) where TPayload : BasePayload;
}