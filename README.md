# BotOrchestriX
A lightweight framework that simplifies bot development by encapsulating all the infrastructure so you can focus directly on your **business logic**.

Now it's work with **Telegram**
## 🚀 Why It Exists
Instead of spending time wiring up controllers, handlers, and message routing, **BotOrchestriX** lets you jump straight into defining how your bot should behave.  
It hides infrastructure complexity and gives you a clean entry point for working with logic and flows.


# Get started

Define a handler class that implements `IStrategyMenu`:
```csharp
public class MenuHandler(
    ITelegramBotClient botClient) : IStrategyMenu
{
    public async Task Handle(ChatContext context)
    {
        await botClient.SendTextMessageAsync(context.ChatId, "Hello User");
    }
}
```

Add Configuration
```csharp
builder.Services.AddBaseTelegramCommands();
builder.Services.AddTelegramBotWithController<MenuHandler>("HOST", "URL", "TG_TOKEN");

builder.Services.AddTelegramDbContext<ChatTelegramDb>();
```
Can use `localhost.run/docs/` for webhook

```csharp
app.MapTelegram();
```

Define a handler class with you business logic that implements `ContextHandler<BasePayload>`
```csharp
public class YourClass(ITelegramBotClient botClient) : ContextHandler<BasePayload>
```



Add Flow
```csharp
var registerFlow = new ServiceRegistryFlow();
builder.Services.AddFlow("trigger", x =>
    x.AddHandler<YourClass>()
        .AddHandler<YourClass>(), registerFlow);

builder.Services.AddSingleton<IServiceRegistryFlow>(registerFlow);
```

