using BotOrchestriX.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotOrchestriX.Abstract;

public interface IStrategyMenu
{
    public Task Handle(ChatContext context);
}

internal class MessageHandler : IContextHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IContextRepository _contextRepository;
    private readonly Dictionary<string, Command> _commands;
    private readonly Dictionary<string, IContextHandler> _contexts;
    private readonly IStrategyMenu strategyMenu;

    public MessageHandler(IEnumerable<Command> commands, IEnumerable<IHandlerInfo> handlerInfos,
        ITelegramBotClient botClient, IContextRepository contextRepository,
        IStrategyMenu strategyMenu)
    {
        _botClient = botClient;
        _contextRepository = contextRepository;
        this.strategyMenu = strategyMenu;
        _commands = commands.ToDictionary(x => x.Trigger, x => x);
        _contexts = handlerInfos.ToDictionary(x => x.number, x => x.ContextHandler);
        _contexts.Add(BaseContextState.Menu.ToString(), this);
    }


    public async Task Handle(Update update, ChatContext context, IContextFactory contextFactory)
    {
        var text = update.Message?.Text;
        
        var oldState = context.State;
        if (_commands.TryGetValue(text ?? "", out var command) && command is { Priority: Priority.SystemCommand })
        {
            await command.Execute(update, context);
        }

        else if (_contexts.TryGetValue(context.State, out var contextHandler) && contextHandler != this)
        {
            await contextHandler.Handle(update, context, contextFactory);
        }

        else if (command is { Priority: Priority.Command })
        {
            await command.Execute(update, context);
        }

        else
        {
            await _botClient.SendTextMessageAsync(update.Message.Chat.Id, "Я ваще ничего не понял"); // сделать изменить
            return;
        }

        if (string.CompareOrdinal(context.State, oldState) != 0 &&
            _contexts.TryGetValue(context.State, out var nextHandler))
        {
            await nextHandler.Enter(context, contextFactory);
        }

        await _contextRepository.Upsert(context);
    }

    async Task IContextHandler.Enter(ChatContext context, IContextFactory _)
    {
        await strategyMenu.Handle(context);
    }
}