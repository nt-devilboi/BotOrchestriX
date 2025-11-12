using System.Diagnostics.CodeAnalysis;
using BotOrchestriX.Abstract;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotOrchestriX.controller;

[ApiController]
[Route("/api/message/update")]
public class BotController(
    ITelegramBotClient telegramBotClient,
    IUpdateProcess updateProcess)
{
    [HttpPost]
    [SuppressMessage("ReSharper.DPA", "DPA0011: High execution time of MVC action", MessageId = "time: 1264ms")]
    public async Task<IActionResult> Post([FromBody] Update? update)
    {
        if (update?.Message == null && update?.CallbackQuery == null) return new OkResult();

        try
        {
            await updateProcess.Update(update);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, $"Я сломался из за команды ```cs {e}```");
        }

        return new OkResult();
    }
}