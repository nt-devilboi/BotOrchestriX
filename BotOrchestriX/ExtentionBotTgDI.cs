using System.Reflection;
using System.Text.RegularExpressions;
using BotOrchestriX.Abstract;
using BotOrchestriX.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotOrchestriX;

internal record RouteSettings(string Host, string Url);

public static class ExtensionSetup
{
    public static async Task AddTelegramBot<TMainMenuHandler>(this IServiceCollection serviceCollection,
        string host, string uri,
        string token) where TMainMenuHandler : class, IStrategyMenu
    {
        serviceCollection.AddControllers().AddNewtonsoftJson();

        var client = new TelegramBotClient(token);
        var webhook = $"{host}/{uri}";
        client.SetWebhook(webhook,
            allowedUpdates: [UpdateType.Message, UpdateType.CallbackQuery, UpdateType.InlineQuery]).Wait();

        //todo: реализовать проверку webHook
        var webhookInfo = await client.GetWebhookInfo();
        

        Console.WriteLine($"webhook address {host}/{uri}");


        serviceCollection.AddSingleton<ITelegramBotClient>(client);
        serviceCollection.AddScoped<IUpdateProcess, UpdateProcess>();
        serviceCollection.AddScoped<MessageHandler>();
        serviceCollection.AddScoped<IStrategyMenu, TMainMenuHandler>();
        serviceCollection.AddScoped<IContextFactory, ContextFactory>();
        serviceCollection.AddScoped<ITriggerProvider, TriggerProvider>();
        serviceCollection.AddSingleton(new RouteSettings(host, uri));
    }


    public static void MapTelegram(this IEndpointRouteBuilder application)
    {
        var url = application.ServiceProvider.GetService<RouteSettings>().Url;
        application.MapPost(url, async
        (
            [FromBody] Update update,
            [FromServices] ITelegramBotClient telegramBotClient,
            IUpdateProcess updateProcess
        ) =>
        {
            if (update?.Message == null && update?.CallbackQuery == null) return new OkResult();
            try
            {
                await updateProcess.Update(update);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await telegramBotClient.SendMessage(update.Message.Chat.Id, $"Я сломался из за команды ```cs {e}```");
            }

            return new OkResult();
        });
    }

    public static void AddTelegramDbContext<TDb>(this IServiceCollection serviceCollection) where TDb : ChatDb
    {
        serviceCollection.AddDbContext<TDb>();
        serviceCollection.AddDbContext<ChatDb, TDb>();

        serviceCollection.AddScoped<IContextRepository, ContextRepository>();
    }

    public static IServiceCollection AddBaseTelegramCommands(this IServiceCollection serviceCollection)
    {
        var assembly = Assembly.GetExecutingAssembly();
        AddCommand(serviceCollection, assembly);

        assembly = Assembly.GetCallingAssembly();
        AddCommand(serviceCollection, assembly);

        return serviceCollection;
    }

    private static void AddCommand(IServiceCollection serviceCollection, Assembly assembly)
    {
        var commandsTypes = GetCommandsFrom(assembly);
        foreach (var commandsType in commandsTypes)
        {
            serviceCollection.AddScoped<Command>(provider =>
                (Command)ActivatorUtilities.CreateInstance(provider, commandsType));
        }
    }


    private static Type[] GetCommandsFrom(Assembly assembly)
    {
        return assembly
            .GetTypes()
            .Where(t => t is { BaseType: not null, IsAbstract: false } &&
                        t.BaseType == typeof(Command) && t != typeof(Router<>))
            .ToArray();
    }
}