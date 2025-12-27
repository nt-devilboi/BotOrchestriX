using BotOrchestriX.Abstract;

namespace BotOrchestriX.BuilderContext;

internal record ContextHandlerDescriptor(IContextHandler ContextHandler, string NameState);