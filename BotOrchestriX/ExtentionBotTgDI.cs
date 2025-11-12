using System.Reflection;
using BotOrchestriX.Abstract;
using BotOrchestriX.controller;
using BotOrchestriX.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace BotOrchestriX;

public static class ExtensionBotTgDi
{
    public static void AddTelegramBotWithController<TMainMenuHandler>(this IServiceCollection serviceCollection,
        string host,
        string token) where TMainMenuHandler : class, IStrategyMenu
    {
        serviceCollection.AddMvc().AddApplicationPart(Assembly.GetAssembly(typeof(BotController)));
        var client = new TelegramBotClient(token);
        var webhook = $"{host}/api/message/update";
        client.SetWebhookAsync(webhook,
            allowedUpdates: [UpdateType.Message, UpdateType.CallbackQuery, UpdateType.InlineQuery]).Wait();
        serviceCollection.AddSingleton<ITelegramBotClient>(client);
        serviceCollection.AddScoped<IUpdateProcess, UpdateProcess>();
        serviceCollection.AddScoped<MessageHandler>();
        serviceCollection.AddScoped<IStrategyMenu, TMainMenuHandler>();
        serviceCollection.AddScoped<IContextFactory, ContextFactory>();
        serviceCollection.AddScoped<ITriggerProvider, TriggerProvider>();
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

        var commandsTypes = GetCommandsFrom(assembly);
        foreach (var commandsType in commandsTypes)
        {
            serviceCollection.AddScoped<Command>(provider =>
                (Command)ActivatorUtilities.CreateInstance(provider, commandsType));
        }

        assembly = Assembly.GetCallingAssembly();

        commandsTypes = GetCommandsFrom(assembly);
        foreach (var commandsType in commandsTypes)
        {
            serviceCollection.AddScoped<Command>(provider =>
                (Command)ActivatorUtilities.CreateInstance(provider, commandsType));
        }

        return serviceCollection;
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