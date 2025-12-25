using BotOrchestriX.Abstract;

namespace BotOrchestriX.BuilderContext;

internal record IContextHandlerDescriptor(IContextHandler ContextHandler, string number);