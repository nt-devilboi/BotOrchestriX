using BotOrchestriX.Entity;

namespace BotOrchestriX;

public static class ContextExtension
{
    internal static void Start(this ChatContext context, long chatId)
    {
        context.State = BaseContextState.Menu.ToString();
        context.ChatId = chatId;
    }

    public static void ToMenu(this ChatContext context)
    {
        context.State = BaseContextState.Menu.ToString();
    }
}