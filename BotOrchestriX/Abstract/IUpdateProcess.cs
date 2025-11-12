using BotOrchestriX.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotOrchestriX.Abstract;

public interface IUpdateProcess
{
    public Task Update(Update update);
}

internal class UpdateProcess(
    ITelegramBotClient telegramBotClient,
    MessageHandler messageHandler,
    IContextRepository contextRepository,
    IContextFactory contextFactory)
    : IUpdateProcess
{
    public async Task Update(Update update)
    {
        var id = GetChatId(update);

        if (id == null)
        {
            await telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id,
                "Я умею понимать только сообщения");
        }
        else
        {
            var context = await contextRepository.Get(id.Value) ?? NotAuthorized();
            await messageHandler.Handle(update, context, contextFactory);
        }
    }

    private long? GetChatId(Update update)
    {
        return update.Type switch
        {
            UpdateType.Message => update.Message?.Chat.Id,
            UpdateType.CallbackQuery => update.CallbackQuery?.Message?.Chat.Id,
            UpdateType.EditedMessage => update.EditedMessage?.Chat.Id,
            UpdateType.ChannelPost => update.ChannelPost?.Chat.Id,
            UpdateType.MyChatMember => update.MyChatMember?.Chat.Id,
            _ => null
        };
    }

    private ChatContext NotAuthorized()
    {
        return new ChatContext
        {
            State = BaseContextState.NotStart.ToString(),
            Id = Guid.NewGuid(),
            Payload = ""
        };
    }
}