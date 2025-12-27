using BotOrchestriX.BuilderContext;
using BotOrchestriX.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotOrchestriX.Abstract;

public interface IStrategyMenu
{
    public Task Handle(ChatContext context);
}

internal class MessageHandler(
    ITelegramBotClient botClient,
    IStrategyMenu strategyMenu,
    GlobalRouter globalRouter)
    : IContextHandler
{
    public async Task Handle(Update update, ChatContext context, IContextFactory contextFactory)
    {
        await foreach (var action in globalRouter.Handle(update, context, contextFactory))
        {
            switch (action.result)
            {
                case RoutingResult.Executed:
                    await action.command();
                    break;
                case RoutingResult.NotFound:
                    await botClient.SendMessage(context.ChatId, "I don't understand");
                    break;
                case RoutingResult.EnterMenu:
                    await strategyMenu.Handle(context);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    async Task IContextHandler.Enter(ChatContext context, IContextFactory _)
    {
        await strategyMenu.Handle(context);
    }
}

internal enum RoutingResult
{
    Executed,
    NotFound,
    EnterMenu
}

internal class GlobalRouter
{
    private readonly Dictionary<string, Command> _commands;
    private readonly Dictionary<string, IContextHandler> _contexts;

    public GlobalRouter(
        IEnumerable<Command> commands,
        IEnumerable<ContextHandlerDescriptor> handlerInfos)
    {
        _commands = commands.ToDictionary(x => x.Trigger, x => x);
        _contexts = handlerInfos.ToDictionary(x => x.NameState, x => x.ContextHandler);
    }

    public async IAsyncEnumerable<(RoutingResult result, Func<Task>? command)> Handle(Update update,
        ChatContext context,
        IContextFactory contextFactory)
    {
        var text = update.Message?.Text;
        var oldState = context.State;

        if (_commands.TryGetValue(text ?? "", out var command) && command is { Priority: Priority.SystemCommand })
        {
            yield return (RoutingResult.Executed, async () => await command.Execute(update, context));
        }

        else if (_contexts.TryGetValue(context.State, out var contextHandler))
        {
            yield return (RoutingResult.Executed,
                async () => await contextHandler.Handle(update, context, contextFactory));
        }

        else if (command is { Priority: Priority.Command })
        {
            yield return (RoutingResult.Executed, async () => await command.Execute(update, context));
        }

        else
        {
            yield return (RoutingResult.NotFound, null);
        }

        while (string.CompareOrdinal(context.State, oldState) != 0)
        {
            oldState = context.State;

            if (_contexts.TryGetValue(context.State, out var nextHandler))
            {
                yield return (RoutingResult.Executed, async () => await nextHandler.Enter(context, contextFactory));
            }

            if (context.State == nameof(BaseContextState.Menu))
            {
                yield return (RoutingResult.EnterMenu, null);
            }
        }
    }
}